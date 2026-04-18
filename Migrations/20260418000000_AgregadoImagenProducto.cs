using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropDownsAnidadosMvc.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoImagenProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agrega la columna ImagenUrl a la tabla Producto.
            // nullable: true → los productos existentes quedan con NULL (sin imagen).
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Producto",
                type: "nvarchar(max)",
                nullable: true);

            // Actualiza los productos sembrados (seed data) para que tengan NULL en ImagenUrl.
            for (int i = 1; i <= 10; i++)
            {
                migrationBuilder.UpdateData(
                    table: "Producto",
                    keyColumn: "Id",
                    keyValue: i,
                    column: "ImagenUrl",
                    value: null);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Si se revierte la migración, elimina la columna.
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Producto");
        }
    }
}
