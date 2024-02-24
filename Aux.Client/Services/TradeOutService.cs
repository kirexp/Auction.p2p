using Aux.Client.Persistence;
using Aux.Client.Persistence.Models;
using AuxGenerated;
using Grpc.Core;

namespace Aux.Client.Services;

public class TradeOutService : AuxGenerated.TradeOut.TradeOutBase
{
    private readonly ILotsRepository _lotsRepository;

    public TradeOutService(ILotsRepository lotsRepository)
    {
        _lotsRepository = lotsRepository;
    }
    public override Task<BidReply> Bid(BidRequest request, ServerCallContext context)
    {
        var potentialLot = _lotsRepository.GetLot(request.LotId);

        if (potentialLot is not null)
        {
            var highestBid = _lotsRepository.GetHighestBid(request.LotId);
            var amt = new decimal(request.Amount);
            if (highestBid < amt)
            {
                _lotsRepository.PlaceBid(new Bid
                {
                    //hack
                    Amount = amt,
                    LotId = request.LotId,
                    UserId = request.SessionId
                });
                var message =
                    $"Bid has been placed successfully on lot: {potentialLot.Title}, Amount: {request.Amount}, Previous bid was: {highestBid}";
                Console.WriteLine();
                return Task.FromResult(new BidReply
                {
                    IsSuccess = true,
                    Message = message
                });
            }
            else
            {
                var message = "Could not place bid, bid was lower then highest bid.";
                Console.WriteLine(message);  
                return Task.FromResult(new BidReply
                {
                    IsSuccess = false,
                    Message = message
                });
            }
        }
        return Task.FromResult(new BidReply
        {
            IsSuccess = false,
            Message = "Could not find bid"
        });
    }
}