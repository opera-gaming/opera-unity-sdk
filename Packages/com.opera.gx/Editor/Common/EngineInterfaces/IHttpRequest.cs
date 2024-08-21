namespace Opera
{
    public interface IHttpRequest
    {
        string Post(string url, string postData, string contentType, string authorization = null, string accept = null);
    }
}
