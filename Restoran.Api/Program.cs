using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Restoran.Core.Business;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Restoran API", Version = "v1" });
    
    // JWT için Swagger konfigürasyonu
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Token giriniz: Bearer {token}"
    });
    
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// JWT Authentication - Core'daki BLLJwt ayarlarını kullan
var jwtHelper = new BLLJwt();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtHelper.GetSecretKey())),
        ValidateIssuer = true,
        ValidIssuer = jwtHelper.GetIssuer(),
        ValidateAudience = true,
        ValidAudience = jwtHelper.GetAudience(),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

// Authorization - Role-based policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOrAdmin", policy => policy.RequireRole("Customer", "Admin"));
    options.AddPolicy("AllUsers", policy => policy.RequireRole("Guest", "Customer", "Admin"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Admin kullanıcısı oluştur
await CreateAdminUser();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Admin kullanıcısı oluşturma metodu
static async Task CreateAdminUser()
{
    var authBll = new BLLAuth();
    
    // Önce admin kullanıcısının var olup olmadığını kontrol et
    var adminUser = await authBll.LoginAsync(new Restoran.Core.DTOs.User.UserLoginDto 
    { 
        Username = "admin", 
        Password = "123456" 
    });
    
    // Admin kullanıcısı yoksa oluştur
    if (adminUser == null)
    {
        var adminRegisterDto = new Restoran.Core.DTOs.User.UserRegisterDto
        {
            FirstName = "Admin",
            LastName = "User",
            Username = "admin",
            Email = "admin@restoran.com",
            Phone = "555-0000",
            Password = "admin123456"
        };
        
        var result = await authBll.RegisterAsync(adminRegisterDto);
        
        if (result.Success)
        {
            // Admin rolünü ayarla
            var userBll = new BLLUser();
            var users = await userBll.GetUsersAsync();
            var createdAdmin = users.FirstOrDefault(u => u.Username == "admin");
            
            if (createdAdmin != null)
            {
                var updateDto = new Restoran.Core.DTOs.User.UserUpdateDto
                {
                    FirstName = "Admin",
                    LastName = "User", 
                    Username = "admin",
                    Email = "admin@restoran.com",
                    Phone = "555-0000",
                    Role = Restoran.Core.Statics.Enums.UserRole.Admin
                };
                
                await userBll.UpdateUserAsync(createdAdmin.Id, updateDto);
                Console.WriteLine("Admin kullanıcısı oluşturuldu: Username=admin, Password=123456");
            }
        }
        else
        {
            Console.WriteLine($"Admin kullanıcısı oluşturulamadı: {result.Message}");
        }
    }
    else
    {
        Console.WriteLine("Admin kullanıcısı zaten mevcut.");
    }
}