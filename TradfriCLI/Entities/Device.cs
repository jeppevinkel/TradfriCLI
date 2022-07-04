using TradfriCLI.Enums;
using TradfriCLI.Responses;

namespace TradfriCLI.Entities
{
    public class Device
    {
        public string Name { get; private set; }
        public DeviceType Type { get; private set; }
        public string ManufacturerName { get; private set; }
        public string ProductName { get; private set; }
        public string FirmwareVersion { get; private set; }
        public int BatteryStatus { get; private set; }
        public int InstanceId { get; set; }
        public int CreationTimestamp { get; set; }
        public int LastSeenTimestamp { get; set; }
        public bool Reachable { get; private set; }

        public Device(DeviceResponse deviceResponse)
        {
            Name = deviceResponse.DeviceName;
            Type = (DeviceType) deviceResponse.DeviceType;
            ManufacturerName = deviceResponse.ProductInfo.ManufacturerName;
            ProductName = deviceResponse.ProductInfo.ProductName;
            FirmwareVersion = deviceResponse.ProductInfo.FirmwareVersion;
            BatteryStatus = deviceResponse.ProductInfo.BatteryStatus;
            InstanceId = deviceResponse.InstanceId;
            CreationTimestamp = deviceResponse.CreationTimestamp;
            LastSeenTimestamp = deviceResponse.LastSeenTimestamp;
            Reachable = deviceResponse.ReachabilityState != 0;
        }
    }
}