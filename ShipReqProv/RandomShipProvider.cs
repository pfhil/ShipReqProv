namespace ShipReqProv
{
    internal class RandomShipProvider : IShipProvider
    {
        public event Action<Ship>? Getship;

        private int Millisecond { get; set; }

        public RandomShipProvider(int Millisecond)
        {
            this.Millisecond = Millisecond;
        }
        private void GenerateShip()
        {
            Random random = new Random();

            Array values = Enum.GetValues(typeof(ContainerType));
            ContainerType randomContainerType = (ContainerType)values.GetValue(random.Next(values.Length))!;

            values = Enum.GetValues(typeof(SizeType));
            SizeType randomSizeType = (SizeType)values.GetValue(random.Next(values.Length))!;

            string name = GenerateName(10);

            lock (Console.Out)
                Console.WriteLine($"Создал корабль {name} с грузом {randomContainerType} и размером {randomSizeType}");

            //Чрезмерное обобщение метафоры о короблях,
            //может привести к тому, что те корабли,
            //которые не могут войти в туннель, будут скапливаться перед ним,
            //что в нашем случае может привести к бесконечному генерированию новых экземпляров класса Ship,
            //которые не могут войти в туннель, но скапливаются перед ним
            //и в конечном итоге рано или поздно ОЗУ компьютера будет полностью занято
            
            //Вариант поведения, который предполагает такое скапливание кораблей пере туннелем:
            //Thread thread = new Thread(() => Getship?.Invoke(new Ship(randomContainerType, randomSizeType)));
            //thread.Start();

            //Конечно в рабочем проекте такой вариант необходимо предусмотреть,
            //но тогда отвественность за отслеживанием кораблей, которые скапливаются перед туннелем,
            //будет лежать на структурах более вместительных чем куча, к примеру, база данных.

            //В учебном проекте обойдемся поведением,
            //когда генератор кораблей будет предоставлять новый экземлпяр корабля в случае освобождения места в туннеле
            Getship?.Invoke(new Ship(randomContainerType, randomSizeType, name));
        }

        /// <summary>
        /// Генерирует случайное имя
        /// </summary>
        /// <param name="len">Длинна имени</param>
        /// <returns>Имя заданной длинны</returns>
        private static string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2;
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }

        /// <summary>
        /// Запускает генерацию кораблей
        /// </summary>
        public void StartProvide()
        {
            while (true)
            {
                GenerateShip();
                Thread.Sleep(Millisecond);
            }
        }
    }
}
