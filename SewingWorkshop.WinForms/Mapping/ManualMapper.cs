using SewingWorkshop.WinForms.Api;

namespace SewingWorkshop.WinForms.Mapping
{
    public static class ManualMapper
    {
        public static EmployeeRow ToRow(EmployeeResponse response)
        {
            return new EmployeeRow
            {
                Id = response.Id,
                Fio = response.Fio,
                Position = response.Position,
                Rank = response.Rank,
                CreatedAt = response.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy")
            };
        }

        public static ContragentRow ToRow(ContragentResponse response)
        {
            return new ContragentRow
            {
                Id = response.Id,
                Type = ToContragentLabel(response.Type),
                Name = response.Name,
                Contact = response.Contact ?? string.Empty,
                Phone = response.Phone ?? string.Empty,
                CreatedAt = response.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy"),
                MaterialsCount = response.MaterialsCount,
                OrdersCount = response.OrdersCount
            };
        }

        public static MaterialRow ToRow(MaterialResponse response)
        {
            return new MaterialRow
            {
                Id = response.Id,
                ContragentId = response.ContragentId,
                Name = response.Name,
                Type = ToMaterialLabel(response.Type),
                Color = response.Color ?? string.Empty,
                Article = response.Article ?? string.Empty,
                Unit = response.Unit,
                Qty = response.Qty,
                Price = response.Price,
                Sum = response.Sum
            };
        }

        public static WarehouseRow ToRow(WarehouseResponse response)
        {
            return new WarehouseRow
            {
                Id = response.Id,
                MaterialId = response.MaterialId,
                ContragentId = response.ContragentId,
                ContragentName = response.ContragentName ?? string.Empty,
                MaterialName = response.MaterialName ?? string.Empty,
                MaterialType = ToMaterialLabel(response.MaterialType ?? string.Empty),
                Qty = response.Qty,
                Price = response.Price,
                DateAdded = response.DateAdded.ToString("dd.MM.yyyy")
            };
        }

        public static FillRow ToRow(FillResponse response)
        {
            return new FillRow
            {
                Id = response.Id,
                Type = response.Type,
                Description = response.Description,
                Related = response.Related,
                CreatedAt = response.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                Status = response.Status
            };
        }

        public static string ToContragentApiValue(string label)
        {
            return label == "Заказчик" ? "Customer" : "Supplier";
        }

        public static string ToContragentLabel(string value)
        {
            return value == "Customer" ? "Заказчик" : "Поставщик";
        }

        public static string ToMaterialApiValue(string label)
        {
            return label == "Нитки" ? "Thread" : "Fabric";
        }

        public static string ToMaterialLabel(string value)
        {
            return value == "Thread" ? "Нитки" : "Ткань";
        }
    }
}
