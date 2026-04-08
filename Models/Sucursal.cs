using System.ComponentModel.DataAnnotations;

namespace DropDownsAnidadosMvc.Models
{
    public class Sucursal
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="El nombre es obligatorio")]
        [StringLength(20, MinimumLength = 2,
        ErrorMessage = "Debe tener entre 2 y 20 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La direccion es obligatoria")]
        [StringLength(20, MinimumLength = 2,
        ErrorMessage = "Debe tener entre 2 y 20 caracteres")]
        public string Direccion { get; set;}

        // Relación uno a muchos con Categoria
        public ICollection<Categoria> Categorias { get; set; }
    }
}
