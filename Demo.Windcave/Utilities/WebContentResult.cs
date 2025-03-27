namespace Demo.Commerce.Utility.Web
{
    public class WebContentResult<T> where T : class
    {
        public string RawContent { get; set; } = string.Empty;
        public T Result { get; set; }
        public int HttpStatusCode { get; set; }
    }
}