using Grpc.Domain;
using Grpc.Net.Client;
using System;

namespace Grpc.ConsoleApp_NetFramework472
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, gRPC!");

            var channel = GrpcChannel.ForAddress("https://localhost:7187");
            var client = new Greeter.GreeterClient(channel);
            var reply = client.SayHello(
                              new HelloRequest { Name = "GreeterClient" });

            Console.WriteLine("Greeting: " + reply.Message);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
