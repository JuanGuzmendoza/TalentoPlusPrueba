using TalentoPlus.Domain.Entities;
using TalentoPlus.Extensions; 
using TalentoPlus.Infrastructure; 
using TalentoPlus.Infrastructure.Persistence;
using TalentoPlus.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application;      
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddHttpClient();
// Capa de API (Auth, Identity, Controllers, Swagger)
builder.Services.AddApiConfiguration();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();


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


app.MapIdentityApi<User>(); 
app.MapControllers();     

app.Run();