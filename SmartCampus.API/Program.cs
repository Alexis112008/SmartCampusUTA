using Microsoft.EntityFrameworkCore;
using SmartCampus.API.Application.Services;
using SmartCampus.API.Persistence.Context;
using SmartCampus.API.Persistence.Repositories;
using SmartCampus.API.Persistence.Repositories;
using SmartCampus.API.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Controladores
builder.Services.AddControllers();

// Repositories
builder.Services.AddScoped<TramiteRepository>();
builder.Services.AddScoped<TurnoRepository>();
builder.Services.AddScoped<CampusRepository>();

// Services
builder.Services.AddScoped<TramiteService>();
builder.Services.AddScoped<TurnoService>();
builder.Services.AddScoped<CampusService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<RecuperacionService>();

// 2. Swagger (documentación automática de la API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Conexión a SQL Server con Entity Framework Core
builder.Services.AddDbContext<SmartCampusDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// 4. CORS: permite que el frontend (HTML en VSCode) llame al backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 5. Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();