namespace ShipReqProv
{
    internal class Dock
    {
        public event Action<Dock>? ShipIsLoaded;
        public string Name { get; init; }
        public ContainerType ServicedProducts { get; }
        public bool IsFree { get; set; }
        public ConsoleColor Color { get; init; }

        public Dock(ContainerType servicedProducts, string name, ConsoleColor color, bool isFree = true)
        {
            this.Color = color;
            this.Name = name;
            this.ServicedProducts = servicedProducts;
            this.IsFree = isFree;
        }

        /// <summary>
        /// Начинает загрузку корабля
        /// </summary>
        /// <param name="ship"></param>
        public void LoadShip(Ship ship)
        {
            Write($"Док {Name} Начал загурзку {ship.Name}");
            while (ship.Load < ship.MaxLoad)
            {
                Thread.Sleep(1000);
                ship.Load += ship.Load + 10 > ship.MaxLoad ? ship.MaxLoad - ship.Load : 10;
                Write($"Док {Name} загрузил корабль {ship.Name} на {ship.Load}/{ship.MaxLoad}");
            }
            Write($"Док {Name} Закончил загурзку {ship.Name}");
            this.ShipIsLoaded?.Invoke(this);
        }

        private void Write(string text)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = this.Color;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }
    }
}
