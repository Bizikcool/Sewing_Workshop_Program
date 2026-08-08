using SewingAPI.Domain.Exceptions;

namespace SewingAPI.Service.Validation
{
    public static class ContragentValidator
    {
        public static void Validate(Domain.Entities.Contragent contragent)
        {
            if (string.IsNullOrWhiteSpace(contragent.Name))
            {
                throw new ValidationException("Название контрагента обязательно.");
            }
        }
    }
}
