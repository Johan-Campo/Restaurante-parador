namespace DropDownsAnidadosMvc.Models
{
    // Tabla de unión para la relación muchos-a-muchos entre Pedido y Producto.
    //
    // ¿Por qué no usar directamente la relación many-to-many implícita de EF?
    // Porque necesitamos guardar DATOS EXTRA en la relación: Cantidad y PrecioUnitario.
    // Una tabla de unión simple no puede almacenar esos datos — necesitamos una entidad propia.
    public class PedidoProducto
    {
        public int Id { get; set; }

        // FK hacia Pedido
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        // FK hacia Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

        // Guardamos el precio al momento del pedido.
        // Si el precio del producto cambia mañana, este registro queda intacto.
        public decimal PrecioUnitario { get; set; }

        // Propiedad calculada: no va a la BD
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
