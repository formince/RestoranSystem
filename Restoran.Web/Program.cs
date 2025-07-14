using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Kimlik do�rulama servislerini ekle
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giri� sayfas� yolu
        options.LogoutPath = "/Account/Logout"; // ��k�� sayfas� yolu
        options.AccessDeniedPath = "/Account/AccessDenied"; // Eri�im engellendi sayfas�
    });


// Add services to the container.  
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();




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


// Kimlik do�rulama ve yetkilendirme middleware'lerini ekle
app.UseAuthentication(); // �NEML�: Authentication middleware'i
app.UseAuthorization();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
