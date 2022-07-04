using System;
using System.Text.Json.Serialization;
using TradfriCLI.Entities;
using TradfriCLI.Interfaces;

namespace TradfriCLI.Responses
{
    public class DeviceResponse
    {
        [JsonPropertyName("9001")]
        public string DeviceName { get; set; }
        [JsonPropertyName("9002")]
        public int CreationTimestamp { get; set; }
        [JsonPropertyName("9003")]
        public int InstanceId { get; set; }
        [JsonPropertyName("9020")]
        public int LastSeenTimestamp { get; set; }
        [JsonPropertyName("9019")]
        public int ReachabilityState { get; set; }
        [JsonPropertyName("3")]
        public ProductInfoResponse ProductInfo { get; set; }
        [JsonPropertyName("3311")]
        public BulbDataResponse[] BulbData { get; set; }
        [JsonPropertyName("5750")]
        public int DeviceType { get; set; }

        /// <summary>
        /// Returns a device object based on the response.
        /// </summary>
        /// <returns></returns>
        public IDevice ToDevice()
        {
            Enums.DeviceType type = (Enums.DeviceType) DeviceType;
            
            switch (type)
            {
                case Enums.DeviceType.Bulb:
                    return new Bulb(this);
                case Enums.DeviceType.Remote:
                    return new Remote(this);
                default:
                    return null;
            }
        }

        public class ProductInfoResponse
        {
            [JsonPropertyName("0")]
            public string ManufacturerName { get; set; }
            [JsonPropertyName("1")]
            public string ProductName { get; set; }
            [JsonPropertyName("3")]
            public string FirmwareVersion { get; set; }
            [JsonPropertyName("9")]
            public int BatteryStatus { get; set; }
        }
        
        public class BulbDataResponse
        {
            [JsonPropertyName("5850")]
            public int State { get; set; }
            [JsonPropertyName("5851")]
            public int Dimmer { get; set; }
            [JsonPropertyName("5706")]
            public string ColorHex { get; set; }
            [JsonPropertyName("5712")]
            public int TransitionTime { get; set; }
        }
    }
}