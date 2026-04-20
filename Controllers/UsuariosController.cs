using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DropDownsAnidadosMvc.Models;

namespace DropDownsAnidadosMvc.Controllers
{
    // Todo el controlador es exclusivo del Admin
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();

            // Para cada usuario obtenemos sus roles y los guardamos en el ViewModel
            var vm = new List<UsuarioRolVM>();
            foreach (var u in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(u);
                vm.Add(new UsuarioRolVM
                {
                    Id    = u.Id,
                    Email = u.Email ?? "",
                    Rol   = roles.FirstOrDefault() ?? "Sin rol"
                });
            }

            return View(vm);
        }

        // GET: Usuarios/EditRol/id
        public async Task<IActionResult> EditRol(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(usuario);
            var vm = new UsuarioRolVM
            {
                Id    = usuario.Id,
                Email = usuario.Email ?? "",
                Rol   = roles.FirstOrDefault() ?? "Sin rol"
            };

            return View(vm);
        }

        // POST: Usuarios/EditRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRol(string id, string nuevoRol)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();

            // No permitir cambiar el rol del propio Admin logueado
            if (usuario.Email == "admin@parador.com" && nuevoRol != "Admin")
            {
                TempData["Error"] = "No puedes cambiar el rol del administrador principal.";
                return RedirectToAction(nameof(Index));
            }

            // Quitamos todos los roles actuales y asignamos el nuevo
            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            await _userManager.AddToRoleAsync(usuario, nuevoRol);

            TempData["Success"] = $"Rol de {usuario.Email} cambiado a {nuevoRol}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
