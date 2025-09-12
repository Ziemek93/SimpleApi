namespace MainApi.Options;

public class InteractionsApiOptions
{
    public string Base { get; set; } = string.Empty;
    public Uris Paths { get; set; }

    public class Uris
    {
        public string Comments { get; set; } = string.Empty;
    }
}