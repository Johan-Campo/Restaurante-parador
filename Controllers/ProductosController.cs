using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DropDownsAnidadosMvc.Datos;
using DropDownsAnidadosMvc.Models;

namespace DropDownsAnidadosMvc.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        // IWebHostEnvironment nos da acceso a la ruta física de wwwroot en el servidor.
        public ProductosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Productos
        // buscar y pagina llegan desde la URL: /Productos?buscar=pizza&pagina=2
        // ASP.NET los inyecta automáticamente en los parámetros (esto se llama model binding).
        public async Task<IActionResult> Index(string? buscar, int pagina = 1)
        {
            const int tamanioPagina = 5;

            // Construimos la consulta base con la relación Categoria cargada.
            // IQueryable<T> es importante: aún NO ejecuta el SQL, solo lo prepara.
            // Así podemos encadenar .Where(), .Skip(), .Take() y EF los convierte
            // en una sola consulta SQL eficiente al final.
            var query = _context.Producto
                .Include(p => p.Categoria)
                .AsQueryable();

            // Si el usuario escribió algo, filtramos por nombre o categoría.
            // .Contains() genera un LIKE '%texto%' en SQL.
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var termino = buscar.Trim().ToLower();
                query = query.Where(p =>
                    p.Nombre.ToLower().Contains(termino) ||
                    p.Categoria.Nombre.ToLower().Contains(termino));
            }

            // CountAsync() ejecuta un SELECT COUNT(*) — necesitamos el total
            // ANTES de paginar para calcular cuántas páginas existen.
            var total = await query.CountAsync();

            // Skip salta los registros de páginas anteriores.
            // Ejemplo: página 3, 5 por página → skip = (3-1)*5 = 10 → salta los primeros 10.
            // Take toma solo los siguientes 5.
            var productos = await query
                .OrderBy(p => p.Nombre)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync(); // aquí sí se ejecuta el SQL

            if (total == 0)
                TempData["Info"] = string.IsNullOrWhiteSpace(buscar)
                    ? "No existen productos registrados."
                    : $"No se encontraron productos para \"{buscar}\".";

            var vm = new ProductosPaginadosVM
            {
                Productos      = productos,
                Buscar         = buscar,
                PaginaActual   = pagina,
                TamanioPagina  = tamanioPagina,
                TotalProductos = total
            };

            return View(vm);
        }

        // GET: Productos/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nombre");

            if (!_context.Categoria.Any())
                TempData["Warning"] = "Debe crear una categoría antes de registrar productos.";

            return View();
        }

        // POST: Productos/Create
        [Authorize(Roles = "Admin")]
        // IFormFile? imagenFile → el archivo que llega desde el <input type="file"> del formulario.
        // El nombre del parámetro debe coincidir exactamente con el name= del input HTML.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Nombre,Precio,Descripcion,CategoriaId")] Producto producto,
            IFormFile? imagenFile)
        {
            if (!_context.Categoria.Any(c => c.Id == producto.CategoriaId))
                ModelState.AddModelError("", "La categoría seleccionada no es válida.");

            if (imagenFile != null && imagenFile.Length > 0)
            {
                var urlImagen = await GuardarImagenAsync(imagenFile);
                if (urlImagen == null)
                    ModelState.AddModelError("", "Solo se permiten imágenes JPG, PNG o WEBP (máx. 5 MB).");
                else
                    producto.ImagenUrl = urlImagen;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(producto);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Producto creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["Error"] = "Error al crear el producto.";
                }
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id de producto inválido.";
                return NotFound();
            }

            var producto = await _context.Producto.FindAsync(id);

            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return NotFound();
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        [Authorize(Roles = "Admin")]
        // Incluimos ImagenUrl en el Bind para que el campo oculto del formulario
        // preserve la URL de la imagen anterior cuando no se sube una nueva.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Nombre,Precio,Descripcion,CategoriaId,ImagenUrl")] Producto producto,
            IFormFile? imagenFile)
        {
            if (id != producto.Id)
            {
                TempData["Error"] = "El producto no coincide.";
                return NotFound();
            }

            if (imagenFile != null && imagenFile.Length > 0)
            {
                // Si el usuario sube una imagen nueva, eliminamos la anterior y guardamos la nueva.
                EliminarImagen(producto.ImagenUrl);
                var urlImagen = await GuardarImagenAsync(imagenFile);
                if (urlImagen == null)
                    ModelState.AddModelError("", "Solo se permiten imágenes JPG, PNG o WEBP (máx. 5 MB).");
                else
                    producto.ImagenUrl = urlImagen;
            }
            // Si no se sube archivo, producto.ImagenUrl conserva el valor del campo oculto del form.

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Producto actualizado correctamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        TempData["Error"] = "Producto no encontrado.";
                        return NotFound();
                    }
                    else
                    {
                        TempData["Error"] = "Error de concurrencia al actualizar.";
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id inválido.";
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado.";
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Producto.FindAsync(id);

            if (producto != null)
            {
                EliminarImagen(producto.ImagenUrl);
                _context.Producto.Remove(producto);
                TempData["Success"] = "Producto eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "Producto no encontrado.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Producto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        private bool ProductoExists(int id) =>
            _context.Producto.Any(e => e.Id == id);

        // Guarda el archivo en wwwroot/images/productos/ y devuelve la URL relativa.
        // Devuelve null si el formato no es válido o el archivo supera 5 MB.
        private async Task<string?> GuardarImagenAsync(IFormFile archivo)
        {
            const long maxBytes = 5 * 1024 * 1024; // 5 MB
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension) || archivo.Length > maxBytes)
                return null;

            // Guid garantiza un nombre único aunque dos usuarios suban la misma foto.
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var carpeta = Path.Combine(_env.WebRootPath, "images", "productos");
            Directory.CreateDirectory(carpeta); // crea la carpeta si no existe

            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);
            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await archivo.CopyToAsync(stream);

            return $"/images/productos/{nombreArchivo}";
        }

        // Elimina el archivo físico de wwwroot dado su URL relativa.
        private void EliminarImagen(string? imagenUrl)
        {
            if (string.IsNullOrEmpty(imagenUrl)) return;
            var rutaFisica = Path.Combine(_env.WebRootPath, imagenUrl.TrimStart('/'));
            if (System.IO.File.Exists(rutaFisica))
                System.IO.File.Delete(rutaFisica);
        }
    }
}
