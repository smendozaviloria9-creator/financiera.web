using Financiera.Web.Repositories;
using Financiera.Web.Services;
using Financiera.Web.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<IAssociatedRepository, AssociatedRepository>();
builder.Services.AddSingleton<IMovementRepository, MovementRepository>();
builder.Services.AddHttpClient<ExchangeRateService>();
builder.Services.AddScoped<BankingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();