using Grpc.Domain;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace Grpc.ConsoleApp_NetFramework472
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, gRPC!");

            var channel = GrpcChannel.ForAddress("https://localhost:7187");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });

            Console.WriteLine("Greeting: " + reply.Message);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
