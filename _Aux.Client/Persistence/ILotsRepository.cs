using Aux.Client.Persistence.Models;

namespace Aux.Client.Persistence;

public interface ILotsRepository
{
    void AddMyLot(string id, string title);
    void PlaceBid(Bid bid);
    Lot? GetLot(string lotId);
    decimal GetHighestBid(string lotId);
}