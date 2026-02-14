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
    options.UseSqlite(connectionString));

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
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Run();