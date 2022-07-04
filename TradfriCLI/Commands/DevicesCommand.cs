using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CommandLine;
using TradfriCLI.Interfaces;

namespace TradfriCLI.Commands
{
    [Verb("devices", HelpText = "Get an output of all devices connected to the gateway.")]
    public class DevicesCommand : ICommand
    {
        [Option('h', "host", Required = true, HelpText = "The address to the Ikea Trådfri Gateway.")]
        public string Host { get; set; }
        
        [Option('p', "psk", Required = true, HelpText = "The pre-shared key associated with the client id.")]
        public string Psk { get; set; }
        
        [Option('c', "client", Required = true, HelpText = "The client id associated with the psk.")]
        public string ClientId { get; set; }
        
        public async Task Execute()
        {
            TradfriClient myClient = new TradfriClient(Host, Psk, ClientId);
            IEnumerable<IDevice> devices;
            try
            {
                devices = await myClient.ListAllDevices();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Serialize());
                return;
            }
            
            
            Console.WriteLine(JsonSerializer.Serialize((IEnumerable<object>)devices));
        }
    }
}