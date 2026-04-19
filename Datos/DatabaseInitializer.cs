using DropDownsAnidadosMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DropDownsAnidadosMvc.Datos
{
    public class DatabaseInitializer : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IServiceProvider services, ILogger<DatabaseInitializer> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            const int maxIntentos = 6;
            for (int intento = 1; intento <= maxIntentos; intento++)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    await db.Database.MigrateAsync(cancellationToken);

                    foreach (var role in new[] { "Admin", "Mesero" })
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                            await roleManager.CreateAsync(new IdentityRole(role));
                    }

                    var adminEmail = "admin@parador.com";
                    if (await userManager.FindByEmailAsync(adminEmail) == null)
                    {
                        var admin = new ApplicationUser
                        {
                            UserName       = adminEmail,
                            Email          = adminEmail,
                            EmailConfirmed = true,
                            Nombre         = "Admin",
                            Apellido       = "Parador"
                        };
                        await userManager.CreateAsync(admin, "Admin123!");
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }

                    _logger.LogInformation("Migración y seed completados en intento {Intento}.", intento);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Migración fallida (intento {Intento}/{Max}): {Error}", intento, maxIntentos, ex.Message);
                    if (intento < maxIntentos)
                        await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                }
            }

            _logger.LogError("Se agotaron los reintentos de migración.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
