// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using Aux.Client.Persistence;
using Aux.Client.Services;
using AuxGenerated;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using NotifyConnectRequest = AuxClientGenerated.NotifyConnectRequest;
using Session = AuxClientGenerated.Session;

namespace Aux.Client;

public class Program
{
    private static int[] _availablePorts = new[]
    {
        5002,
        5003,
        5004,
        5005,
        5006,
        5007,
        5008,
        5009,
        5010,
    };

    private static WebApplication? host;

    private static int _centralNodePort = 5001;

    private static Guid ClientId = Guid.NewGuid();

    public static async Task Main(string[] args)
    {
        var repository = new LotsRepository();
        var clientMeta = new ClientMeta
        {
            ClientId = ClientId,
            CentralNodePort = _centralNodePort,
        };
        var actions = new AuxActions(clientMeta, repository);
        foreach (var availablePort in _availablePorts)
        {
            if (IsPortAvailable(availablePort))
            {
                StartWebHost(availablePort, repository);
                break;
            }
        }

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("Stopping the server...");
            host?.Lifetime.StopApplication();
        };
        while (true)
        {
            string command = Console.ReadLine();
            switch (command)
            {
                case "quit":
                {
                    Environment.Exit(0);
                    break;
                }
                case "start-lot":
                {
                    var title = await actions.StartLot();
                    Console.WriteLine($"Lot: {title} has been created");
                    break;
                }

                case "list":
                {
                    var lots = await actions.GetList();
                    if (!lots.Any())
                    {
                        Console.WriteLine("No lots available for bidding");
                    }
                    foreach (var lot in lots)
                    {
                        Console.WriteLine($"[Id: {lot.Id} Lot: {lot.Title}; Addr: {lot.Addr}]");
                    }
                        
                    Console.WriteLine();
                    break;
                }

                case "bid":
                {
                    Console.WriteLine("Please specify lot ID");
                    var id = Console.ReadLine();
                    Console.WriteLine("good. now specify amount");
                    var l = Console.ReadLine();
                    if (decimal.TryParse(l, out var amount))
                    {
                        var lots = await actions.GetList();
                        var lotToBidOn = lots.FirstOrDefault(x => x.Id == id);
                        if (lotToBidOn is null)
                        {
                            Console.WriteLine("Lot is not found");
                            break;
                        }

                        await actions.Bid(id, amount, lotToBidOn.Addr);
                    }
                    break;
                }
            }
        }
    }
    
    private static bool IsPortAvailable(int port)
    {
        try
        {
            // Attempt to bind to the specified port
            using (var client = new TcpClient())
            {
                client.Connect(IPAddress.Loopback, port);
                return false; // Port is not available
            }
        }
        catch (SocketException)
        {
            // Port is available
            return true;
        }
    }
    
    private static async Task StartWebHost(int port, ILotsRepository lotsRepository)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls($"http://localhost:{port}");
        builder.Services.AddSingleton(lotsRepository);
        builder.Services.AddGrpc();
        // Add services to the container.
        builder.WebHost.ConfigureKestrel(options =>
        {
            // Setup a HTTP/2 endpoint without TLS.
            options.ListenLocalhost(port, o => o.Protocols =
                HttpProtocols.Http2);
        });


        host = builder.Build();
        host.MapGet("/",
            () =>
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
        host.MapGrpcService<TradeOutService>();
        await RegisterClient(port);
        await host.RunAsync();
    }
    
    private static async Task RegisterClient(int port)
    {
        try
        {
            GrpcChannel channel = GrpcChannel.ForAddress($"http://localhost:{_centralNodePort}");
            var client = new Session.SessionClient(channel);
            var result = await client.NotifyConnectAsync(new NotifyConnectRequest
            {
                IpAddress = $"grpc://127.0.0.1:{port}",
                Id = ClientId.ToString()
            });
            Console.WriteLine($"Client time: {result.Time}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not send a request to register client; Shutting down....");
        }
    }
}