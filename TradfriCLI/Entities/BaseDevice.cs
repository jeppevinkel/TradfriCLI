using TradfriCLI.Enums;
using TradfriCLI.Interfaces;
using TradfriCLI.Responses;

namespace TradfriCLI.Entities
{
    public abstract class BaseDevice : IDevice
    {
        public string Name { get; }
        public DeviceType Type { get; }

        public string TypeString => Type.ToString();

        public string ManufacturerName { get; }
        public string ProductName { get; }
        public string FirmwareVersion { get; }
        public int InstanceId { get; }
        public int CreationTimestamp { get; }
        public int LastSeenTimestamp { get; }
        public bool Reachable { get; }
        
        protected BaseDevice(DeviceResponse deviceResponse)
        {
            Name = deviceResponse.DeviceName;
            Type = (DeviceType) deviceResponse.DeviceType;
            ManufacturerName = deviceResponse.ProductInfo.ManufacturerName;
            ProductName = deviceResponse.ProductInfo.ProductName;
            FirmwareVersion = deviceResponse.ProductInfo.FirmwareVersion;
            InstanceId = deviceResponse.InstanceId;
            CreationTimestamp = deviceResponse.CreationTimestamp;
            LastSeenTimestamp = deviceResponse.LastSeenTimestamp;
            Reachable = deviceResponse.ReachabilityState != 0;
        }
    }
}