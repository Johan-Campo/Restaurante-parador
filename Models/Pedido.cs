using System.ComponentModel.DataAnnotations;

namespace DropDownsAnidadosMvc.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        // Se asigna automáticamente en el controlador al crear el pedido.
        public DateTime FechaCreacion { get; set; }

        // Estado del pedido: guardado como int en la BD (0=Pendiente, 1=EnPreparacion…)
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

        [StringLength(80, ErrorMessage = "Máximo 80 caracteres")]
        [Display(Name = "Cliente")]
        public string? NombreCliente { get; set; }

        [StringLength(10)]
        [Display(Name = "Mesa")]
        public string? NumeroMesa { get; set; }

        [StringLength(300, ErrorMessage = "Máximo 300 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        // Propiedad de navegación: lista de ítems del pedido.
        // EF Core la usa para cargar los productos relacionados con .Include().
        public ICollection<PedidoProducto> Items { get; set; } = new List<PedidoProducto>();

        // Propiedad calculada: NO se guarda en BD, se calcula sumando subtotales.
        // EF Core ignora propiedades sin setter.
        public decimal Total => Items?.Sum(i => i.Subtotal) ?? 0;
    }
}
