using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Restoran.Core.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
Console.WriteLine(" Logging aktif");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Kimlik doðrulama servislerini ekle
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giriþ sayfasý yolu
        options.LogoutPath = "/Account/Logout"; // Çýkýþ sayfasý yolu
        options.AccessDeniedPath = "/Account/AccessDenied"; // Eriþim engellendi sayfasý
    });


// Add services to the container.  
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
//    RequestPath = "/uploads"
//});



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// Fix for CS1503: Use a lambda to configure AutoMapper  


app.UseHttpsRedirection();
app.UseStaticFiles();


// Kimlik doðrulama ve yetkilendirme middleware'lerini ekle
app.UseAuthentication(); // ÖNEMLÝ: Authentication middleware'i
app.UseAuthorization();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
