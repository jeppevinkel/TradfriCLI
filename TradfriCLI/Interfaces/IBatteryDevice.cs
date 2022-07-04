namespace TradfriCLI.Interfaces
{
    public interface IBatteryDevice : IDevice
    {
        public int BatteryStatus { get; }
    }
}