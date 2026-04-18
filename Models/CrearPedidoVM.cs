using System.ComponentModel.DataAnnotations;

namespace DropDownsAnidadosMvc.Models
{
    // ViewModel para el formulario de creación de pedidos.
    // Agrupa los datos del pedido + la lista de productos seleccionables.
    public class CrearPedidoVM
    {
        [StringLength(80)]
        [Display(Name = "Cliente")]
        public string? NombreCliente { get; set; }

        [StringLength(10)]
        [Display(Name = "Mesa")]
        public string? NumeroMesa { get; set; }

        [StringLength(300)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        // Lista de todos los productos disponibles, cada uno con su campo de cantidad.
        // ASP.NET MVC enlaza automáticamente Items[0].Cantidad, Items[1].Cantidad, etc.
        // desde el formulario HTML gracias al model binding por índice.
        public List<ItemSeleccionVM> Items { get; set; } = new();
    }

    // Representa un producto en el formulario de selección.
    public class ItemSeleccionVM
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = "";
        public string NombreCategoria { get; set; } = "";
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }

        // Cantidad que el usuario escribe. 0 = no incluir en el pedido.
        [Range(0, 99, ErrorMessage = "Entre 0 y 99")]
        public int Cantidad { get; set; } = 0;
    }
}
