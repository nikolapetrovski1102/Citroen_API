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
using System.Text.Json.Nodes;
using System.Data.Common;
using static CitroenAPI.Models.Enums;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitroenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitroenApiController : ControllerBase
    {
        private readonly CitroenDbContext _context;

        public CitroenApiController(CitroenDbContext context)
        {
            _context = context;
        }

        HttpListener.ExtendedProtectionSelector ExtendedProtectionSelector { get; set; }
        static X509Certificate2 clientCertificate;
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

                clientCertificate = GetCert(absolutePath.ToString(), absolutePathKEY.ToString());

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

            var resp = Get().Result;
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(clientCertificate);
            using (var httpClient = new HttpClient(new HttpLoggingHandler(handler)))
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

           

                    TokenAuth tokenObject = JsonConvert.DeserializeObject<TokenAuth>(resp);
                    var content = new StringContent(jsonDate, Encoding.UTF8, "application/json");
                    AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Authorization", "Bearer "+tokenObject.access_token);

                    httpClient.DefaultRequestHeaders.Add("User-Agent", "YourUserAgent");
                    httpClient.DefaultRequestHeaders.Authorization = authHeader;
                    
                    var response = await httpClient.PostAsync("https://api-secure.forms.awsmpsa.com/formsv3/api/leads", content);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();


                    RootObject responseData=JsonConvert.DeserializeObject<RootObject>(responseBody);

                    Logs logs = new Logs();

                    foreach(Message msg in responseData.message)
                    {
                        await PostAsync(msg.leadData,msg.preferredContactMethod);
                        logs.GitId = msg.gitId;
                        logs.DispatchDate = msg.dispatchDate;
                        await AddLog(logs);
                    }



                    return responseBody.ToString();
                }
                catch (HttpRequestException e)
                {
                    return e.Message.ToString();
                }
            }

        }

        [HttpPost("AddLog")]
        public async Task AddLog (Logs logModel)
        {

            if (!CheckLogs(logModel))
            {
                try
                {
                    _context.Logs.Add(logModel);

                    await _context.SaveChangesAsync();
                }
                catch (DbException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                return;
            }

        }

        bool CheckLogs(Logs logsModel)
        {
            if (logsModel == null)
            {
                return false;
            }

            var res = _context.Logs.Where(model => model.GitId.Equals(logsModel.GitId)
                                                    && model.DispatchDate.Equals(logsModel.DispatchDate));

            if (res == null)
            {
                return false;
            }
            else
                return true;

        }

        [HttpPost("SalesForce")]
        public async Task PostAsync(LeadData data, PreferredContactMethodEnum prefered)
        {

            string url = "https://webto.salesforce.com/servlet/servlet.WebToLead?eencoding=UTF-8&orgId=00D7Q000004shjs" +
                "&salutation=" + data.customer.civility +
                "&first_name=" + data.customer.firstname +
                "&last_name=" + data.customer.lastname +
                "&email=" + data.customer.email +
                "&mobile=" + data.customer.personalMobilePhone +
                "&submit=submit&oid=00D7Q000004shjs&retURL=" +
                "&00N7Q00000KWlx2=" + data.requestType +
                "&lead_source=www.citroen.com.mk" +
                "&description=" + data.interestProduct.description+
                "&00N7Q00000KWlx7="+ prefered+
                "&00N7Q00000KWlxC=" +data.interestProduct.model +
                "&00N7Q00000KWlxH=TrebaInformacija"//Fali data;



            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Cookie", "BrowserId=asdasdasdasdasda");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());









            /* string xlink = "https://webto.salesforce.com/servlet/servlet.WebToLead?eencoding=UTF-8&orgId=00D7Q000004shjs&salutation=Mr.&first_name=" + data.customer.firstname
                 + "&last_name=" + data.customer.lastname +
                 "&email=" + data.customer.email +
                 "&phone=" + data.customer.personalMobilePhone +
                 "&submit=submit&oid=00D7Q000004shjs&retURL=";
             var client = new HttpClient(new HttpLoggingHandler());
             var request = new HttpRequestMessage(HttpMethod.Post, xlink);
             request.Headers.Add("Cookie", "BrowserId=asdasdasdasdasda");
             var response = await client.SendAsync(request);
             response.EnsureSuccessStatusCode();
             Console.WriteLine(await response.Content.ReadAsStringAsync());*/
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
