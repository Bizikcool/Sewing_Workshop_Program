namespace SewingWorkshop.WinForms.Api
{
    public sealed class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public sealed class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public sealed class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public sealed class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public sealed class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public sealed class EmployeeCreateRequest
    {
        public string Fio { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Rank { get; set; }
    }

    public sealed class EmployeeUpdateRequest
    {
        public string Fio { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Rank { get; set; }
    }

    public sealed class EmployeeResponse
    {
        public int Id { get; set; }
        public string Fio { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Rank { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class ContragentCreateRequest
    {
        public string Type { get; set; } = "Supplier";
        public string Name { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
    }

    public sealed class ContragentUpdateRequest
    {
        public string Type { get; set; } = "Supplier";
        public string Name { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
    }

    public sealed class ContragentResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MaterialsCount { get; set; }
        public int OrdersCount { get; set; }
    }

    public sealed class MaterialCreateRequest
    {
        public int ContragentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Fabric";
        public string? Color { get; set; }
        public string? Article { get; set; }
        public string Unit { get; set; } = "шт";
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
    }

    public sealed class MaterialUpdateRequest
    {
        public int ContragentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Fabric";
        public string? Color { get; set; }
        public string? Article { get; set; }
        public string Unit { get; set; } = "шт";
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
    }

    public sealed class MaterialResponse
    {
        public int Id { get; set; }
        public int ContragentId { get; set; }
        public string? ContragentName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Article { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Sum { get; set; }
    }

    public sealed class WarehouseUpdateQtyRequest
    {
        public decimal Qty { get; set; }
    }

    public sealed class WarehouseResponse
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialType { get; set; }
        public int ContragentId { get; set; }
        public string? ContragentName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public DateOnly DateAdded { get; set; }
    }

    public sealed class FillCreateRequest
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Related { get; set; } = string.Empty;
    }

    public sealed class FillResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Related { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public sealed class FillStatsResponse
    {
        public int TotalCount { get; set; }
        public int TodayCount { get; set; }
    }

    public sealed class DatabaseHealthResponse
    {
        public bool CanConnect { get; set; }
        public string? Provider { get; set; }
        public string? Database { get; set; }
        public string? DataSource { get; set; }
        public bool ConnectionStringConfigured { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
