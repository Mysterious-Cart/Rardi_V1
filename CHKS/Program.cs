using Radzen;
using Microsoft.EntityFrameworkCore;
using CHKS.Data;
using Microsoft.AspNetCore.Identity;
using CHKS.Models;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using CHKS.Models.Interface;
using CHKS.Services;
using Python.Runtime;
using Microsoft.OData;
using Microsoft.AspNetCore.OData;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = true;
});
builder.Services.AddMudServices();

builder.Services.AddScoped<InventoryNotificationHubConnectionService>();


builder.Services.AddScoped<InventoryControlService>();
builder.Services.AddScoped<IDbProvider, DbProvider<Rardi_Context>>();
builder.Services.AddScoped<CartControlService>();
builder.Services.AddScoped<StockLogsTrackingService>();

builder.Services.AddTransient<EmployeeControl>();
builder.Services.AddTransient<VehicleAPI>();

builder.Services.AddLogging(config => {
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddControllers().AddOData(options =>
{
    options.Select().Filter().OrderBy().Expand().SetMaxTop(100).Count();
});

builder.Services.AddDbContextFactory<Rardi_Context>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("development"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("development")));
});

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("development"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("development")));
});


builder.Services.AddHttpClient("CHKS").ConfigurePrimaryHttpMessageHandler(
    () => new HttpClientHandler { UseCookies = false })
        .AddHeaderPropagation(o => o.Headers.Add("Cookie")
        
);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None; // Allow cross-site cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Secure if HTTPS
});

builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<SecurityService>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
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