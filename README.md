# 🏦 MiniWebBank

¡Bienvenido a **MiniWebBank**! Este es un proyecto **Full-Stack** construido desde cero, diseñado para formar parte de mi portafolio profesional como desarrollador de software.

Este proyecto demuestra mi capacidad para construir sistemas completos abarcando desde el diseño de la interfaz de usuario hasta la arquitectura de bases de datos y seguridad en el lado del servidor.

---

## 🚀 Tecnologías Utilizadas

### Backend (Servidor)
* **Lenguaje:** C# (.NET 8)
* **Framework:** ASP.NET Core (Minimal APIs)
* **Base de Datos:** SQLite
* **ORM:** Entity Framework Core
* **Seguridad:** Autenticación por Cookies (Cookie Authentication)

### Frontend (Cliente)
* **Estructura:** HTML5 Semántico
* **Estilos:** Vanilla CSS (Diseño moderno tipo *Glassmorphism* y Modo Oscuro)
* **Lógica:** Vanilla JavaScript (ES6+ usando `fetch` y asincronismo)

---

## ✨ Características Principales

1. **Gestión de Cuentas:** Capacidad de crear nuevas cuentas bancarias asociadas a un número de cuenta, titular y un PIN de seguridad.
2. **Autenticación (Login):** Sistema de inicio de sesión seguro usando Cookies en el navegador. Las operaciones financieras están bloqueadas (`[Authorize]`) hasta que el usuario se autentica exitosamente.
3. **Operaciones Financieras:** Depósitos y retiros de dinero validados mediante lógica de negocio (ej. prevención de retiros mayores al saldo disponible o montos negativos).
4. **Interfaz Dinámica (SPA):** La página no necesita recargarse. Todo se comunica mediante peticiones HTTP (REST) de fondo, actualizando el DOM en tiempo real.

---

## 🛠️ Cómo ejecutar este proyecto localmente

Si deseas probar este proyecto en tu propia computadora, sigue estos pasos:

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/TU_USUARIO/MiniWebBank.git
   ```
2. **Entrar a la carpeta del proyecto:**
   ```bash
   cd MiniWebBank
   ```
3. **Ejecutar el servidor de .NET:**
   ```bash
   dotnet run
   ```
4. **Abrir la aplicación:**
   Abre tu navegador de internet favorito y navega hacia `http://localhost:5025/index.html` (o el puerto que se asigne automáticamente).

---

> *"Un viaje de mil millas comienza con un solo paso."*  
> Este proyecto representa mi paso sólido hacia la construcción de aplicaciones empresariales modernas.
