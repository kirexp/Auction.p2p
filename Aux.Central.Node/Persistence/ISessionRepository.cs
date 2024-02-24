using Aux.Central.Node.Persistence.Models;

namespace Aux.Central.Node.Persistence;

public interface ISessionRepository
{
    void AddSession(Guid id, ClientSession session);
    void StartLot(LotData lotData);
    IEnumerable<LotData> GetLots();
    IDictionary<Guid, ClientSession> GetSession();
}