namespace MainApi.Models
{
    public class ResponseObject<O>
    {
        public O? Object { get; init; }
        public string Message { get; init; }
        public ResponseObject(O? resObject, string resMessage = "")
        {
            Object = resObject;
            Message = resMessage;
        }
    }
}
