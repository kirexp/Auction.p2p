using Aux.Client.Persistence;
using AuxClientTGenerated;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;

namespace Aux.Client;

public class AuxActions
{
    private readonly LotsRepository _lotsRepository;
    private readonly string _clientId;
    private readonly int _centralNodePort;
    private readonly WebApplication _host;

    public AuxActions(ClientMeta meta, LotsRepository lotsRepository)
    {
        _lotsRepository = lotsRepository;
        _clientId = meta.ClientId.ToString();
        _centralNodePort = meta.CentralNodePort;
        _host = meta.Host;
    }

    public async Task<string> StartLot()
    {
        GrpcChannel channel = GrpcChannel.ForAddress($"http://localhost:{_centralNodePort}");
        var client = new AuxClientTGenerated.Trades.TradesClient(channel);
        var title = $"Title-{Guid.NewGuid()}";
        var id = Guid.NewGuid().ToString();
        var result = await client.StartLotAsync(new StartLotRequest
        {
            Id = id,
            Title = title,
            ClientId = _clientId
        });

        _lotsRepository.AddMyLot(id, title);
        
        return title;
    }

    public async Task Bid(string id, decimal amount, string addr)
    {
        var uri = new Uri(addr);
        //check if bit is not related to this client's lot; 
        var myLot = _lotsRepository.GetLot(id);
        if (myLot is not null)
        {
            Console.WriteLine("Attempt to bid on current client's LOT");
            return;
        }
        GrpcChannel channel = GrpcChannel.ForAddress($"http://localhost:{uri.Port}");
        var client = new AuxClientTGenerated.TradeOut.TradeOutClient(channel);
        var result = client.Bid(new AuxClientTGenerated.BidRequest
        {
            LotId = id,
            Amount = (double)amount,
            SessionId = _clientId
        });
        Console.WriteLine(result.Message);
    }
    
    public async Task<IEnumerable<Lot>> GetList()
    {
        GrpcChannel channel = GrpcChannel.ForAddress($"http://localhost:{_centralNodePort}");
        var client = new AuxClientTGenerated.Trades.TradesClient(channel);
        var result = client.GetList(new GetListRequest());
        return result.Lots;
    }
}