using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DropDownsAnidadosMvc.Datos;
using DropDownsAnidadosMvc.Models;

namespace DropDownsAnidadosMvc.Controllers
{
    public class PedidosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PedidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            // Include(p => p.Items) carga los ítems de cada pedido en la misma consulta.
            // ThenInclude(i => i.Producto) carga además el producto de cada ítem.
            // Sin esto, Pedido.Items estaría vacío y Pedido.Total daría 0.
            var pedidos = await _context.Pedido
                .Include(p => p.Items)
                    .ThenInclude(i => i.Producto)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();

            return View(pedidos);
        }

        // GET: Pedidos/Create
        public async Task<IActionResult> Create()
        {
            var productos = await _context.Producto
                .Include(p => p.Categoria)
                .OrderBy(p => p.Categoria.Nombre)
                .ThenBy(p => p.Nombre)
                .ToListAsync();

            if (!productos.Any())
            {
                TempData["Warning"] = "Debe crear productos antes de tomar un pedido.";
                return RedirectToAction(nameof(Index));
            }

            // Convertimos cada Producto en un ItemSeleccionVM con Cantidad = 0.
            // El usuario solo subirá la cantidad de los productos que quiera incluir.
            var vm = new CrearPedidoVM
            {
                Items = productos.Select(p => new ItemSeleccionVM
                {
                    ProductoId       = p.Id,
                    NombreProducto   = p.Nombre,
                    NombreCategoria  = p.Categoria?.Nombre ?? "",
                    Precio           = p.Precio,
                    ImagenUrl        = p.ImagenUrl
                }).ToList()
            };

            return View(vm);
        }

        // POST: Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearPedidoVM vm)
        {
            // Filtramos solo los productos que tienen Cantidad > 0
            var seleccionados = vm.Items.Where(i => i.Cantidad > 0).ToList();

            if (!seleccionados.Any())
            {
                ModelState.AddModelError("", "Debes seleccionar al menos un producto.");
            }

            if (ModelState.IsValid)
            {
                // Consultamos los precios actuales desde la BD para evitar manipulación
                // (el usuario no debería poder cambiar el precio desde el formulario).
                var ids = seleccionados.Select(i => i.ProductoId).ToList();
                var precios = await _context.Producto
                    .Where(p => ids.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, p => p.Precio);

                var pedido = new Pedido
                {
                    FechaCreacion = DateTime.Now,
                    Estado        = EstadoPedido.Pendiente,
                    NombreCliente = vm.NombreCliente,
                    NumeroMesa    = vm.NumeroMesa,
                    Observaciones = vm.Observaciones,
                    Items = seleccionados.Select(i => new PedidoProducto
                    {
                        ProductoId     = i.ProductoId,
                        Cantidad       = i.Cantidad,
                        // Tomamos el precio real de la BD, no el que vino del formulario
                        PrecioUnitario = precios.ContainsKey(i.ProductoId) ? precios[i.ProductoId] : 0
                    }).ToList()
                };

                _context.Pedido.Add(pedido);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Pedido #{pedido.Id} creado correctamente.";
                return RedirectToAction(nameof(Details), new { id = pedido.Id });
            }

            // Si hay error, recargamos los nombres/imágenes de productos (no vienen en el form)
            var productos = await _context.Producto
                .Include(p => p.Categoria)
                .ToListAsync();

            for (int i = 0; i < vm.Items.Count; i++)
            {
                var prod = productos.FirstOrDefault(p => p.Id == vm.Items[i].ProductoId);
                if (prod != null)
                {
                    vm.Items[i].NombreProducto  = prod.Nombre;
                    vm.Items[i].NombreCategoria = prod.Categoria?.Nombre ?? "";
                    vm.Items[i].Precio          = prod.Precio;
                    vm.Items[i].ImagenUrl       = prod.ImagenUrl;
                }
            }

            return View(vm);
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedido
                .Include(p => p.Items)
                    .ThenInclude(i => i.Producto)
                        .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            return View(pedido);
        }

        // GET: Pedidos/EditEstado/5
        public async Task<IActionResult> EditEstado(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null) return NotFound();

            return View(pedido);
        }

        // POST: Pedidos/EditEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEstado(int id, EstadoPedido estado)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Estado = estado;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Estado actualizado a: {estado}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedido
                .Include(p => p.Items)
                    .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedido.Remove(pedido);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Pedido eliminado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
