using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radzen;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<CHKS.mydbService>();
builder.Services.AddDbContext<CHKS.Data.mydbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mydbConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mydbConnection")));
});
builder.Services.AddScoped<CHKS.fordevService>();
builder.Services.AddDbContext<CHKS.Data.fordevContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("fordevConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("fordevConnection")));
});
builder.Services.AddScoped<CHKS.fordevlocalService>();
builder.Services.AddDbContext<CHKS.Data.fordevlocalContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("fordevlocalConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("fordevlocalConnection")));
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
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();