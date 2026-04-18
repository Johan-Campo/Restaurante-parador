# Estadero Parador Turístico — Sistema de Gestión

Aplicación web de administración para un restaurante/estadero, construida con ASP.NET Core 8 MVC. Permite gestionar sucursales, categorías y productos con autenticación de usuarios.

---

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8.0 MVC |
| ORM | Entity Framework Core 8 |
| Base de datos | SQL Server (LocalDB en dev / Azure SQL en prod) |
| Autenticación | ASP.NET Core Identity |
| Frontend | Bootstrap 5 · Font Awesome 6 · Toastr.js |
| Tipografía | Google Fonts — DM Sans · DM Mono · Fraunces |
| Deploy | Azure App Service |

---

## Funcionalidades principales

### Autenticación
- Registro e inicio de sesión con ASP.NET Identity
- Gestión de cuenta (cambio de contraseña, email, 2FA)
- Toda la aplicación requiere autenticación (excepto login/registro)

### Gestión de Sucursales
- CRUD completo (crear, leer, actualizar, eliminar)
- Validaciones de longitud en nombre y dirección

### Gestión de Categorías
- CRUD completo
- Cada categoría pertenece a una sucursal (relación uno a muchos)

### Gestión de Productos
- CRUD completo con validaciones
- **Subida de imágenes** — archivo guardado en `wwwroot/images/productos/`
  - Formatos aceptados: JPG, PNG, WEBP (máx. 5 MB)
  - Preview en vivo antes de guardar
  - Al editar, se puede reemplazar la imagen o conservar la actual
  - Al eliminar un producto, su imagen se borra del servidor
- Descripción opcional (máx. 200 caracteres)
- Precio con precisión decimal (18,2)

### Dropdowns anidados (página de inicio)
- Selecciona una sucursal → carga sus categorías via AJAX
- Selecciona una categoría → carga sus productos via AJAX
- Muestra precio y descripción del producto seleccionado
- Implementado con jQuery + endpoints JSON en `HomeController`

---

## Estructura del proyecto

```
├── Controllers/
│   ├── HomeController.cs         # Dashboard + endpoints AJAX
│   ├── SucursalesController.cs   # CRUD sucursales
│   ├── CategoriasController.cs   # CRUD categorías
│   └── ProductosController.cs    # CRUD productos + subida de imágenes
├── Models/
│   ├── Sucursal.cs
│   ├── Categoria.cs
│   ├── Producto.cs               # Incluye ImagenUrl
│   └── DropDownsVM.cs
├── Datos/
│   └── ApplicationDbContext.cs   # DbContext con seed data
├── Views/
│   ├── Home/
│   ├── Sucursales/
│   ├── Categorias/
│   ├── Productos/
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
```

Cascade delete: al borrar una sucursal, se eliminan sus categorías y productos.

---

## Configuración local

### Requisitos
- .NET 8 SDK
- SQL Server LocalDB (incluido en Visual Studio)

### Pasos
```bash
# Clonar el repositorio
git clone <url>
cd "Restaurante Parador"

# Restaurar dependencias y ejecutar
dotnet run
```

Las migraciones se aplican automáticamente al iniciar la aplicación (`MigrateAsync()` en `Program.cs`).

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
- **Base de datos:** Azure SQL (cadena de conexión en variables de entorno de Azure)
- La carpeta `wwwroot/images/productos/` es efímera en Azure App Service — para producción real se recomienda migrar el almacenamiento de imágenes a **Azure Blob Storage**

---

## Autor

**Johan Campo** · Proyecto de portafolio personal  
Desarrollado para aprender ASP.NET Core MVC de forma progresiva.
