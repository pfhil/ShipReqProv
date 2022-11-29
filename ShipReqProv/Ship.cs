using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipReqProv
{
    internal class Ship
    {
        public string Name { get; init; }
        public ContainerType TransportedProducts { get; }
        public SizeType Size { get; }
        public int MaxLoad { get; }
        private int _load = 0;
        public int Load 
        {
            get => _load;
            set
            {
                if (value > MaxLoad || value < 0)
                    throw new ArgumentException();
                else
                    _load = value;
            }
        }
        public Ship(ContainerType containerType, SizeType sizeType, string name)
        {
            this.TransportedProducts = containerType;
            this.Size = sizeType;
            this.Name = name;

            switch (sizeType)
            {
                case SizeType.Small:
                    this.MaxLoad = (int)SizeType.Small;
                    break;
                case SizeType.Medium:
                    this.MaxLoad = (int)SizeType.Medium;
                    break;
                case SizeType.Large:
                    this.MaxLoad = (int)SizeType.Large;
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
