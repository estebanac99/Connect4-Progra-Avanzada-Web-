# Repositorio para el proyecto final de Programación Avanzada Web SC-701 - Grupo 1

## Integrantes

- Nombre: Esteban Aguilar Chavarría
  Carné: FI19013629  
  Usuario Git: estebanac99  
  Correo Git: eaguilarchavarria@gmail.com  

- Nombre: Emma Alfaro Vargas  
  Carné: FI22026273  
  Usuario Git: emmalfaro
  Correo Git: emma.alfaro2310@gmail.com 

- Nombre: Llerym Choi Gonzales   
  Carné: FI23028341  
  Usuario Git: Llerym  
  Correo Git: llerymch25@gmail.com

---

## Frameworks y Herramientas Utilizadas

- **ASP.NET MVC (.NET Framework 4.8.1)** Framework principal para la arquitectura MVC.  
- **Entity Framework** ORM para el manejo de base de datos.  
- **Razor View Engine** Motor de vistas para la integración de C# con HTML.  
- **C#** Lenguaje backend para controladores, modelos y lógica de negocio.  
- **JavaScript** Validaciones y mejoras en la interacción del frontend.  
- **HTML5 y CSS3** Estructura y diseño de las vistas.  
- **SQL Server** Base de datos relacional utilizada.  
- **Visual Studio 2022** Entorno de desarrollo integrado (IDE).  
- **Git y GitHub** Control de versiones y colaboración en equipo.  

---

## Tipo de Aplicación

- **MPA (Multi-Page Application)** utilizando **patrón MVC**.  

---

## Arquitectura

El proyecto sigue la arquitectura **Modelo-Vista-Controlador (MVC)**:  

- **Modelo**: Representa las entidades 'Jugador", "Partida" y "Movimiento".  
- **Vista**: Renderiza la interfaz (archivos .cshtml).  
- **Controlador**: Gestiona la lógica y comunica modelos con vistas.  

---

## Definición de la Base de Datos

La base de datos es **relacional en SQL Server**, con las siguientes tablas y relaciones:

```mermaid
erDiagram
    direction LR

    JUGADORES {
        int JugadorId PK "Primary key. Manual"
        nvarchar Nombre "Nombre del jugador"
        int Marcador "Marcador acumulado"
        int Ganadas "Partidas ganadas"
        int Perdidas "Partidas perdidas"
        int Empatadas "Partidas empatadas"
    }

    PARTIDAS {
        int PartidaId PK "Primary Key, identity"
        datetime FechaHora "Fecha y hora"
        int Jugador1Id FK "Jugador 1 (FK → Jugadores)"
        int Jugador2Id FK "Jugador 2 (FK → Jugadores)"
        nvarchar Estado "Estado de la partida"
        int TurnoJugadorId FK "Jugador con el turno actual (FK → Jugadores)"
        nvarchar Resultado "Resultado final"
    }

    MOVIMIENTOS {
        int MovimientoId PK "Primary Key, identity"
        int PartidaId FK "FK → Partidas"
        int JugadorId FK "FK → Jugadores"
        char Columna "Columna jugada"
        int Fila "Fila jugada"
        int OrdenTurno "Secuencia del movimiento"
        datetime FechaHora "Fecha y hora"
    }

    JUGADORES ||--o{ PARTIDAS : juega
    JUGADORES ||--o{ MOVIMIENTOS : realiza
    PARTIDAS ||--o{ MOVIMIENTOS : tiene

**Relaciones principales:**  
- Un **Jugador** puede participar en múltiples **Partidas** (como Jugador1 o Jugador2).  
- Una **Partida** contiene múltiples **Movimientos**.  
- Cada **Movimiento** está asociado a un **Jugador** y a una **Partida**. 

## Instructivo de Instalación y Ejecución

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/estebanac99/Connect4-Progra-Avanzada-Web-
   cd Connect4.Web
2. Crear la base de datos a partir de las migraciones:
   ```bash
   dotnet ef database update
3. Ejecutar la aplicación:
   ```bash
   dotnet run
4. Abrir en el navegador:
   ```bash
   http://localhost:5293

## Referencias:
- Overview of ASP.NET Core MVC (https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-9.0)
- Entity Framework Core (https://learn.microsoft.com/en-us/ef/core/)
- Bootstrap 5 (https://getbootstrap.com/docs/5.0/getting-started/introduction/)
- jQuery API (https://api.jquery.com/)
- Connect 4 in C# windows form application - Referencia StackOverFlow (https://stackoverflow.com/questions/43036662/connect-4-in-c-sharp-windows-form-application)
- How to create the Connect 4 game in Blazor WebAssembly in one hour! - Referencia (https://www.roundthecode.com/dotnet-tutorials/create-connect-4-blazor-webassembly-in-hour)
- Connect Four game using html css bootstrap and javascript - Referencia Youtube (https://www.youtube.com/shorts/IrkxKEKQ374)

## Prompts y Respuestas de ChatGPT (IA utilizada):

Desde un inicio también le pedimos a ChatGPT que nos diera respuestas resumidas en base a nuestros prompts, considerando el requerimiento del readme (para evitar un exceso de líneas de código en este apartado).

1. ¿Cómo creo un modelo Jugador en ASP.NET Core con validación para que el ID se inserte manualmente?
IA: Puedes usar [DatabaseGenerated(DatabaseGeneratedOption.None)] en la propiedad JugadorId y agregar [Range(1, int.MaxValue)] para forzar valores positivos.

2. ¿Cómo defino la relación entre Jugador y Partida en Entity Framework?
IA: Usa propiedades de navegación como public ICollection<Partida> PartidasComoJugador1 { get; set; }. EF reconocerá las relaciones por convención con las FKs.

3. ¿Cómo puedo generar la base de datos a partir de mis modelos en EF Core?
IA: Usa dotnet ef migrations add InitialCreate y luego dotnet ef database update para generar el esquema.

4. ¿Cómo hago un bucle en Razor para renderizar un tablero 6x7 en una vista?
IA: Puedes usar dos for anidados en tu .cshtml: el primero para las filas y el segundo para las columnas.

5. ¿Cómo resalto las celdas ganadoras en mi vista de Razor?
IA: Puedes pasar una lista de coordenadas ganadoras al modelo y en la vista aplicar un if (coordsGanadoras.Contains(clave)) para cambiar el color de fondo.

6. ¿Cómo valido que un jugador no pueda editar el ID en la vista de edición?
IA: No incluyas JugadorId en el formulario o márcalo como readonly. Además, en el POST valida que id == jugador.JugadorId.

7. ¿Cómo agrego Bootstrap a mi proyecto ASP.NET Core MVC?
IA: Puedes instalarlo vía LibMan o CDN. Luego incluyes el CSS en _Layout.cshtml con <link href="...bootstrap.min.css" rel="stylesheet" />.

8. ¿Cómo implemento botones debajo de cada columna del tablero en lugar de un dropdown?
IA: Crea un <form> con botones tipo <button name="columna" value="A">A</button>. Al enviarlo, sabrás qué columna fue seleccionada.

9. ¿Cómo puedo evitar duplicar IDs de jugadores en la base de datos?
IA: Configura JugadorId como clave primaria y desactiva la autogeneración con DatabaseGeneratedOption.None. Si intentas insertar un duplicado, SQL Server lanzará un error.

10. Me da error 404 al intentar editar un jugador con un ID distinto, ¿qué puede ser?
IA: Probablemente intentaste modificar el JugadorId. Como es la PK, EF no permite actualizarla. Solo debes permitir editar el Nombre.

11. Mis compañeras no pueden abrir la solución en Visual Studio, pero sí con dotnet run. ¿Por qué?
IA: Tienes más de un archivo .sln en el repo y VS no sabe cuál abrir. Debes conservar el que apunta al .csproj correcto y eliminar el duplicado.