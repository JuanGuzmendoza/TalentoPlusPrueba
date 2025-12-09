using TalentoPlus.Domain.Entities;
using TalentoPlus.Extensions; // Para usar ApiExtensions
using TalentoPlus.Infrastructure; // Para usar DependencyInjection de Infrastructure
using TalentoPlus.Infrastructure.Persistence;
using TalentoPlus.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application;      // Para AddApplication
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. INYECCI�N DE DEPENDENCIAS (ORGANIZADO)
// ==========================================

// Capa de Infraestructura (Base de datos)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddHttpClient();
// Capa de API (Auth, Identity, Controllers, Swagger)
builder.Services.AddApiConfiguration();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// ==========================================
// 2. PIPELINE Y CONFIGURACI�N
// ==========================================

// Seed de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        await DbSeeder.SeedAsync(services);
        Console.WriteLine("? Base de datos inicializada correctamente.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "? Error en el Seeding.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de rutas
app.MapIdentityApi<User>(); // Endpoints m�gicos de Identity
app.MapControllers();       // Tus controladores manuales

app.Run();