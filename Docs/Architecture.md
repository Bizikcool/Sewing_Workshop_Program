# Архитектура Sewing Workshop

## Проверка требований

| Требование | Где реализовано |
| --- | --- |
| JWT token | `SewingAPI/Program.cs`, `SewingAPI.Service/Services/AuthService.cs`, `SewingWorkshop.WinForms/Api/ApiClient.cs` |
| Пагинация | `SewingAPI.Service/DTOs/Shared/PagedResult.cs`, `SewingWorkshop.WinForms/Api/ApiModels.cs` |
| Ручной маппинг | `SewingAPI.Service/Mapping/*Mapper.cs`, `SewingWorkshop.WinForms/Mapping/ManualMapper.cs` |
| Валидация | `SewingAPI.Service/Validation/*Validator.cs`, `SewingWorkshop.WinForms/UI/Dialogs.cs` |
| Request/Response модели | `SewingWorkshop.WinForms/Api/*Request`, `SewingWorkshop.WinForms/Api/*Response`, API request/response модели |
| Слоеная / луковая архитектура | `Domain -> Service -> Repo -> API -> WinForms` |
| Отдельная desktop-программа | `SewingWorkshop.WinForms.sln`, `SewingWorkshop.WinForms/SewingWorkshop.WinForms.csproj` |

## Onion Architecture

```mermaid
flowchart TB
    UI["SewingWorkshop.WinForms<br/>Windows Forms Desktop App"]
    API["SewingAPI<br/>ASP.NET Core Web API"]
    SERVICE["SewingAPI.Service<br/>Business logic, validation, manual mapping"]
    DOMAIN["SewingAPI.Domain<br/>Entities, enums, exceptions, interfaces"]
    REPO["SewingAPI.Repo<br/>EF Core repositories, AppDbContext"]
    DB[("PostgreSQL")]

    UI -->|"HTTP + Bearer JWT"| API
    API --> SERVICE
    SERVICE --> DOMAIN
    SERVICE -->|"repository interfaces"| DOMAIN
    REPO -->|"implements interfaces"| DOMAIN
    API --> REPO
    REPO -->|"EF Core / Npgsql"| DB

    classDef core fill:#111,color:#fff,stroke:#111;
    classDef app fill:#f7f7f7,color:#111,stroke:#aaa;
    classDef data fill:#fff,color:#111,stroke:#aaa;

    class DOMAIN core;
    class SERVICE,API,UI app;
    class REPO,DB data;
```

## API Class Diagram

```mermaid
classDiagram
    class Employee {
        int Id
        string Fio
        string Position
        int Rank
        DateTime CreatedAt
    }

    class Contragent {
        int Id
        ContragentType Type
        string Name
        string Contact
        string Phone
        DateTime CreatedAt
    }

    class Material {
        int Id
        int ContragentId
        string Name
        MaterialType Type
        decimal Qty
        decimal Price
        decimal Sum
    }

    class WarehouseItem {
        int Id
        int MaterialId
        int ContragentId
        decimal Qty
        decimal Price
        DateOnly DateAdded
    }

    class Fill {
        int Id
        string Type
        string Description
        string Related
        string Status
        DateTime CreatedAt
    }

    class AppUser {
        int Id
        string Username
        string PasswordHash
        string Role
        string RefreshToken
        DateTime TokenExpiresAt
    }

    class AuthService {
        RegisterAsync()
        LoginAsync()
        RefreshAsync()
    }

    Contragent "1" --> "*" Material
    Contragent "1" --> "*" Order
    Material "1" --> "0..1" WarehouseItem
    AppUser --> AuthService
```

## WinForms Client Diagram

```mermaid
classDiagram
    class MainForm {
        ShowSectionAsync()
        LoadEmployeesAsync()
        LoadContragentsAsync()
        LoadWarehouseAsync()
        LoadFillsAsync()
    }

    class ApiClient {
        string BaseUrl
        string AccessToken
        LoginAsync()
        RegisterAsync()
        GetEmployeesAsync()
        GetContragentsAsync()
        GetWarehouseAsync()
        GetFillsAsync()
    }

    class ManualMapper {
        ToRow(EmployeeResponse)
        ToRow(ContragentResponse)
        ToRow(MaterialResponse)
        ToRow(WarehouseResponse)
        ToRow(FillResponse)
    }

    class AuthDialog
    class EmployeeDialog
    class ContragentDialog
    class MaterialDialog
    class QuantityDialog

    MainForm --> ApiClient
    MainForm --> ManualMapper
    MainForm --> AuthDialog
    MainForm --> EmployeeDialog
    MainForm --> ContragentDialog
    MainForm --> MaterialDialog
    MainForm --> QuantityDialog
    ApiClient --> "Request/Response models"
```

## UI Structure

```mermaid
flowchart LR
    Sidebar["Черный sidebar<br/>Заполнения / Сотрудники / Контрагенты / Склад / Настройки"]
    Topbar["Topbar<br/>заголовок, подзаголовок, API status, часы"]
    Content["Content area<br/>статистика, фильтры, таблицы, пагинация"]
    Modal["Dialogs<br/>auth, employee, contragent, material, warehouse qty"]

    Sidebar --> Topbar
    Topbar --> Content
    Content --> Modal
```
