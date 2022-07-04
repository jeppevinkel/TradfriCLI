using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CoAPnet;
using CoAPnet.Client;
using CoAPnet.Extensions.DTLS;
using CoAPnet.Logging;
using TradfriCLI.Entities;
using TradfriCLI.Interfaces;
using TradfriCLI.Responses;

namespace TradfriCLI
{
    public class TradfriClient
    {
        private readonly string _host;
        private readonly string _clientPsk;
        private readonly string _clientId;

        private readonly CoapFactory _coapFactory;
        
        public TradfriClient(string host, string psk, string clientId = "trfClient")
        {
            _host = host;
            _clientPsk = psk;
            _clientId = clientId;

            _coapFactory = new CoapFactory();
            // _coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());
        }
        
        /// <summary>
        /// Generate a new PSK client token.
        /// </summary>
        /// <param name="gatewayPsk">The PSK found on the bottom of the gateway.</param>
        /// <exception cref="Exception"></exception>
        public async Task GenerateTradfriPskToken(string gatewayPsk)
        {
            using (var coapClient = _coapFactory.CreateClient())
            {
                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost(_host)
                    .WithDtlsTransportLayer(o =>
                        o.WithPreSharedKey("Client_identity", gatewayPsk))
                    .Build();

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token).ConfigureAwait(false);
                }

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Post)
                    .WithPath("15011/9063")
                    .WithPayload($"{{\"9090\":\"{_clientId}\"}}")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);

                PrintResponse(response);
                
                if (response.StatusCode != CoapResponseStatusCode.Valid)
                {
                    throw new Exception($"Error: {response.StatusCode} ({(int) response.StatusCode})");
                }

                var pskResponse = JsonSerializer.Deserialize<PskResponse>(Encoding.UTF8.GetString(response.Payload));
                Console.WriteLine($"PSK: {pskResponse.PreSharedKey}\nFirmware Version: {pskResponse.GatewayFirmwareVersion}");
            }
        }

        public async Task<IEnumerable<IDevice>> ListAllDevices()
        {
            List<IDevice> devices = new List<IDevice>();

            using (var coapClient = _coapFactory.CreateClient())
            {
                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost(_host)
                    .WithDtlsTransportLayer(o =>
                        o.WithPreSharedKey(_clientId, _clientPsk))
                    .Build();

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token).ConfigureAwait(false);
                }

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("15001")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);

                if (response.StatusCode != CoapResponseStatusCode.Content)
                {
                    throw new Exception($"Error: {response.StatusCode} ({(int) response.StatusCode})");
                }

                var deviceIds = JsonSerializer.Deserialize<int[]>(Encoding.UTF8.GetString(response.Payload));

                foreach (var deviceId in deviceIds)
                {
                    var deviceInfo = await GetDeviceInfo(deviceId, coapClient);
                    var device = deviceInfo.ToDevice();
                    devices.Add(device);
                }
            }

            return devices;
        }

        public async Task<DeviceResponse> GetDeviceInfo(int deviceId)
        {
            using (var coapClient = _coapFactory.CreateClient())
            {
                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost(_host)
                    .WithDtlsTransportLayer(o =>
                        o.WithPreSharedKey(_clientId, _clientPsk))
                    .Build();

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token).ConfigureAwait(false);
                }

                return await GetDeviceInfo(deviceId, coapClient);
            }
        }

        private static async Task<DeviceResponse> GetDeviceInfo(int deviceId, ICoapClient coapClient)
        {
            var request = new CoapRequestBuilder()
                .WithMethod(CoapRequestMethod.Get)
                .WithPath($"15001/{deviceId}")
                .Build();

            var response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);

            if (response.StatusCode != CoapResponseStatusCode.Content)
            {
                throw new Exception($"Error: {response.StatusCode} ({(int) response.StatusCode})");
            }

            var deviceResponse = JsonSerializer.Deserialize<DeviceResponse>(Encoding.UTF8.GetString(response.Payload));

            return deviceResponse;
        }

        public async Task ToggleBulb(int deviceId, bool on = true)
        {
            using (var coapClient = _coapFactory.CreateClient())
            {
                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost(_host)
                    .WithDtlsTransportLayer(o =>
                        o.WithPreSharedKey(_clientId, _clientPsk))
                    .Build();

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token).ConfigureAwait(false);
                }

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Put)
                    .WithPath($"15001/{deviceId}")
                    .WithPayload($"{{\"3311\": [{{\"5850\": {(on ? 1 : 0)}}}]}}")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);

                if (response.StatusCode != CoapResponseStatusCode.Changed)
                {
                    throw new Exception($"Error: {response.StatusCode} ({(int) response.StatusCode})");
                }
            }
        }

        void PrintResponse(CoapResponse response)
        {
            Console.WriteLine("> RESPONSE");
            Console.WriteLine("   + Status         = " + response.StatusCode);
            Console.WriteLine("   + Status code    = " + (int) response.StatusCode);
            Console.WriteLine("   + Content format = " + response.Options.ContentFormat);
            Console.WriteLine("   + Max age        = " + response.Options.MaxAge);
            Console.WriteLine("   + Payload        = " + Encoding.UTF8.GetString(response.Payload));
            Console.WriteLine();
        }
    }
}