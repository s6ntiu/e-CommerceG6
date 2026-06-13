```mermaid 
graph TD
    %% Estilos de los bloques con alto contraste
    classDef usuario fill:#e1f5fe,stroke:#0288d1,stroke-width:2px,color:#000000;
    classDef entrada fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px,color:#000000;
    classDef api fill:#e8f5e9,stroke:#388e3c,stroke-width:2px,color:#000000;
    classDef db fill:#fff3e0,stroke:#f57c00,stroke-width:2px,color:#000000;

    %% Nodos principales
    Cliente[Cliente / Navegador]:::usuario
    Gateway[ECommerce.Gateway <br> Puerto: 7000]:::entrada
    
    UserAPI[User.API <br>5002]:::api
    ProdAPI[Products.API <br>5001]:::api
    CartAPI[Cart.API <br>5000]:::api
    OrderAPI[Order.API <br>5000]:::api
    NotifAPI[Notifications.API <br>5000]:::api

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
    
    OrderAPI -->|Consulta stock y precio| ProdAPI
    OrderAPI -->|Order consulta usuario| UserAPI
    
    NotifAPI -->|Noti consulta usuario| UserAPI

    %% Flujo 3: Persistencia
    UserAPI --> UserDB
    ProdAPI --> ProdDB
    CartAPI --> CartDB
    OrderAPI --> OrderDB
    NotifAPI --> NotifDB
```
    
    USERS.API SCREENSHOTS

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



ORDERS.API SCREENSHOTS

Falla de Integración HTTP - Stock Insuficiente (ORD-005) 
<img width="2012" height="1200" alt="image" src="https://github.com/user-attachments/assets/cb1f8ceb-ccf0-4d3c-b98a-3aa9fa0ffbbc" />
<img width="1855" height="781" alt="image" src="https://github.com/user-attachments/assets/0b1cf6af-3072-40aa-ba4a-b26cf12aa941" />




Products.API ScreenShots
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/e4e62ea6-c247-4363-81a3-ed0f2381b997" />
