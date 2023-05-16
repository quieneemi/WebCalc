using SmartCalc.Core;
using SmartCalc.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISmartCalcService, SmartCalcService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();

builder.Services.AddRazorPages();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{expression?}");

app.Run();