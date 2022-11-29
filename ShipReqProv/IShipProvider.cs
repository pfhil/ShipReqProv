using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipReqProv
{
    internal interface IShipProvider
    {
        public void StartProvide();
        event Action<Ship> Getship;
    }
}
