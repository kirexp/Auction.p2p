using Microsoft.AspNetCore.Builder;

namespace Aux.Client;

public class ClientMeta
{
    public Guid ClientId { get; set; }
    public int CentralNodePort { get; set; }
    public WebApplication Host { get; set; }
}