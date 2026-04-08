using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var productos = _context.Producto.Include(p => p.Categoria);

            if (!productos.Any())
            {
                TempData["Info"] = "No existen productos registrados.";
            }

            return View(await productos.ToListAsync());
        }


        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nombre");

            if (!_context.Categoria.Any())
            {
                TempData["Warning"] = "Debe crear una categoría antes de registrar productos.";
            }

            return View();
        }


        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Precio,CategoriaId")] Producto producto)
        {
            if (!_context.Categoria.Any(c => c.Id == producto.CategoriaId))
            {
                ModelState.AddModelError("", "La categoría seleccionada no es válida.");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Precio,CategoriaId")] Producto producto)
        {
            if (id != producto.Id)
            {
                TempData["Error"] = "El producto no coincide.";
                return NotFound();
            }

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Producto.FindAsync(id);

            if (producto != null)
            {
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


        private bool ProductoExists(int id)
        {
            return _context.Producto.Any(e => e.Id == id);
        }
    }
}