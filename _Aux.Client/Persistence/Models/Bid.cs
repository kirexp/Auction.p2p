namespace Aux.Client.Persistence.Models;

public class Bid
{
    public string LotId { get; set; }
    public decimal Amount { get; set; }
    public string UserId { get; set; }
}