# 🏦 MiniWebBank (Full-Stack Banking App)

Una aplicación bancaria de nivel de producción construida con **C# ASP.NET Core** y **JavaScript Vanilla**, diseñada como proyecto destacado para mi portafolio profesional.

## ✨ Características Técnicas (Arquitectura)

Este proyecto no es solo un CRUD básico; simula la arquitectura de un banco real implementando estándares de la industria:

* **Arquitectura de Base de Datos en la Nube:** Migración exitosa de SQLite local a un clúster de **PostgreSQL Serverless alojado en AWS (Neon)** usando Entity Framework Core.
* **Control de Acceso Basado en Roles (RBAC):** Implementación de seguridad estricta con tres niveles de autorización:
  * `Admin`: Acceso total al CRM para auditar y crear cuentas.
  * `Cajero`: Permisos exclusivos para realizar depósitos y retiros físicos en ventanilla.
  * `Client`: Acceso al Home Banking privado, con rutas protegidas para evitar acceso a fondos de terceros.
* **Autenticación Segura:** Sistema de Login validado por **Cookies de Sesión Cifradas (ASP.NET Claims Identity)**, abandonando validaciones inseguras del lado del cliente.
* **Transacciones Atómicas (ACID):** Lógica de transferencias monetarias respaldada por `IDbContextTransaction`, garantizando que en caso de fallos de red, el dinero jamás se pierda en el limbo (Rollback automático).
* **Frontend Desacoplado:** Arquitectura "Single Responsibility" separando el Portal de Login (`index.html`), el CRM Administrativo (`admin.html`), la Ventanilla (`cajero.html`) y el Home Banking (`dashboard.html`).
* **UI/UX Moderna:** Diseño implementando *Glassmorphism* (efecto cristal) con CSS puro, sin depender de librerías externas.

## 🛠️ Tecnologías Usadas
- **Backend:** C# 11, .NET 8, ASP.NET Core Minimal APIs
- **ORM:** Entity Framework Core
- **Base de Datos:** PostgreSQL (Neon Tech / AWS)
- **Frontend:** HTML5, CSS3, JavaScript (Fetch API)

## 🚀 Cómo Ejecutarlo Localmente
1. Clona este repositorio.
2. Asegúrate de tener el SDK de .NET instalado.
3. Ejecuta en la terminal:
   ```bash
   dotnet run
   ```
4. Abre `http://localhost:5025/index.html` en tu navegador.
*(Nota: Requiere configurar tus propios User Secrets de .NET para la cadena de conexión de PostgreSQL).*
