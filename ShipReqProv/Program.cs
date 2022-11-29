using ShipReqProv;

List<Dock> docks = new List<Dock>()
{
    new Dock(ContainerType.Banana, "Dock1", ConsoleColor.Red),
    new Dock(ContainerType.Auto, "Dock2", ConsoleColor.Green),
    new Dock(ContainerType.KeyBoard, "Dock3", ConsoleColor.Blue)
};
RandomShipProvider shipProvider = new RandomShipProvider(100);
Tunnel tunnel = new Tunnel(shipProvider, docks);

Thread thread = new Thread(shipProvider.StartProvide);
thread.Start();
