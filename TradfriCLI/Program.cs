using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CoAPnet;
using CoAPnet.Client;
using CoAPnet.Extensions.DTLS;
using CoAPnet.Logging;
using CommandLine;
using TradfriCLI.Commands;
using TradfriCLI.Entities;
using TradfriCLI.Interfaces;

namespace TradfriCLI
{
    static class Program
    {
        private static string tradfriHost = "192.168.0.249";
        private static string generatedPsk = "txuwwfCPiZYt7kkM";

        static async Task Main(string[] args)
        {
            // Console.WriteLine(string.Join(' ', args));

            await Parser.Default.ParseArguments<DevicesCommand>(args)
                .WithParsedAsync(t => t.Execute());

            if (args[0] == "-devices")
            {
                TradfriClient myClient = new TradfriClient(tradfriHost, "1DykL556UvLOf7dM", "CS");
                var devices = await myClient.ListAllDevices();
                
                Console.WriteLine(JsonSerializer.Serialize((IEnumerable<object>)devices, new JsonSerializerOptions()
                {
                    // WriteIndented = true,
                }));
            }
            
            // TradfriClient myClient = new TradfriClient(tradfriHost, "1DykL556UvLOf7dM", "CS");
            // await myClient.GenerateTradfriPskToken("yi9h2gknckaKIAXx");
            // var devices = await myClient.ListAllDevices();

            // Console.WriteLine(JsonSerializer.Serialize((IEnumerable<object>)devices, new JsonSerializerOptions()
            // {
            //     WriteIndented = true,
            // }));
        }

        static async Task RequestInParallelTasks()
        {
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            const int TaskCount = 100;

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("coap.me")
                    .Build();

                await coapClient.ConnectAsync(connectOptions, CancellationToken.None).ConfigureAwait(false);

                var tasks = new Task[TaskCount];

                for (var i = 0; i < TaskCount; i++)
                {
                    tasks[i] = Task.Run(async () =>
                    {
                        var request = new CoapRequestBuilder()
                            .WithMethod(CoapRequestMethod.Get)
                            .WithPath("separate")
                            .Build();

                        var response = await coapClient.RequestAsync(request, CancellationToken.None);
                        //PrintResponse(response);

                        Console.WriteLine("Received response.");
                    });
                }

                await Task.WhenAll(tasks);
            }
        }

        static async Task GetStatusFromTradfriGateway()
        {
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using var coapClient = coapFactory.CreateClient();

            Console.WriteLine("< CONNECTING...");

            var connectOptions = new CoapClientConnectOptionsBuilder()
                .WithHost(tradfriHost)
                //.WithHost("127.0.0.1")
                .WithDtlsTransportLayer(o =>
                    o.WithPreSharedKey("Client_identity", generatedPsk))
                .Build();

            using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token);
            }

            var request = new CoapRequestBuilder()
                .WithMethod(CoapRequestMethod.Get)
                .WithPath("15001")
                .Build();

            using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                var response = await coapClient.RequestAsync(request, cancellationTokenSource.Token);
                PrintResponse(response);
            }
        }

        static async Task GenerateTradfriPskToken()
        {
            // Generate new PSK Token.

            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost(tradfriHost)
                    .WithDtlsTransportLayer(o =>
                        o.WithPreSharedKey("Client_identity", "yi9h2gknckaKIAXx"))
                    .Build();

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token).ConfigureAwait(false);
                }

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Post)
                    .WithPath("15011/9063")
                    .WithPayload("{\"9090\":\"WH\"}")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);
            }
        }

        static void PrintResponse(CoapResponse response)
        {
            Console.WriteLine("> RESPONSE");
            Console.WriteLine("   + Status         = " + response.StatusCode);
            Console.WriteLine("   + Status code    = " + (int) response.StatusCode);
            Console.WriteLine("   + Content format = " + response.Options.ContentFormat);
            Console.WriteLine("   + Max age        = " + response.Options.MaxAge);
            Console.WriteLine("   + E tag          = " + ByteArrayToString(response.Options.ETag));
            Console.WriteLine("   + Payload        = " + Encoding.UTF8.GetString(response.Payload));
            Console.WriteLine();
        }

        static string ByteArrayToString(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return string.Empty;
            }

            var hex = new StringBuilder(buffer.Length * 2);
            hex.Append("0x");

            foreach (var @byte in buffer)
            {
                hex.AppendFormat("{0:x2}", @byte);
            }

            return hex.ToString();
        }

        static async Task SendRequestsToCoapMe()
        {
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("coap.me")
                    .Build();

                await coapClient.ConnectAsync(connectOptions, CancellationToken.None).ConfigureAwait(false);

                // separate

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("separate")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // hello

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("hello")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // broken

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("broken")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // secret

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("secret")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // large-create

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("large-create")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // location1/location2/location3

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("location1/location2/location3")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // large

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("large")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        static async Task ObserveTradfriLamp()
        {
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost(tradfriHost)
                    .WithDtlsTransportLayer(o =>
                        o.WithPreSharedKey("WH", generatedPsk))
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
                PrintResponse(response);

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("15001/65541")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Put)
                    .WithPath("15001/65541")
                    .WithPayload("{\"3311\": [{\"5850\": 1}]}")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);

                var observeOptions = new CoapObserveOptionsBuilder()
                    .WithPath("15001/65541")
                    .WithResponseHandler(new ResponseHandler())
                    .Build();

                var observeResponse =
                    await coapClient.ObserveAsync(observeOptions, CancellationToken.None).ConfigureAwait(false);

                PrintResponse(observeResponse.Response);

                Console.WriteLine("Observed messages for lamp!");

                Console.WriteLine("Press any key to unobserve.");
                Console.ReadLine();

                await coapClient.StopObservationAsync(observeResponse, CancellationToken.None).ConfigureAwait(false);
            }
        }

        class ResponseHandler :
            ICoapResponseHandler
        {
            public Task HandleResponseAsync(HandleResponseContext context)
            {
                Console.WriteLine("> RECEIVED OBSERVED RESOURCE");
                Console.WriteLine("    + Sequence number = " + context.SequenceNumber);
                PrintResponse(context.Response);
                return Task.CompletedTask;
            }
        }

        public static string Serialize(this Exception e)
        {
            var error = new Dictionary<string, string>
            {
                {"Type", e.GetType().ToString()},
                {"Message", e.Message},
                {"StackTrace", e.StackTrace},
            };
            
            foreach (DictionaryEntry data in e.Data)
                error.Add(data.Key.ToString(), data.Value.ToString());

            string json = JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return json;
        }
    }
}