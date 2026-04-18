namespace DropDownsAnidadosMvc.Models
{
    // Un enum es un tipo con valores fijos. En la BD se guarda como int (0, 1, 2, 3).
    // Esto evita guardar strings como "Pendiente" que son propensos a errores tipográficos.
    public enum EstadoPedido
    {
        Pendiente     = 0,
        EnPreparacion = 1,
        Listo         = 2,
        Entregado     = 3
    }
}
