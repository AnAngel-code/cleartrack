using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using TicketSystem.Components;
using TicketSystem.Components.Account;
using TicketSystem.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
//    options.UseSqlite(connectionString));

//options.UseNpgsql(connectionString));

static string BuildNpgsqlConnectionStringFromDatabaseUrl(string databaseUrl)
{
    var uri = new Uri(databaseUrl);

    var userInfo = uri.UserInfo.Split(':', 2);
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";

    var database = uri.AbsolutePath.TrimStart('/');
    
    return $"Host={uri.Host};Port={uri.Port};Database={database};Username={username};Password={password};SSL Mode=Disable";
}


var sqliteConn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlite(sqliteConn);
    else
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        
        Console.WriteLine($"DATABASE_URL present? {(!string.IsNullOrWhiteSpace(databaseUrl))}, length={(databaseUrl?.Length ?? 0)}");
        if (string.IsNullOrWhiteSpace(databaseUrl))
            throw new InvalidOperationException("DATABASE_URL not found in environment.");

        //var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
        //    ?? throw new InvalidOperationException("DATABASE_URL not found in environment.");

        var npgsqlConn = BuildNpgsqlConnectionStringFromDatabaseUrl(databaseUrl);
        options.UseNpgsql(npgsqlConn);
    }     

});
   
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- ADDED FOR SECURITY ---
builder.Services.AddCascadingAuthenticationState();

// --- ADDED FOR RADZEN ---
builder.Services.AddRadzenComponents();


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddScoped<IdentityRedirectManager>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseAntiforgery(); 

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();
app.MapAdditionalIdentityEndpoints();

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    
    await using var db = await factory.CreateDbContextAsync();
    db.Database.Migrate();

    await SeedData.Initialize(scope.ServiceProvider);
}

app.Run();