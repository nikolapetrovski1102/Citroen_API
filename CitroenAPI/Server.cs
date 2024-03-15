using System.Net;

namespace CitroenAPI
{
    public class Server
    {

        public void HandleRequest(IAsyncResult result)
        {

            HttpListener listener = (result.AsyncState as HttpListener)!;

            var context = listener.EndGetContext(result);

            using var wh = result.AsyncWaitHandle;

            var request = context.Request;

            // obtain httplistenerresponse
            using var response = context.Response;

            // extract the requested resource from url
            string message = "Welcome, server!";

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);

            response.ContentLength64 = bytes.Length;

            response.OutputStream.Write(bytes);

        }

    }
}
