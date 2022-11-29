using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipReqProv
{
    internal class Tunnel
    {
        private List<Ship> _ships = new List<Ship>();

        private Semaphore semaphore = new Semaphore(7, 7);

        private object shipsLocker = new object();

        public IReadOnlyCollection<Ship> Ships;
        
        public IReadOnlyCollection<Dock> Docks;

        /// <summary>
        /// Добавляет корабль в туннель
        /// </summary>
        /// <param name="ship">Корабль, который необходимо добавить</param>
        public void Add(Ship ship)
        {
            //На этой инструкции, если весь семафор занят, то поток генератора кораблей засыпает и он перестает их генерировать 
            semaphore.WaitOne();
            //Если в туннеле есть свободное место,
            //то остальная работа по передаче корабля на док или оставлении его в туннеле отводится на отдельный поток,
            //а генератор кораблей может дальше продолжить генерировать корабли
            new Thread(() =>
            {
                lock (shipsLocker)
                {
                    //Если есть док, способный сейчас принять полученный корабль, то отправляем корабль к доку; иначе помещаем корабль в туннель (коллекция _ships)
                    if (TryStartShipToDock(ship))
                    {
                        semaphore.Release();
                    }
                    else
                    {
                        _ships.Add(ship);
                    }
                }
            }).Start();
        }

        /// <summary>
        /// Метод-обработчик события, когда док закончил разгрузку и готов к принятию нового корабля
        /// </summary>
        /// <param name="dock">Док, который закончил разгрузку</param>
        private void OnFreeDockHandler(Dock dock)
        {
            lock (shipsLocker)
            {
                //Ищем корабль, который имеет тот же груз, который может разгрузить освободившийся док
                var ship = this._ships.FirstOrDefault(ship => ship.TransportedProducts == dock.ServicedProducts);
                if (ship != null)
                {
                    this._ships.Remove(ship);
                    TryStartShipToDock(ship, dock);
                    semaphore.Release();
                }
                else
                {
                    dock.IsFree = true;
                }
            }
        }

        /// <summary>
        /// Попытаться запустить корабль в док.
        /// </summary>
        /// <param name="ship">Корабль, который необходимо передать в док</param>
        /// <param name="dock">Необязательный параметр - док, который будет принимать корабль. Если параметр не передан, то метод попытается найти подходящий док</param>
        /// <returns>True - если док найден и коабль был передан в него; False - если док не был найден</returns>
        private bool TryStartShipToDock(Ship ship, Dock? dock = null)
        {
            if (dock == null)
                dock = this.Docks.FirstOrDefault(dock => dock.ServicedProducts == ship.TransportedProducts && dock.IsFree);
            if (dock != null)
            {
                dock.IsFree = false;
                Thread threadLoadShip = new Thread(() => dock.LoadShip(ship));
                threadLoadShip.Name = "ThreadDock";
                threadLoadShip.Start();
                return true;
            }

            return false;
        }

        public Tunnel(IShipProvider shipProvider, IReadOnlyCollection<Dock> docks)
        {
            shipProvider.Getship += this.Add;
            this.Ships = this._ships;
            foreach (var dock in docks)
            {
                dock.ShipIsLoaded += this.OnFreeDockHandler;
            }
            this.Docks = docks;
        }
    }
}
