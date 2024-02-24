using Aux.Central.Node.Persistence;
using Aux.Central.Node.Persistence.Models;
using Grpc.Core;
using AuxGenerated;
using AuxGeneratedTrades;
using Google.Protobuf.Collections;

namespace Aux.Central.Node.Services;

public class TradesService : AuxGeneratedTrades.Trades.TradesBase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<TradesService> _logger;

    public TradesService(ISessionRepository sessionRepository, ILogger<TradesService> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }
    public override Task<StartLotReply> StartLot(StartLotRequest request, ServerCallContext context)
    {
        _sessionRepository.StartLot(new LotData
        {
            Title = request.Title,
            ClientId = Guid.Parse(request.ClientId),
            Id = Guid.Parse(request.Id),
        });
        _logger.LogInformation($"A new Lot was added to storage: {request.Title} by: {request.ClientId}");
        return Task.FromResult(new StartLotReply());
    }

    public override Task<GetListReply> GetList(GetListRequest request, ServerCallContext context)
    {
        var lotsFromDs = _sessionRepository.GetLots();
        var sessionData = _sessionRepository.GetSession();
        GetListReply reply = new ();
        reply.Lots.AddRange(lotsFromDs.Select(x=> new Lot
        {
            Id = x.Id.ToString(),
            Addr = sessionData[x.ClientId].IpAddress,
            Title = x.Title,
        }));

        return Task.FromResult(reply);
    }
}