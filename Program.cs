    using DropDownsAnidadosMvc.Datos;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;

    try
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuración conexión SQL
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();
    

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
        });

        // Forzar login en toda la aplicación (excepto Login y Register)
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

        using (var scope = app.Services.CreateScope())
        {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        }

        var supportedCultures = new[] { new CultureInfo("es-CO") };

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("es-CO"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-CO");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-CO");

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

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