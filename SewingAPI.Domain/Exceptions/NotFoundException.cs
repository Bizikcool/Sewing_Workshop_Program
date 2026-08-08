using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewingAPI.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, object id)
            : base($"{entity} с идентификатором {id} не найден.") { }

        public NotFoundException(string message) : base(message) { }
    }
}
