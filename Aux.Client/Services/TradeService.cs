using AuxGenerated;
using Grpc.Core;

namespace Aux.Client.Services;

public class TradeService: AuxGenerated.Trade.TradeBase
{
    public override Task<InitiateTradeReply> InitiateTrade(InitiateTradeRequest request, ServerCallContext context)
    {
        return base.InitiateTrade(request, context);
    }
}