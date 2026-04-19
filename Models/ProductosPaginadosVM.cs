namespace DropDownsAnidadosMvc.Models
{
    public class ProductosPaginadosVM
    {
        // Solo los productos de la página actual (ej: 5 de 23)
        public List<Producto> Productos { get; set; } = new();

        // Texto que el usuario escribió en el buscador
        public string Buscar { get; set; }

        // Página que se está viendo ahora (empieza en 1)
        public int PaginaActual { get; set; }

        // Cuántos productos caben por página
        public int TamanioPagina { get; set; }

        // Total de productos que coinciden con la búsqueda
        public int TotalProductos { get; set; }

        // Calculado: cuántas páginas existen en total
        // Math.Ceiling redondea hacia arriba: 23 productos / 5 por página = 4.6 → 5 páginas
        public int TotalPaginas => (int)Math.Ceiling((double)TotalProductos / TamanioPagina);

        // Helpers para saber si hay página anterior o siguiente (usados en la vista)
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    }
}
