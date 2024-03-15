using Microsoft.AspNetCore.Mvc;
using ReptilApp.Api;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;

using CitroenAPI.Models;
using System.Text.Json;
using System.Text;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;
using System.Net;
using static System.Net.HttpListener;
using Newtonsoft.Json;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitroenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitroenApiController : ControllerBase
    {

        HttpListener.ExtendedProtectionSelector ExtendedProtectionSelector { get; set; }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<string> Get()
        {
            try
            {
                string certificateFilePath = @".\Certificate\MZPDFMAP.cer";
                string certificatePassword = @".\Certificate\MZPDFMAP.pk"; // If the certificate is password-protected

                string currentDirectory = Environment.CurrentDirectory;

                string absolutePath = System.IO.Path.Combine(currentDirectory, certificateFilePath);
                string absolutePathKEY= System.IO.Path.Combine(currentDirectory, certificatePassword);

                X509Certificate2 clientCertificate = GetCert(absolutePath.ToString(), absolutePathKEY.ToString());

                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(clientCertificate);

                var loggingHandler = new HttpLoggingHandler(handler);

                var client = new HttpClient(loggingHandler);
                client.DefaultRequestHeaders.Add("User-Agent", "YourUserAgent");

                var requestUri = "https://api-secure.forms.awsmpsa.com/oauth/v2/token?client_id=5f7f179e7714a1005d204b43_2w88uv9394aok4g8gs0ccc4w4gwsskowck0gs0oo0sggw0kog0&client_secret=619ffmx8sn0g8ossso44wwok8scgoww00s8sogkw8w08cgc0wg&grant_type=password&username=ACMKPR&password=N9zTQ6v1";
                var response = await client.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return null;

            }
        }
      
        private X509Certificate2 GetCert(string certPath, string keyPath)
        {
            X509Certificate2 cert = new X509Certificate2(certPath);
            StreamReader reader = new StreamReader(keyPath);
            PemReader pemReader = new PemReader(reader);
            RsaPrivateCrtKeyParameters keyPair = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();
            RSA rsa = DotNetUtilities.ToRSA(keyPair);
            cert = cert.CopyWithPrivateKey(rsa);
            return new X509Certificate2(cert.Export(X509ContentType.Pfx));
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<string> Post()
        {
            //https://api-secure.forms.awsmpsa.com/formsv3/api/leads

            using (var httpClient = new HttpClient())
            {
                try
                {
                    DateTime date = DateTime.Now;

                    DateTime sevenDays = date.AddDays(-7);

                    var dateRange = new
                    {
                        startDate = sevenDays.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                        endDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
                    };

                    string jsonDate = JsonConvert.SerializeObject(dateRange);

                    var resp = new CitroenApiController().Get().Result;

                    TokenAuth tokenObject = JsonConvert.DeserializeObject<TokenAuth>(resp);
                    var content = new StringContent(jsonDate, Encoding.UTF8, "application/json");
                    AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", tokenObject.access_token);

                    httpClient.DefaultRequestHeaders.Add("User-Agent", "YourUserAgent");
                    httpClient.DefaultRequestHeaders.Authorization = authHeader;
                    
                    var response = await httpClient.PostAsync("https://api-secure.forms.awsmpsa.com/formsv3/api/leads", content);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody.ToString();
                }
                catch (HttpRequestException e)
                {
                    return e.Message.ToString();
                }
            }

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
