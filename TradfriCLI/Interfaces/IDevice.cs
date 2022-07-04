using TradfriCLI.Enums;
using TradfriCLI.Responses;

namespace TradfriCLI.Interfaces
{
    public interface IDevice
    {
        public string Name { get; }
        public DeviceType Type { get; }

        public string ManufacturerName { get; }
        public string ProductName { get; }
        public string FirmwareVersion { get; }
        public int InstanceId { get; }
        public int CreationTimestamp { get; }
        public int LastSeenTimestamp { get; }
        public bool Reachable { get; }
    }
}