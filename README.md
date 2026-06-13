# Trabajo Práctico: E-Commerce (Grupo 6)

## Arquitectura de Microservicios 🛒

Este proyecto está construido utilizando una arquitectura de microservicios en .NET 9, comunicados de manera asíncrona y orquestados por un API Gateway (YARP).

### Microservicios Integrados:
- **ECommerce.Gateway** (Puerto: 5000 / 7000)
- **Products.API** (Puerto: 5001 / 7001)
- **User.API** (Puerto: 5002 / 7002)
- **Cart.API** (Puerto: 5003 / 7003)
- **Order.API** (Puerto: 5004 / 7004)
- **Notifications.API** (Puerto: 5005 / 7005)

---

## Pruebas y Manejo de Errores 🚨

A continuación, documentamos las pruebas de los distintos errores y validaciones de negocio manejadas por los microservicios:

### 1. Error 409 (Conflict) - Producto Duplicado
*(Espacio para subir la captura de pantalla de Santi sobre el error 409)*

### 2. Error 404 (Not Found) - Recurso Inexistente
*(Espacio para subir la captura de pantalla)*

### 3. Error 422 (Unprocessable Entity) - Validaciones
*(Espacio para subir la captura de pantalla)*

### 4. Error 401/403 (Unauthorized / Forbidden)
*(Espacio para subir la captura de pantalla)*

---
**Nota para el profesor:** El proyecto no utiliza expresiones lambda (`=>`) ni LINQ en cumplimiento estricto con los requerimientos de la cátedra.
