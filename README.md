# Bodega Backend

Este repositorio contiene el backend de la aplicación para la gestión de Bodega Central. Está desarrollado con .NET y C#, y permite realizar operaciones relacionadas con la autenticación de usuarios(solo se tiene 2 Roles: Administrador de bodega y operador), muestra listado de sucursales activas, obtener producto por codigo y tambien por listado de productos activos, brinda la disponibilidad de lotes de productos activos ordenados por fecha de vencimiento y por ultimo ingreso de lotes de productos para salida de inventario, recepción de producto por codigo de Salida y paginación de las ordenes de salidas que se puede filtrar por sucursal o rango de fechas.

## Características
- API RESTful para la gestión de Bodega en ordenes de salida para inventario.
- Conexión a base de datos MS Sql Server.
- Arquitectura en capas.
- Patrones de arquitectura: MVC y CQRS.

## Requisitos previos
- .NET SDK 8.0 o superior instalado.
- Base de datos MS Sql Server.
- Git para clonar el repositorio.

## Instalación y configuración

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/KEV4142/Bodega_Backend.git
   cd Bodega_Backend
   ```

2. **Configurar las variables de entorno:**
   Crear un archivo `.env` en el directorio raíz del proyecto y agregar las siguientes variables:
   ```env
   DB_CONNECTION= [Data Source=<NombreServidor>;Initial Catalog=Bodega; User Id=<NombreUsuario>;Password=<Contraseña>;Encrypt=False;]
   TOKEN_KEY= [Campo tipo string aleatorio]
   FRONTEND_ORIGIN=*
   BACKEND_ORIGIN=*
   PORT=5000
   ```

   > **Nota:** Cambia estas variables de entorno según tus necesidades y evita compartir credenciales sensibles en repositorios públicos. Adicional las ultimas 2 variables es para habilitar la funcion CORS y qué encabezados de host (Host) son permitidos al realizar solicitudes al servidor(* para aceptar todas las solicitudes). La variable PORT dependera del servicio donde se despliegue.

3. **Restaurar las dependencias:**
   Ejecuta el siguiente comando para restaurar los paquetes necesarios:
   ```bash
   dotnet restore
   ```

4. **Ejecutar las migraciones:**
   Asegúrate de que la conexión a la base de datos esté configurada correctamente y ejecuta:
   ```bash
   cd Repositorio
   dotnet ef database update
   ```
   > **Nota:** Tambien esta el repositorio https://github.com/KEV4142/Bodega_BD.git con el scritp de la base de datos(Bodega).
   Tambien descomentando la Linea del program/WebApi (//await app.SeedDataAuthentication();) realiza las migraciones pendientes e inserta los datos para algunas pruebas de ejecucion.

5. **Iniciar el servidor:**
   Inicia el backend localmente:
   ```bash
   cd WebApi
   dotnet run
   ```

   El servidor estará disponible en `http://localhost:5000` por defecto (o `https://localhost:5001` para HTTPS).

## Uso
- Usa herramientas como [Postman](https://www.postman.com/) para probar los endpoints de la API.
- Integra el backend con el frontend especificando el origen permitido en `FRONTEND_ORIGIN`.
- La documentación de la API con Swagger está disponible automáticamente en el entorno de desarrollo accediendo a http://localhost:5000/swagger .
> **Nota:** Swagger está habilitado cuando el entorno (ASPNETCORE_ENVIRONMENT) está configurado como Development.

## Despliegue
Puedes desplegar este proyecto en cualquier servicio compatible, como Azure App Service, AWS, o Heroku. Recuerda configurar las variables de entorno necesarias en tu plataforma de despliegue.

## Tecnologías utilizadas
- **.NET 8**: Framework principal para el backend.
- **MS Sql Server**: Base de datos relacional.
- **JWT**: JWT para manejo de seguridad.


## Licencia
Este proyecto está bajo la Licencia MIT. Consulta el archivo `LICENSE` para más detalles.
