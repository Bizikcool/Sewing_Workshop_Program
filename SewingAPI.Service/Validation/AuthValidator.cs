using SewingAPI.Domain.Exceptions;

namespace SewingAPI.Service.Validation
{
    public static class AuthValidator
    {
        public static void ValidateCredentials(string username, string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
            {
                errors.Add("Имя пользователя обязательно.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Пароль обязателен.");
            }

            if (!string.IsNullOrWhiteSpace(password) && password.Length < 6)
            {
                errors.Add("Пароль должен содержать минимум 6 символов.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
}
