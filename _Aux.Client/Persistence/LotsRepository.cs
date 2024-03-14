using Aux.Client.Persistence.Models;

namespace Aux.Client.Persistence;

public class LotsRepository : ILotsRepository
{
    private List<Lot> MyLots = new List<Lot>();
    private List<Bid> BidsOnMyLots = new List<Bid>();

    public void AddMyLot(string id, string title)
    {
        MyLots.Add(new Lot
        {
            Id = id,
            Title = title
        });
    }

    public void PlaceBid(Bid bid)
    {
        BidsOnMyLots.Add(bid);
    }

    public Lot? GetLot(string lotId) => MyLots.FirstOrDefault(x => x.Id == lotId);

    public decimal GetHighestBid(string lotId)
    {
        var bidsOnLot = BidsOnMyLots.Where(x => x.LotId == lotId);
        if (bidsOnLot.Any())
        {
            return bidsOnLot.Max(x => x.Amount);
        }

        return 0;
    }
}