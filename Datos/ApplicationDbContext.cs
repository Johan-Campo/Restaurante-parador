    using DropDownsAnidadosMvc.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;


    namespace DropDownsAnidadosMvc.Datos
    {
        public class ApplicationDbContext : IdentityDbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            //Agregar los modelos
            public DbSet<Sucursal> Sucursal { get; set; }
            public DbSet<Categoria> Categoria { get; set; }
            public DbSet<Producto> Producto { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Sucursal>().HasData(
                    new Sucursal { Id = 1, Nombre = "Sucursal Principal", Direccion = "123 Calle Principal" },
                    new Sucursal { Id = 2, Nombre = "Sucursal Central", Direccion = "456 Calle Central" },
                    new Sucursal { Id = 3, Nombre = "Sucursal Norte", Direccion = "789 Calle Norte" }
                );

                modelBuilder.Entity<Categoria>().HasData(
                    new Categoria { Id = 1, Nombre = "Aperitivos", SucursalId = 1 },
                    new Categoria { Id = 2, Nombre = "Plato Principal", SucursalId = 1 },
                    new Categoria { Id = 3, Nombre = "Postres", SucursalId = 2 },
                    new Categoria { Id = 4, Nombre = "Bebidas", SucursalId = 2 },
                    new Categoria { Id = 5, Nombre = "Especialidades", SucursalId = 3 }
                );
                modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Producto>().HasData(
                   new Producto { Id = 1, Nombre = "Rollitos de Primavera", Precio = 4.99M, CategoriaId = 1 },
                   new Producto { Id = 2, Nombre = "Hamburguesa Vegana", Precio = 9.99M, CategoriaId = 2 },
                   new Producto { Id = 3, Nombre = "Tarta de Chocolate", Precio = 7.99M, CategoriaId = 3 },
                   new Producto { Id = 4, Nombre = "Refresco de Frutas", Precio = 2.99M, CategoriaId = 4 },
                   new Producto { Id = 5, Nombre = "Plato Especial", Precio = 12.99M, CategoriaId = 5 },
                   new Producto { Id = 6, Nombre = "Ensalada Mediterránea", Precio = 8.99M, CategoriaId = 2 },
                   new Producto { Id = 7, Nombre = "Pastel de Queso", Precio = 6.99M, CategoriaId = 3 },
                   new Producto { Id = 8, Nombre = "Café Espresso", Precio = 3.99M, CategoriaId = 4 },
                   new Producto { Id = 9, Nombre = "Pizza Margarita", Precio = 10.99M, CategoriaId = 5 },
                   new Producto { Id = 10, Nombre = "Sopa de Tomate", Precio = 5.99M, CategoriaId = 2 }
                   );
            }
        }
    }
