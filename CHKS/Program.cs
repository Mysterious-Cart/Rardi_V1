using Radzen;
using Microsoft.EntityFrameworkCore;
using CHKS.Data;
using Microsoft.AspNetCore.Identity;
using CHKS.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MudBlazor.Services;
using CHKS.Models.Interface;
using CHKS.Services;
using Python.Runtime;


var builder = WebApplication.CreateBuilder(args);

Runtime.PythonDLL = builder.Configuration.GetSection("DLL")["PythonDLL"];


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});
builder.Services.AddMudServices();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<NotificationService>();


builder.Services.AddTransient<InventoryControlService>();
builder.Services.AddTransient<CartControlService>();
builder.Services.AddTransient<IDbProvider, DbProvider<mydbContext>>();
builder.Services.AddTransient<VehicleAPI>();
builder.Services.AddLogging(config => {
    config.AddConsole();
    config.AddDebug();
});


builder.Services.AddDbContextFactory<mydbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("development"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("development")));
});

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mydbConnection"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mydbConnection")));
});


builder.Services.AddHttpClient("CHKS").ConfigurePrimaryHttpMessageHandler(
    () => new HttpClientHandler { UseCookies = false }).AddHeaderPropagation(o => o.Headers.Add("Cookie")
);

builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<SecurityService>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddControllers().AddOData(o =>
{
    var oDataBuilder = new ODataConventionModelBuilder();
    oDataBuilder.EntitySet<ApplicationUser>("ApplicationUsers");
    var usersType = oDataBuilder.StructuralTypes.First(x => x.ClrType == typeof(ApplicationUser));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Password)));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.ConfirmPassword)));
    oDataBuilder.EntitySet<ApplicationRole>("ApplicationRoles");
    o.AddRouteComponents("odata/Identity", oDataBuilder.GetEdmModel()).Count().Filter().OrderBy().Expand()
    .Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
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
app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();