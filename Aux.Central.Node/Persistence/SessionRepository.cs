using Aux.Central.Node.Persistence.Models;

namespace Aux.Central.Node.Persistence;

public class SessionRepository : ISessionRepository
{
    private IDictionary<Guid, ClientSession> _sessions = new Dictionary<Guid, ClientSession>();
    private List<LotData> _lots = new List<LotData>();

    public void AddSession(Guid id, ClientSession session)
    {
        _sessions.Add(id, session);
    }

    public void StartLot(LotData lotData)
    {
        _lots.Add(lotData);
    }

    public IEnumerable<LotData> GetLots()
    {
        return _lots;
    }

    public IDictionary<Guid, ClientSession> GetSession()
    {
        return _sessions;
    }
}