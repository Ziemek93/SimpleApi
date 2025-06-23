namespace UsersInteractions.Infrastructure.Options;

public class MessageConsumerOptions
{
    public string Queue { get; set; }
    public string Exchange { get; set; }
    public string ExchangeType { get; set; }
    public string RoutingKey { get; set; }
}

