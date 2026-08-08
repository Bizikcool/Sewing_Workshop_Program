using SewingAPI.Domain.Exceptions;

namespace SewingAPI.Service.Validation
{
    public static class MaterialValidator
    {
        public static void Validate(Domain.Entities.Material material)
        {
            var errors = new List<string>();

            if (material.ContragentId <= 0)
            {
                errors.Add("Контрагент обязателен.");
            }

            if (string.IsNullOrWhiteSpace(material.Name))
            {
                errors.Add("Название материала обязательно.");
            }

            if (string.IsNullOrWhiteSpace(material.Unit))
            {
                errors.Add("Единица измерения обязательна.");
            }

            if (material.Qty < 0)
            {
                errors.Add("Количество не может быть отрицательным.");
            }

            if (material.Price < 0)
            {
                errors.Add("Цена не может быть отрицательной.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
}
