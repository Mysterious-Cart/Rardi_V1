using Radzen;
using Microsoft.EntityFrameworkCore;
using CHKS.Data;
using Microsoft.AspNetCore.Identity;
using CHKS.Models;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using CHKS.Models.Interface;
using CHKS.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});

builder.Services.AddRazorPages();

builder.Services
    .AddMudServices()
    .AddScoped<InventoryNotificationHubConnectionService>()
    .AddScoped<InventoryControlService>()
    .AddScoped<IDbProvider, DbProvider<Rardi_Context>>()
    .AddScoped<CartControlService>()
    .AddScoped<StockLogsTrackingService>()
    .AddScoped<EmployeeControl>()
    .AddTransient<VehicleAPI>()
    .AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
    });

builder.Services.AddLogging(config => {
    config.AddConsole();
    config.AddDebug();
});


/* REGISTER DATABASE CONTEXTS */
var connectionString = builder.Configuration.GetConnectionString("development");

builder.Services
    .AddDbContextFactory<Rardi_Context>(
        options =>{
            options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
            );
        }
    ).AddDbContext<ApplicationIdentityDbContext>(
        options =>
        {
            options.UseMySql(
                connectionString, 
                ServerVersion.AutoDetect(connectionString)
            );
    });

/* REGISTER CLIENT HTTPS */
builder.Services
    .AddHttpClient("CHKS")
    .ConfigurePrimaryHttpMessageHandler(
        () => new HttpClientHandler { UseCookies = false })
            .AddHeaderPropagation(o => o.Headers.Add("Cookie")      
    );

builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.None; // Allow cross-site cookies
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Secure if HTTPS
    });

/* SECURITY SERVICES */

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<SecurityService>()
                .AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>()
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddCors(
    options =>{
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    }
);

/* BUILDING */
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseHeaderPropagation();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapHub<InventoryNotificationHub>("/inventorylogs");
app.MapFallbackToPage("/_Host");

app.Run();