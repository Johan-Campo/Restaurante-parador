# Estadero Parador Turístico — Sistema de Gestión

Aplicación web de administración para un restaurante/estadero, construida con ASP.NET Core 8 MVC. Permite gestionar sucursales, categorías, productos y pedidos con autenticación y roles de usuario.

---

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8.0 MVC |
| ORM | Entity Framework Core 8 |
| Base de datos | SQL Server (LocalDB en dev / Azure SQL en prod) |
| Autenticación | ASP.NET Core Identity |
| Frontend | Bootstrap 5 · Font Awesome 6 · Toastr.js |
| Tipografía | Playfair Display · Inter · DM Mono (Google Fonts) |
| Deploy | Azure App Service + GitHub Actions (OIDC) |

---

## Funcionalidades principales

### Autenticación y roles
- Registro e inicio de sesión con ASP.NET Core Identity
- Dos roles: **Admin** (acceso completo) y **Mesero** (solo pedidos + ver productos)
- Seed automático al iniciar: crea roles y usuario `admin@parador.com / Admin123!`
- Gestión de roles desde el panel de administración (solo Admin)
- Los nuevos registros reciben el rol Mesero automáticamente

### Gestión de Sucursales *(solo Admin)*
- CRUD completo (crear, leer, actualizar, eliminar)
- Validaciones de longitud en nombre y dirección

### Gestión de Categorías *(solo Admin)*
- CRUD completo
- Cada categoría pertenece a una sucursal (relación uno a muchos)

### Gestión de Productos
- CRUD completo (Admin) · Solo lectura (Mesero)
- **Subida de imágenes** — archivo guardado en `wwwroot/images/productos/`
  - Formatos aceptados: JPG, PNG, WEBP (máx. 5 MB)
  - Preview en vivo antes de guardar
  - Al editar, se puede reemplazar la imagen o conservar la actual
  - Al eliminar un producto, su imagen se borra del servidor
- Descripción opcional (máx. 200 caracteres)
- Precio con precisión decimal (18,2)
- **Paginación** — 5 productos por página con navegación numerada
- **Búsqueda** — filtro por nombre o categoría (parámetros en URL)

### Módulo de Pedidos
- Crear pedidos seleccionando productos con cantidades
- Productos agrupados por categoría en el formulario
- Resumen en tiempo real con total calculado
- **Estados** con flujo progresivo: `Pendiente → En preparación → Listo → Entregado`
- Precio histórico por ítem — los cambios futuros de precio no afectan pedidos anteriores
- Vista de detalle con tabla de ítems, subtotales y total
- Eliminación de pedidos con confirmación

### Dropdowns anidados (página de inicio)
- Selecciona una sucursal → carga sus categorías via AJAX
- Selecciona una categoría → carga sus productos via AJAX
- Implementado con jQuery + endpoints JSON en `HomeController`

---

## Estructura del proyecto

```
├── Controllers/
│   ├── HomeController.cs         # Dashboard + endpoints AJAX
│   ├── SucursalesController.cs   # CRUD sucursales [Admin]
│   ├── CategoriasController.cs   # CRUD categorías [Admin]
│   ├── ProductosController.cs    # CRUD productos + imágenes
│   ├── PedidosController.cs      # CRUD pedidos + estados
│   └── UsuariosController.cs     # Gestión de roles [Admin]
├── Models/
│   ├── Sucursal.cs
│   ├── Categoria.cs
│   ├── Producto.cs               # Incluye ImagenUrl, Descripcion
│   ├── Pedido.cs                 # Estado (enum), NombreCliente, NumeroMesa
│   ├── PedidoProducto.cs         # Junction table con PrecioUnitario histórico
│   ├── EstadoPedido.cs           # Enum: Pendiente, EnPreparacion, Listo, Entregado
│   ├── UsuarioRolVM.cs           # ViewModel para gestión de roles
│   ├── CrearPedidoVM.cs          # ViewModel para crear pedidos
│   ├── ProductosPaginadosVM.cs   # ViewModel para paginación
│   └── DropDownsVM.cs
├── Datos/
│   └── ApplicationDbContext.cs   # DbContext con seed de roles/admin
├── Views/
│   ├── Home/
│   ├── Sucursales/
│   ├── Categorias/
│   ├── Productos/
│   ├── Pedidos/
│   ├── Usuarios/
│   └── Shared/                   # Layout, notificaciones
├── Areas/Identity/               # Páginas de autenticación
├── Migrations/                   # Historial de cambios en BD
└── wwwroot/
    ├── css/site.css
    ├── images/productos/         # Imágenes subidas por el usuario
    └── lib/                      # Bootstrap, jQuery
```

---

## Modelo de datos

```
Sucursal (1) ──→ (N) Categoria (1) ──→ (N) Producto
                                              │
                                    PedidoProducto (junction)
                                              │
                                          Pedido
```

Cascade delete: al borrar una sucursal se eliminan sus categorías y productos.  
Restrict delete: no se puede borrar un producto que tenga pedidos registrados.

---

## Configuración local

### Requisitos
- .NET 8 SDK
- SQL Server LocalDB (incluido en Visual Studio)

### Pasos
```bash
git clone <url>
cd "Restaurante Parador"
dotnet run
```

Las migraciones se aplican automáticamente al iniciar (`MigrateAsync()` en `Program.cs`).  
Se crean los roles **Admin** y **Mesero**, y el usuario administrador por defecto.

### Cadena de conexión (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DropDownDemo1;Trusted_Connection=True;"
  }
}
```

---

## Deploy en Azure

- **App Service:** plan gratuito/básico con runtime .NET 8
- **CI/CD:** GitHub Actions con autenticación OIDC — deploy automático en cada push a `master`
- **Base de datos:** Azure SQL (cadena de conexión en variables de entorno de Azure)

---

## Autor

**Johan Campo** · Proyecto de portafolio personal  

