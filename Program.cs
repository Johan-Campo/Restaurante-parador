using DropDownsAnidadosMvc.Datos;
using DropDownsAnidadosMvc.Models;
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

    builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
    });

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
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

    builder.Services.AddHostedService<DatabaseInitializer>();

    var app = builder.Build();

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
    app.UseSession();
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
    throw;
}
