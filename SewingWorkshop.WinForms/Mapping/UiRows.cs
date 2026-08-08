namespace SewingWorkshop.WinForms.Mapping
{
    public sealed class EmployeeRow
    {
        public int Id { get; set; }
        public string Fio { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Rank { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public sealed class ContragentRow
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public int MaterialsCount { get; set; }
        public int OrdersCount { get; set; }
    }

    public sealed class MaterialRow
    {
        public int Id { get; set; }
        public int ContragentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Sum { get; set; }
    }

    public sealed class WarehouseRow
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int ContragentId { get; set; }
        public string ContragentName { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public string DateAdded { get; set; } = string.Empty;
    }

    public sealed class FillRow
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Related { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
