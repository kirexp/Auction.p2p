using Aux.Central.Node.Persistence;
using Aux.Central.Node.Persistence.Models;
using AuxGenerated;
using Grpc.Core;

namespace Aux.Central.Node.Services;

public class SessionService : AuxGenerated.Session.SessionBase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ISessionRepository sessionRepository, ILogger<SessionService> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }
    public override Task<NotifyConnectReply> NotifyConnect(NotifyConnectRequest request, ServerCallContext context)
    {
        _sessionRepository.AddSession(Guid.Parse(request.Id), new ClientSession
        {
            IpAddress = request.IpAddress
        });
        _logger.LogInformation($"A new client has popped up online. ID: {request.Id}, Addr: {request.IpAddress}");
        return Task.FromResult(new NotifyConnectReply
        {
            Time = $"{DateTime.UtcNow}"
        });
    }
}