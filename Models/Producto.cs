using System.ComponentModel.DataAnnotations;

namespace DropDownsAnidadosMvc.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 2,
        ErrorMessage = "Debe tener entre 2 y 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(typeof(decimal), "0,01", "1000000", ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Precio { get; set; }


        [StringLength(200,
        ErrorMessage = "Máximo 200 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        public Categoria Categoria { get; set; }

        public string? ImagenUrl { get; set; }
    }
}