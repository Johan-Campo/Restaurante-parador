using System.ComponentModel.DataAnnotations;

namespace DropDownsAnidadosMvc.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string Nombre { get; set; }

        // Foreign Key
        [Display(Name = "Sucursal")]
        [Required(ErrorMessage = "Debe seleccionar una sucursal")]
        public int SucursalId { get; set; }

        // Propiedad de navegación
        public Sucursal Sucursal { get; set; }

        // Relación uno a muchos con Producto
        public ICollection<Producto> Productos { get; set; }

    }
}
