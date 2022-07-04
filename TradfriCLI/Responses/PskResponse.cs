using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradfriCLI.Responses
{
    public class PskResponse
    {
        [JsonPropertyName("9091")]
        public string PreSharedKey { get; set; }
        [JsonPropertyName("9029")]
        public string GatewayFirmwareVersion { get; set; }
    }
}