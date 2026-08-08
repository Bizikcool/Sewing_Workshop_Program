# Запуск проекта

Проект разделен на две самостоятельные части:

- `SewingAPI.sln` - ASP.NET Core Web API.
- `SewingWorkshop.WinForms.sln` - отдельная Windows Forms программа.

## 1. Настроить PostgreSQL

Проверьте строку подключения в `SewingAPI/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=sewing_workshop;Username=postgres;Password=postgres"
}
```

## 2. Запустить API

```powershell
dotnet run --project .\SewingAPI\SewingAPI.csproj
```

По умолчанию API запускается на:

```text
http://localhost:5217
```

Swagger:

```text
http://localhost:5217/swagger
```

## 3. Запустить Windows Forms программу

```powershell
dotnet run --project .\SewingWorkshop.WinForms\SewingWorkshop.WinForms.csproj
```

В окне авторизации укажите:

```text
API URL: http://localhost:5217/api/
```

Для первого запуска можно включить `Создать пользователя`, чтобы зарегистрировать первого пользователя. После входа программа отправляет JWT Bearer token во все запросы к API.

## 4. Проверить сборку

API:

```powershell
dotnet build .\SewingAPI.sln
```

Windows Forms:

```powershell
dotnet build .\SewingWorkshop.WinForms.sln
```

Ожидаемый результат для обоих решений:

```text
Сборка успешно завершена.
Предупреждений: 0
Ошибок: 0
```
