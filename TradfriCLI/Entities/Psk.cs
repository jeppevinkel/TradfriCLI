using TradfriCLI.Responses;

namespace TradfriCLI.Entities
{
    public class Psk
    {
        public string PreSharedKey { get; set; }
        public string GatewayFirmwareVersion { get; set; }

        public Psk(string psk, string gatewayFirmwareVersion)
        {
            PreSharedKey = psk;
            GatewayFirmwareVersion = gatewayFirmwareVersion;
        }

        public Psk(PskResponse pskResponse)
        {
            PreSharedKey = pskResponse.PreSharedKey;
            GatewayFirmwareVersion = pskResponse.GatewayFirmwareVersion;
        }
    }
}