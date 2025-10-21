namespace Company.App.Cross.Common
{
    public class ResponseT<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }
        public IEnumerable<string>? ListErrors { get; set; }
    }
}
