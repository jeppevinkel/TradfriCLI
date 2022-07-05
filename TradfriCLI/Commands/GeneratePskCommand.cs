using System;
using System.Text.Json;
using System.Threading.Tasks;
using CommandLine;
using TradfriCLI.Entities;
using TradfriCLI.Interfaces;

namespace TradfriCLI.Commands
{
    [Verb("generate-psk", HelpText = "Generate a PSK for the given client id. Can only be done if the id doesn't already have an active PSK.")]
    public class GeneratePskCommand : ICommand
    {
        [Option('h', "host", Required = true, HelpText = "The address to the Ikea Trådfri Gateway.")]
        public string Host { get; set; }
        
        [Option('p', "psk", Required = true, HelpText = "The pre-shared key found underneath the gateway.")]
        public string Psk { get; set; }
        
        [Option('c', "client", Required = true, HelpText = "The client id to be used with the new PSK.")]
        public string ClientId { get; set; }
        
        public async Task Execute()
        {
            TradfriClient myClient = new TradfriClient(Host, null, ClientId);
            Psk psk;
            
            try
            {
                psk = await myClient.GenerateTradfriPskToken(Psk);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Serialize());
                return;
            }
            
            Console.WriteLine(JsonSerializer.Serialize(psk));
        }
    }
}