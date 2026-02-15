using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using TicketSystem.Components;
using TicketSystem.Components.Account;
using TicketSystem.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(connectionString);
        Console.WriteLine("In Dev");
    }        
    else
    {        
        options.UseNpgsql(connectionString);
        Console.WriteLine("In Prod");
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