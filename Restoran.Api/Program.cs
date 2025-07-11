using Microsoft.EntityFrameworkCore;
using Restoran.Core.Statics;

var builder = WebApplication.CreateBuilder(args);

// Connection string'i uygulama başlangıcında ayarla
//AppConfiguration.SetConnectionString(
//    builder.Configuration.GetConnectionString("DefaultConnection")!);

// Sadece AutoMapper ve Service'leri kaydet


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
