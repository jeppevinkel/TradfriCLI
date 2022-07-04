using System;
using TradfriCLI.Interfaces;
using TradfriCLI.Responses;

namespace TradfriCLI.Entities
{
    public class Remote: BaseDevice, IBatteryDevice
    {
        public int BatteryStatus { get; }
        
        public Remote(DeviceResponse deviceResponse) : base(deviceResponse)
        {
            BatteryStatus = deviceResponse.ProductInfo.BatteryStatus;
        }
    }
}