# Hola profe,

Este es nuestro trabajo de la primera entrega del micro servicio de pedidos. 

Tecnologías utilizadas: .net core 8 (C#) y Mongo Atlas. la db la tenemos full expuesta  0.0.0.0/0 \:v para que realice las pruebas respectivas.

Esquema básico uml, sin agregar todas las clases 
![Diagrama de clases](Service-pedido.drawio.png)

## Cómo Ejecutar el Proyecto localmente

1. **Clonar este repositorio**
    ```sh
   git clone https://github.com/DevJerm/Microservicio-pedidos.git
   ```

2. **ir o pararse en la ruta donde se encuentra en docker-compose.yml**
    ```sh
   cd ruta/docker-compose.yml
   ```
    
3. **Construir la imagen Docker desde el docker compose**

   ```sh
   docker-compose build
   ```
   
4. **Inicia los contenedores construidos en el paso anterior**

   ```sh
   docker-compose up
   ```

    **¡¡Importante!!** : Este despliegue esta preparado para salir por el puerto 8080 desde el contenedor. Si se desea modificar el puerto, se debe cambiar el docker-compose.yml y colocar el puerto deseado, y tambien en el program.cs
   
   builder.WebHost.ConfigureKestrel(serverOptions =>
   {
       serverOptions.ListenAnyIP(8080); <--- Colocar el puerto deseado
   });

El puerto local lo puede cambiar sin problema
[puerto_local]:[puerto_contenedor]

6. **Acceder a la API**

   - ahora la API esta disponible en: `http://localhost:8080`
   - La documentación Swagger esta disponible en: `http://localhost:8080/swagger`

## Pruebas de Endpoints

### GET

- Obtener cliente por ID: da5004fb-191c-4f75-88f2-e9ec77afa6f0

- Obtener pedido por ID: a0e3c6c5-e337-44cf-9266-973e74f0eaf8

- Obtener pedidos por estado: CREADO

### POST

Los endpoints POST están documentados en Swagger. 

Evidencias del coverage 

![Cobertura de pruebas unitarias](Coverage_micro_pedidos.png)

cualquier novedad quedamos super atentos y nos puede escribir via TEAMS, 

Saludos!!


John Estiven Restrepo Marin 

Carlos Alejandro Zuluaga Lopez

Juan Andres Loaiza Acosta
