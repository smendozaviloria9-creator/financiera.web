# Cooperativa El Progreso — Sistema de Gestión Financiera

Sistema de gestión para una cooperativa financiera, desarrollado en **C# / ASP.NET Core MVC**. Permite administrar asociados, registrar movimientos financieros (consignaciones y retiros), consultar saldos en pesos y dólares (TRM oficial), y generar informes de gerencia.

Este proyecto nació como una aplicación de consola y fue migrado a una aplicación web manteniendo toda la lógica de negocio original.

## Funcionalidades

- **Gestión de asociados**: registrar, listar, buscar por documento o nombre, actualizar y eliminar (baja lógica).
- **Movimientos financieros**: registrar consignaciones y retiros, con validación de fondos y comisión automática ($8.000) en retiros mayores a $1.000.000.
- **Consulta de saldo**: saldo en pesos (COP) y su equivalente en dólares (USD), calculado con la TRM oficial obtenida en tiempo real desde la API de [datos.gov.co](https://www.datos.gov.co).
- **Informes de gerencia**:
  1. Saldo total de la cooperativa y promedio por asociado.
  2. Ranking de los 5 asociados con mayor saldo.
  3. Asociados sin movimientos registrados ("dormidos").
  4. Resumen de consignaciones/retiros por periodo de fechas.
  5. Los 10 movimientos más grandes registrados.
  6. Actividad de movimientos por asociado.

## Tecnologías usadas

- **C# / .NET** — ASP.NET Core MVC
- **Bootstrap 5** — interfaz web
- **LINQ** — consultas sobre las colecciones en memoria
- **Programación asíncrona** (`async`/`await`) — consumo de la API de TRM
- Persistencia en memoria mediante repositorios desacoplados (`IAssociatedRepository`, `IMovementRepository`)

## Estructura del proyecto
Financiera/

├── Financiera.slnx

└── Financiera.Web/

├── Controllers/ # HomeController, AssociatedsController, MovementsController, ReportsController

├── Interfaces/ # IAssociatedRepository, IMovementRepository

├── Models/ # Associated, Movement, TrmDto

├── Repositories/ # Implementaciones en memoria + InMemoryDatabase

├── Services/ # BankingService, ExchangeRateService

├── ViewModels/ # Modelos de formularios y de informes

└── Views/ # Vistas Razor (.cshtml)

## Cómo ejecutar el proyecto

Requisitos: [.NET SDK](https://dotnet.microsoft.com/download) instalado.

```bash
git clone https://github.com/smendozaviloria9-creator/financiera.web.git
```

```bash
cd financiera.web/Financiera.Web
```

```bash
dotnet restore
```

```bash
dotnet run
```

Luego abre la URL que indique la consola (por defecto algo como `http://localhost:5000`).

## Modelo de base de datos (referencia)

Aunque la persistencia actual es en memoria, el modelo relacional equivalente es:

```sql
CREATE TABLE Associated (
    DocumentNumber VARCHAR(20) PRIMARY KEY,
    FullName       VARCHAR(100) NOT NULL,
    Phone          VARCHAR(20) NOT NULL,
    Address        VARCHAR(150) NOT NULL,
    IsActive       BIT NOT NULL DEFAULT 1
);

CREATE TABLE Movement (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    DocumentNumber VARCHAR(20) NOT NULL,
    Type           INT NOT NULL, -- 0: Depósito, 1: Retiro
    Amount         DECIMAL(18,2) NOT NULL,
    Commission     DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    Date           DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Movement_Associated FOREIGN KEY (DocumentNumber)
        REFERENCES Associated(DocumentNumber)
);
```

## Autor

**Sebastian David Mendoza Viloria**
