using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewingAPI.Domain.Enums
{
    public enum OrderStatus
    {
        InProgress = 0,  // В работе
        Completed = 1,  // Выполнен
        Cancelled = 2,  // Отменён
        OnHold = 3   // На паузе
    }
}
