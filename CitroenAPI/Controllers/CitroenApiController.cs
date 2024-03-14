using Microsoft.AspNetCore.Mvc;
using ReptilApp.Api;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitroenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitroenApiController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<string> Get()
        {
            // Load client certificate from file
            string certificateFilePath = "C:/Users/nikpe/source/repos/CitroenAPI/CitroenAPI/Certificate/MZPDFMAP.cer";
            string certificatePassword = "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC+xzFNk8oH/E1Z\r\nmVLw1JDrRsKDbR0sGszB6jzq+MWw6MjZonLnUHLxKDCPGkItu6d6x8/vukUTd5U+\r\nQkzLMKtHj2k6TQkYhveIieXfilt2IdU5w6x2XbFLaDhE053UygZwHiD57f+614zP\r\nU186wh0FJx/lBKzj2tePp85W13JeU/+FHNSbYkaZ1Ew5E1yDHqG7FzmyVgLJl4F5\r\na5h6aFArMpHL6iC16kC82TlKnSlBfWKjDf5cUbk0Ucd7PZgEoShiUaZc/G9kwm4b\r\nj+/zE7mJkgILrSQbZt8dg33K6E6u5pIdq5+63T2lYFMC3pqil3PFGVdQJ7ZGgU0g\r\nW6QsSENzAgMBAAECggEAOBqKJD9HFawb7zKI0qwgZiBsCYxoHNVQy+IfwhYgxRLg\r\nJiiA1Aezlbn09dCKD7r02MW0H9LOh5gOOr3yqXqJlETXdD69Ywol93BeOqwMk6QF\r\nebRSnoiVIBDpI0x8SNyUohoqJnXYU9EZ5sqk4bm+IQrNdWM3mNZ9uBBoPN+lEZnu\r\nHb9Sk3SspcB5wbl62jHOSCjkjk0SCaOQEgutypApbLBWMdQSw4X0otyKvhmJZrg1\r\n4yKhGJEu58mC5cEKuRH7VqJvjRzjHwcYuP4bc3EFrmeggVONhTpsJRX6NzFJfOL5\r\n9f7P1jbqSXzgjx88MxmDlM9E/U4+WtN+8yh5qxv9AQKBgQDmYI0DXxnsHJtMwF5s\r\nvKDjLNAPfyS2V2XXblfPeHEqCPWpbpXBUocBbZt0IQKNp8v/6emH7nSL867v4b/9\r\nk43cq1ohyPbDTGHshxKtiKemAjQP4mNlBACaFuP+H5ZpVZw+rBHegMVzUG1yy0kp\r\nPJrPZciZ9OnYzezytE19aDv+gQKBgQDT/ybJIsogF5gVPUofKNeA3uptipXU9SQ2\r\na+LJeCgcET0zkdwP6LzdTEbfyTu4zk+1e+NwqWukYvd4pn7OQaxBYsG4TrurpQ1z\r\nssNcPlfssog2jiFr+8/SxgODsRCP2Etqy5urdL1rrEtg9E8XwurY6jdHdrG1jiBR\r\nP41ZoZwv8wKBgQCT+91ZFxPduZqAuOluy9RFWZnk+nUotAd1VSoO2X1H1S2IwsJr\r\nxol2f2PmOvYa7Hh8UVNzv9cZt1TePpNHXis4XtGs9hyc40nb2ABFfIzEdJHgyjmy\r\nv9lrId0edkf2LcoJ8BoiZXFwW2+S18aNOKLxKp6rVsmIPO9CEFPehMBVAQKBgQC+\r\nb95voIjh+0/rBHupMg8k/Rqp/GxMOUqmeJPpV77wN6w6vzRoNjIyuWqRbTvw76Q6\r\n62eMtSS1LxIPl8Ehl20d75EF9/QuZL6IyHUmT/q77kTefR8Y2cP/G1Hc4xp1nV9i\r\nODVG+D+Qkd3E4rKKLda5tOyjjcRly57MXcKkWcDscQKBgQCo0p9bEf4d86AcdxCo\r\n5sPmGy468t3Z5QQi7ndWIfLQ/j4z+xRjGDLUfcVqA6KPHn+wVN2cFE6PxsnmJYJY\r\nsozDsBjcFLIjHKSc32GmmrBcAwTJbfxiA/QZ3WpxVth/hmaIku7IreQcPbTT+4U+\r\nzkjirOGl2vCSA0zVvEklvDNp0Q=="; // If the certificate is password-protected
            X509Certificate2 clientCertificate = new X509Certificate2(certificateFilePath, certificatePassword);

            // Create HttpClientHandler with client certificate
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(clientCertificate);

            // Optionally, you can add other handlers such as logging
            var loggingHandler = new HttpLoggingHandler(handler);

            // Create HttpClient with configured handler
            var client = new HttpClient(loggingHandler);
            client.DefaultRequestHeaders.Add("User-Agent", "YourUserAgent");

            // Make the request
            var requestUri = "https://api-secure.forms.awsmpsa.com/oauth/v2/token?client_id=5f7f179e7714a1005d204b43_2w88uv9394aok4g8gs0ccc4w4gwsskowck0gs0oo0sggw0kog0&client_secret=619ffmx8sn0g8ossso44wwok8scgoww00s8sogkw8w08cgc0wg&grant_type=password&username=ACMKPR&password=N9zTQ6v1";
            var response = await client.GetAsync(requestUri);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Read and return the response content
            return await response.Content.ReadAsStringAsync();
        }

        private RSA LoadPrivateKey(string privateKeyFilePath)
        {
            try
            {
                byte[] privateKeyBytes = System.IO.File.ReadAllBytes(privateKeyFilePath);
                RSA rsa = RSA.Create();

                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                return rsa;
            }
            catch (Exception ex) {
                return null;
            }
        }


        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
