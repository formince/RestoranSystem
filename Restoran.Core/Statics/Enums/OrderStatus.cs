using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Statics.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Preparing = 2,
        ReadyForDelivery = 3,
        Completed = 4,
        Cancelled = 5
    }
}
