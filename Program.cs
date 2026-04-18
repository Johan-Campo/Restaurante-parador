using DropDownsAnidadosMvc.Datos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()          // Habilita RoleManager — sin esto los roles no funcionan
    .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
    });

    builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
    });

    builder.Services.AddRazorPages(options =>
    {
        options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
        options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Register");
    });

    var app = builder.Build();

    // ── Migraciones y seed de roles/admin ─────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        db.Database.Migrate();

        // Crear roles si no existen — se ejecuta en cada arranque sin problema
        // porque la condición verifica primero si ya existen.
        string[] roles = { "Admin", "Mesero" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Crear usuario Admin por defecto si no existe
        // Cambia la contraseña después del primer login desde el panel de usuarios.
        var adminEmail = "admin@parador.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    var supportedCultures = new[] { new CultureInfo("es-CO") };
    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("es-CO"),
        SupportedCultures     = supportedCultures,
        SupportedUICultures   = supportedCultures
    });

    CultureInfo.DefaultThreadCurrentCulture   = new CultureInfo("es-CO");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-CO");

    if (!app.Environment.IsDevelopment())
        app.UseExceptionHandler("/Home/Error");

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("=== ERROR AL INICIAR LA APP ===");
    Console.WriteLine(ex.ToString());
    Console.WriteLine("Presione cualquier tecla para cerrar...");
    Console.ReadKey();
}
