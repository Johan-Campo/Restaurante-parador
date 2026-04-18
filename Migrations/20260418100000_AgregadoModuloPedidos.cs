using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropDownsAnidadosMvc.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoModuloPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tabla Pedido
            // Estado se guarda como int: 0=Pendiente, 1=EnPreparacion, 2=Listo, 3=Entregado
            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id             = table.Column<int>(type: "int", nullable: false)
                                         .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCreacion  = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado         = table.Column<int>(type: "int", nullable: false),
                    NombreCliente  = table.Column<string>(type: "nvarchar(80)",  maxLength: 80,  nullable: true),
                    NumeroMesa     = table.Column<string>(type: "nvarchar(10)",  maxLength: 10,  nullable: true),
                    Observaciones  = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Id);
                });

            // Tabla PedidoProducto (tabla de unión con datos extra: Cantidad y PrecioUnitario)
            migrationBuilder.CreateTable(
                name: "PedidoProducto",
                columns: table => new
                {
                    Id              = table.Column<int>(type: "int", nullable: false)
                                          .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId        = table.Column<int>(type: "int", nullable: false),
                    ProductoId      = table.Column<int>(type: "int", nullable: false),
                    Cantidad        = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario  = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoProducto", x => x.Id);

                    // Si se borra un Pedido, sus ítems se borran en cascada.
                    table.ForeignKey(
                        name: "FK_PedidoProducto_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    // Restrict: no se puede borrar un Producto si tiene pedidos asociados.
                    table.ForeignKey(
                        name: "FK_PedidoProducto_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoProducto_PedidoId",
                table: "PedidoProducto",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoProducto_ProductoId",
                table: "PedidoProducto",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PedidoProducto");
            migrationBuilder.DropTable(name: "Pedido");
        }
    }
}
