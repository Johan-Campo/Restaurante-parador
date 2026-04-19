using DropDownsAnidadosMvc.Datos;
using DropDownsAnidadosMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DropDownsAnidadosMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _contexto;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext contexto, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _contexto = contexto;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var sucursales = _contexto.Sucursal.ToList();

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (HttpContext.Session.GetString("WelcomeShown") == null)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var nombre = !string.IsNullOrEmpty(user?.Nombre) ? user.Nombre : User.Identity.Name;
                    TempData["Success"] = $"¡Bienvenido, {nombre}!";
                    HttpContext.Session.SetString("WelcomeShown", "1");
                }
            }

            var viewModel = new DropDownsVM
            {
                Sucursales = sucursales,
                Producto   = new Producto()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public JsonResult ObtenerCategorias(int sucursalId)
        {
            var categorias = _contexto.Categoria.Where(c => c.SucursalId == sucursalId).ToList();
            return Json(categorias);
        }

        [HttpGet]
        public JsonResult ObtenerProductos(int categoriaId)
        {
            var productos = _contexto.Producto.Where(c => c.CategoriaId == categoriaId).ToList();
            return Json(productos);
        }
    }
}
