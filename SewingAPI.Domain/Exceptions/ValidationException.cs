using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewingAPI.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base("Ошибка валидации")
        {
            Errors = errors.ToList().AsReadOnly();
        }

        public ValidationException(string error)
            : base("Ошибка валидации")
        {
            Errors = new[] { error }.AsReadOnly();
        }
    }
}
