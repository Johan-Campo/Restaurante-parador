# Estadero Parador Turístico — Sistema de Gestión

Aplicación web de administración para un restaurante/estadero construida con ASP.NET Core MVC. Permite gestionar sucursales, categorías, productos y pedidos, con control de acceso por roles, despliegue automático en Azure y base de datos en la nube.

---

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core MVC |
| ORM | Entity Framework Core |
| Base de datos | SQL Server (LocalDB en dev · Azure SQL en prod) |
| Autenticación | ASP.NET Core Identity |
| Frontend | Bootstrap 5 · Font Awesome 6 · Toastr.js · jQuery |
| Tipografía | Playfair Display · Inter · DM Mono (Google Fonts) |
| Deploy | Azure App Service |
| CI/CD | GitHub Actions |
| Cultura | es-CO (separador decimal con coma) |

---

## Funcionalidades

### Autenticación y registro
- Registro con nombre, apellido, correo y contraseña
- Indicador de fortaleza de contraseña en tiempo real
- Inicio de sesión con opción "mantener sesión iniciada"
- Gestión de cuenta: cambio de contraseña, correo y datos de perfil
- Toda la aplicación requiere autenticación (excepto login y registro)
- Notificaciones toast en login, registro y errores de formulario

### Control de acceso por roles
- Dos roles: **Admin** y **Mesero**
- El rol **Admin** tiene acceso total: sucursales, categorías, productos, pedidos y gestión de usuarios
- El rol **Mesero** solo accede a productos y pedidos
- El sidebar muestra opciones según el rol del usuario autenticado
- Usuario administrador creado automáticamente al iniciar la app (`admin@parador.com`)
- Los nuevos registros se asignan al rol Mesero por defecto

### Gestión de Usuarios *(solo Admin)*
- Lista de todos los usuarios registrados con su rol actual
- Edición de rol por usuario (Admin / Mesero)
- Protección del administrador principal — no se puede cambiar su rol

### Gestión de Sucursales *(solo Admin)*
- CRUD completo (crear, ver, editar, eliminar)
- Validaciones de nombre y dirección

### Gestión de Categorías *(solo Admin)*
- CRUD completo
- Cada categoría pertenece a una sucursal (relación uno a muchos)
- Cascade delete: eliminar sucursal elimina sus categorías

### Gestión de Productos
- CRUD completo con validaciones
- **Subida de imágenes** — archivos guardados en `wwwroot/images/productos/`
  - Formatos aceptados: JPG, PNG, WEBP (máx. 5 MB)
  - Preview en vivo antes de guardar
  - Al editar se puede reemplazar o conservar la imagen actual
  - Al eliminar un producto su imagen se borra del servidor
- Precio decimal con cultura `es-CO` (ej: `12.500,00`)
- Descripción opcional (máx. 200 caracteres)
- **Paginación** — 5 productos por página con navegación numerada
- **Búsqueda** — filtro por nombre o categoría con parámetros en URL

### Módulo de Pedidos
- Crear pedidos seleccionando sucursal, mesa, productos y cantidades
- Productos agrupados por categoría en el formulario
- Resumen en tiempo real con total calculado antes de confirmar
- **Estados con flujo progresivo:** `Pendiente → En preparación → Listo → Entregado`
- Precio histórico por ítem — cambios futuros de precio no afectan pedidos anteriores
- Vista de detalle con tabla de ítems, subtotales y total
- Eliminación de pedidos con confirmación

### Dashboard (página de inicio)
- Dropdowns anidados con carga dinámica vía AJAX:
  - Selecciona sucursal → carga categorías
  - Selecciona categoría → carga productos
- Muestra precio y descripción del producto seleccionado
- Accesos rápidos a los módulos principales
- Toast de bienvenida personalizado con el nombre del usuario (una sola vez por sesión)

---

## Roles y permisos

| Funcionalidad | Admin | Mesero |
|--------------|:-----:|:------:|
| Dashboard | ✅ | ✅ |
| Productos | ✅ | ✅ |
| Pedidos | ✅ | ✅ |
| Sucursales | ✅ | ❌ |
| Categorías | ✅ | ❌ |
| Gestión de usuarios | ✅ | ❌ |

---

## Estructura del proyecto

```
├── Controllers/
│   ├── HomeController.cs           # Dashboard + endpoints AJAX
│   ├── SucursalesController.cs     # CRUD sucursales
│   ├── CategoriasController.cs     # CRUD categorías
│   ├── ProductosController.cs      # CRUD productos + imágenes
│   ├── PedidosController.cs        # Módulo de pedidos
│   └── UsuariosController.cs       # Gestión de roles
├── Models/
│   ├── ApplicationUser.cs          # Usuario extendido (Nombre, Apellido)
│   ├── Sucursal.cs
│   ├── Categoria.cs
│   ├── Producto.cs
│   ├── Pedido.cs
│   ├── PedidoProducto.cs
│   ├── EstadoPedido.cs
│   ├── DropDownsVM.cs
│   ├── CrearPedidoVM.cs
│   ├── ProductosPaginadosVM.cs
│   └── UsuarioRolVM.cs
├── Datos/
│   ├── ApplicationDbContext.cs
│   └── DatabaseInitializer.cs      # Servicio en segundo plano: migraciones + seed
├── Areas/Identity/Pages/Account/   # Login, Register, Manage (scaffolded + personalizado)
├── Views/
│   ├── Home/
│   ├── Sucursales/
│   ├── Categorias/
│   ├── Productos/
│   ├── Pedidos/
│   ├── Usuarios/
│   └── Shared/
│       ├── _Layout.cshtml          # Sidebar con roles
│       ├── _LayoutAuth.cshtml      # Layout para login/registro
│       └── _Notification.cshtml    # Toasts globales
├── Migrations/                     # Historial EF Core
├── .github/workflows/              # Pipeline CI/CD GitHub Actions
├── appsettings.json                # Config local (LocalDB)
└── wwwroot/
    ├── css/
    ├── images/productos/
    └── lib/
```

---

## Modelo de datos

```
Sucursal (1) ──→ (N) Categoria (1) ──→ (N) Producto
                                              │
Pedido (1) ─────────────────────────→ (N) PedidoProducto
```

- Cascade delete: eliminar sucursal elimina categorías y productos asociados
- `PedidoProducto` guarda el precio al momento de crear el pedido (precio histórico)

---

## CI/CD con GitHub Actions y Azure

Cada `push` a la rama `master` dispara automáticamente el pipeline:

1. **Build** — `dotnet build --configuration Release`
2. **Publish** — `dotnet publish -c Release`
3. **Deploy** — sube el artefacto a Azure App Service (`estadero-Parador`) vía `azure/webapps-deploy`

La autenticación con Azure usa OIDC (sin secretos de larga duración):
- `AZUREAPPSERVICE_CLIENTID`
- `AZUREAPPSERVICE_TENANTID`
- `AZUREAPPSERVICE_SUBSCRIPTIONID`

El archivo del workflow está en `.github/workflows/master_estadero-parador.yml`.

---

## Infraestructura en Azure

| Recurso | Nombre |
|---------|--------|
| App Service | `estadero-Parador` |
| SQL Server | `estadero-sql-server2.database.windows.net` |
| Base de datos | `EstaderoDb2` (plan Free serverless) |

La cadena de conexión se configura como **variable de entorno** en Azure App Service (Configuración → Cadenas de conexión → `DefaultConnection`), sobreescribiendo el `appsettings.json` local.

Las migraciones y el seed de roles/admin se ejecutan automáticamente al iniciar la app mediante `DatabaseInitializer` (servicio en segundo plano con reintentos), sin bloquear el arranque del servidor.

---

## Configuración local

### Requisitos
- SDK de la plataforma instalado
- SQL Server LocalDB (incluido en Visual Studio)

### Pasos

```bash
git clone https://github.com/Johan-Campo/Restaurante-parador.git
cd "Restaurante Parador"
dotnet run
```

Las migraciones se aplican automáticamente al iniciar. La base de datos local se crea en `(localdb)\mssqllocaldb` con el nombre `DropDownDemo1`.

### Credenciales del administrador (local y producción)

```
Email:      admin@parador.com
Contraseña: Admin123!
```

---

## Autor

**Johan Campo** · portafolio personal
