# Sistema de E-Commerce - Microservicios (Grupo 6)

Trabajo Práctico desarrollado para Construcción de Aplicaciones Informáticas en el primer cuatrimestre de 2026. 
E-Commerce con una arquitectura orientada a microservicios utilizando .NET 9, SQLite para una DB independiente por servicio, logging, trazabilidad de peticiones, Health Checks, y un API gateway. 
Integrado con Swagger. Ofrece un manejo estandarizado de excepciones detalladas en las consignas del trabajo práctico y ejemplificadas en el mismo README.

---

## Integrantes
* **Leandro Salzberg**: Order.API, Cart.API
* **Santiago Ubeid**: User.API, Notifications.API
* **Mariano Fioretti**: ECommerce.Gateway, ECommerce.Shared, Products.API.

---

## Requisitos Previos y Configuración
Herramientas:
* SDK de .NET 9.0 o superior
* Visual Studio 2022 o VS Code
* Herramienta o Visor de SQLite (opcional, para auditar los archivos `.db`)

### Configuración del Inicio Múltiple en Visual Studio
**Para ejecutar el flujo completo del backend en paralelo**

**Para empezar, hay que clonar el repositorio utilizando git clone https://github.com/s6ntiu/e-CommerceG6"**

En la barra de tareas:

<img width="1225" height="66" alt="image" src="https://github.com/user-attachments/assets/5965fdda-b69b-4c7c-8908-1f79975a446b" />

Desplegamos el menú entre el botón verde Start y el TestNotiplusUser y seleccionamos en configurar Startup Projects

<img width="213" height="234" alt="image" src="https://github.com/user-attachments/assets/adfb9a36-bbc3-4677-8fac-2a66b2dc1519" />

Una vez en el menú creamos uno nuevo y seleccionamos Start en todas menos ECommerce.shared

<img width="799" height="541" alt="image" src="https://github.com/user-attachments/assets/d385c14a-c980-48b4-9fa3-78b2e3951e64" />

**Una vez tenemos este perfil, lo guardamos tocando aplicar y lo iniciamos utilizando el boton de Start en la barra de herramientas, o utilizamos el siguiente comando en una terminal de powershell**

**dotnet run --launch-profile "Nombre_Perfil"**

---

### Endpoints Principales y Puertos Locales

| Servicio | Swagger UI | Health Check |
| :--- | :--- | :--- |
| **Users API** | [http://localhost:5002/swagger](http://localhost:5002/swagger) | [http://localhost:5002/health](http://localhost:5002/health) |
| **Products API** | [http://localhost:5001/swagger](http://localhost:5001/swagger) | [http://localhost:5001/health](http://localhost:5001/health) |
| **Cart API** | [http://localhost:5003/swagger](http://localhost:5003/swagger) | [http://localhost:5003/health](http://localhost:5003/health) |
| **Orders API** | [http://localhost:5004/swagger](http://localhost:5004/swagger) | [http://localhost:5004/health](http://localhost:5004/health) |
| **Notifications API** | [http://localhost:5005/swagger](http://localhost:5005/swagger) | [http://localhost:5005/health](http://localhost:5005/health) |

---
## Diagrama de arquitectura del proyecto
```mermaid 
graph TD
    %% Estilos de los bloques con alto contraste
    classDef usuario fill:#e1f5fe,stroke:#0288d1,stroke-width:2px,color:#000000;
    classDef entrada fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px,color:#000000;
    classDef api fill:#e8f5e9,stroke:#388e3c,stroke-width:2px,color:#000000;
    classDef db fill:#fff3e0,stroke:#f57c00,stroke-width:2px,color:#000000;

    %% Nodos principales
    Cliente[Cliente / Navegador]:::usuario
    Gateway[ECommerce.Gateway <br> Puerto: 5000/7000]:::entrada
    
    UserAPI[User.API <br>5002]:::api
    ProdAPI[Products.API <br>5001]:::api
    CartAPI[Cart.API <br>5003]:::api
    OrderAPI[Order.API <br>5004]:::api
    NotifAPI[Notifications.API <br>5005]:::api

    UserDB[(user.db)]:::db
    ProdDB[(products.db)]:::db
    CartDB[(cart.db)]:::db
    OrderDB[(order.db)]:::db
    NotifDB[(notifications.db)]:::db

    %% Flujo 1: Entrada Externa de la Solicitud
    Cliente -->|1. Ingresa la solicitud HTTP| Gateway
    Gateway -->|2. Rutea segun la URL| UserAPI
    Gateway -->|2. Rutea segun la URL| ProdAPI
    Gateway -->|2. Rutea segun la URL| CartAPI
    Gateway -->|2. Rutea segun la URL| OrderAPI

    %% Flujo 2: Comunicacion Interna (Validaciones Directas)
    CartAPI -->|Cart consulta stock| ProdAPI
    CartAPI -->|Cart consulta usuario| UserAPI

    ProdAPI -->|Prod verifica orden| OrderAPI
    
    OrderAPI -->|Consulta stock y precio| ProdAPI
    OrderAPI -->|Order consulta usuario| UserAPI
    OrderAPI -->|Order notifica orden creada| NotifAPI
    OrderAPI -->|Order vacía carrito| CartAPI
    
    NotifAPI -->|Noti consulta usuario| UserAPI

    %% Flujo 3: Persistencia
    UserAPI --> UserDB
    ProdAPI --> ProdDB
    CartAPI --> CartDB
    OrderAPI --> OrderDB
    NotifAPI --> NotifDB
```
    
### USERS.API SCREENSHOTS

Registro exitoso
<img width="1276" height="670" alt="image" src="https://github.com/user-attachments/assets/379bce8d-a05e-4ede-98fa-c223bb03b27f" />

Error 400 - USR-002
<img width="1269" height="520" alt="image" src="https://github.com/user-attachments/assets/16ee7eec-36b8-4554-bb26-b294c419b794" />

Error 409 - USR-001 -  Mail ya registrado
<img width="1279" height="468" alt="image" src="https://github.com/user-attachments/assets/d83ffd7f-2da7-4b20-a5ec-30ad305d3f46" />


Error 401 - USR-003 - Login con clave incorrecta
<img width="1275" height="469" alt="image" src="https://github.com/user-attachments/assets/63fb0b6e-8bf3-4f5a-88c2-a8e297bbdb71" />


Error 403 - USR-004 - Superar intentos máximos
<img width="1277" height="437" alt="image" src="https://github.com/user-attachments/assets/b5fe8614-710f-43e0-aacc-7c7a228997d3" />



### ORDERS.API SCREENSHOTS

Falla de Integración HTTP - Stock Insuficiente (ORD-005) 
<img width="2012" height="1200" alt="image" src="https://github.com/user-attachments/assets/cb1f8ceb-ccf0-4d3c-b98a-3aa9fa0ffbbc" />
<img width="1855" height="781" alt="image" src="https://github.com/user-attachments/assets/0b1cf6af-3072-40aa-ba4a-b26cf12aa941" />

Transición de Estado Inválida (ORD-006)
<img width="1812" height="1296" alt="image" src="https://github.com/user-attachments/assets/ccfb314e-ff74-4b4a-8c89-6ae7d2cf3420" />
<img width="1810" height="762" alt="image" src="https://github.com/user-attachments/assets/16459b95-6b48-4a9e-b291-5ae9097def98" />



### Products.API ScreenShots

Producto Duplicado (PRD-003)
<img width="1792" height="1298" alt="image" src="https://github.com/user-attachments/assets/fa63573a-5701-46ec-9637-4bf8cec01dd6" />
<img width="1782" height="778" alt="image" src="https://github.com/user-attachments/assets/75f450e1-f45b-4641-8c6b-4d139940c4b3" />


### Cart.Api ScreenShots

Cantidad Inválida en Carrito (CRT-004)
<img width="1785" height="1193" alt="image" src="https://github.com/user-attachments/assets/35cd4ad2-caa9-40c9-94d3-edbeed892bdc" />
<img width="1774" height="775" alt="image" src="https://github.com/user-attachments/assets/5706a144-b64a-469a-94a5-02f58d09160f" />


