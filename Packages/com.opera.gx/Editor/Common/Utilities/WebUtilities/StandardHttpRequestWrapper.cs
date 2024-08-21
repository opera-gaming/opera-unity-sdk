using System.IO;
using System.Net;

namespace Opera
{
    public class StandardHttpRequestWrapper : IHttpRequest
    {
        public string Post(string url, string postData, string contentType, string authorization = null, string accept = null)
        {
            string serverResponse;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = contentType;
            if (authorization != null) request.Headers.Add("Authorization", authorization);
            if (accept != null) request.Accept = "*/*";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(postData);
            } // end using


            var responseTokenRequest = (HttpWebResponse)request.GetResponse();
            using (var responseStream = responseTokenRequest.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    serverResponse = reader.ReadToEnd();
                } // end using
            } // end using

            return serverResponse;
        }
    }
}
