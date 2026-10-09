using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Business.Services;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.DataAccess.Context;
using TrafficFineManagement.DataAccess.Repositories;
using TrafficFineManagement.DataAccess.Repositories.Interfaces;
using TrafficFineManagement.DataAccess.Seed;
using TrafficFineManagement.Entities.Identity;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);


// Katman servisleri
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITrafficFineRepository, TrafficFineRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<ITrafficFineService, TrafficFineService>();

// Veritabanı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Kimlik doğrulama + roller
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Katman servisleri
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Form verileri (ondalık sayı, tarih) kültürden bağımsız okunsun; ekranda Türkçe formatlıyoruz
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Geliştirme ortamında migration + test verisi
if (app.Environment.IsDevelopment())
{
    await DbSeeder.SeedAsync(app.Services);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();