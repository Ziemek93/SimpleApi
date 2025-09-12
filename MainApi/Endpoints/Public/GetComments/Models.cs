namespace MainApi.Endpoints.Public.GetComments;

public class Request
{
    public int ArticleId { get; set; }
}

public class Response
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Login { get; set; }
    public int ArticleId { get; set; }
}