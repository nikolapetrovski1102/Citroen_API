using Microsoft.AspNetCore.Mvc;
using ReptilApp.Api;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using CitroenAPI.Models;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Data.Common;
using static CitroenAPI.Models.Enums;
using CitroenAPI.Models.DbContextModels;
using CitroenAPI.Models;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitroenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitroenApiController : ControllerBase
    {
        private readonly CitroenDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        string absolutePath;
        string absolutePathKEY;
        int callLimit = 0;
        private Timer timer;

     
        public CitroenApiController(CitroenDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        HttpListener.ExtendedProtectionSelector ExtendedProtectionSelector { get; set; }
        static X509Certificate2 clientCertificate;
        // GET: api/<ValuesController>
        [HttpGet]
        private async Task<string> Get()
        {
            try
            {
                string certificateFilePath = @".\Certificate\MZPDFMAP.cer";
                string certificatePassword = @".\Certificate\MZPDFMAP.pk"; // If the certificate is password-protected

                certificateFilePath = certificateFilePath.Replace(".\\", "");
                certificatePassword = certificatePassword.Replace(".\\", "");

                string currentDirectory = Environment.CurrentDirectory;

                absolutePath = System.IO.Path.Combine(_hostingEnvironment.ContentRootPath, certificateFilePath);
                absolutePathKEY = System.IO.Path.Combine(_hostingEnvironment.ContentRootPath, certificatePassword);

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
                return ex.Message + " looking folder for: " + absolutePath;

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
           
            var handler = new HttpClientHandler();
            var resp = Get().Result;
            handler.ClientCertificates.Add(clientCertificate);
            using (var httpClient = new HttpClient(new HttpLoggingHandler(handler)))
            {
                try
                {
                    DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

                    DateTime sevenDays = date.AddDays(-3);

                    var dateRange = new
                    {
                        startDate = sevenDays.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                        endDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
                    };

                    string jsonDate = JsonConvert.SerializeObject(dateRange);

                    TokenAuth tokenObject = JsonConvert.DeserializeObject<TokenAuth>(resp);
                    var content = new StringContent(jsonDate, Encoding.UTF8, "application/json");
                    AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Authorization", "Bearer " + tokenObject.access_token);

                    httpClient.DefaultRequestHeaders.Add("User-Agent", "YourUserAgent");
                    httpClient.DefaultRequestHeaders.Authorization = authHeader;

                    var response = await httpClient.PostAsync("https://api-secure.forms.awsmpsa.com/formsv3/api/leads", content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.NotFound) 
                    {
                        return "No new leads";
                    }

                    RootObject responseData = JsonConvert.DeserializeObject<RootObject>(responseBody);

                    Logs logs = new Logs();

                    foreach (Message msg in responseData.message)
                    {
                        logs.GitId = msg.gitId;
                        logs.DispatchDate = msg.dispatchDate;
                        logs.CreatedDate = DateTime.Now;
                        bool inserted = await AddLog(logs);
                        if (inserted)
                        {
                            msg.leadData.gitId = msg.gitId;
                            await PostAsync(msg.leadData, msg.preferredContactMethod);
                        }

                    }

                    ApiCalls apiCalls = new ApiCalls();

                    DateTime dateTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

                    apiCalls.CallDateTime = dateTimeNow;
                    apiCalls.Status = response.StatusCode.ToString();

                    try
                    {
                        _context.ApiCalls.Add(apiCalls);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        apiCalls.Status = ex.Message;
                        try
                        {
                            _context.ApiCalls.Add(apiCalls);
                            await _context.SaveChangesAsync();
                        }
                        catch {  }
                    }
                   return response.StatusCode.ToString();
                }
                catch (HttpRequestException e)
                {
                    return e.Message.ToString();
                }
            }

        }

        [HttpPost("AddLog")]
        private async Task<bool> AddLog(Logs logModel)
        {

            if (CheckLogs(logModel))
            {
                try
                {
                    _context.Logs.Add(logModel);

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbException ex)
                {
                    throw new Exception(ex.Message);
                }
              
            }
            else
            {
                return false;
            }

        }

        bool CheckLogs(Logs logsModel)
        {
            if (logsModel == null)
            {
                return false;
            }

            Logs res = _context.Logs.FirstOrDefault(model => model.GitId.Equals(logsModel.GitId));

            if (res == null)
            {
                return true;
            }
            else
                return false;

        }

        [HttpPost("SalesForce")]
        private async Task PostAsync(LeadData data, PreferredContactMethodEnum prefered)
        {
            StatusLeads sl = new StatusLeads();
            try
            {
                string salutation = data.customer.civility == null ? "--None--" : String.IsNullOrEmpty(Enums.GetEnumValue(data.customer.civility)) ? "--None-- " : Enums.GetEnumValue(data.customer.civility);
                string requestType = data.requestType == null ? "" : String.IsNullOrEmpty(Enums.GetEnumValue(data.requestType)) ? "" : Enums.GetEnumValue(data.requestType);
                string description = data.interestProduct == null ? "--None--" : String.IsNullOrEmpty(data.interestProduct.description) ? "no desctiption defined" : data.interestProduct.description;
                string model = data.interestProduct == null ? "--None--" : String.IsNullOrEmpty(data.interestProduct.model) ? "no model defined" : data.interestProduct.model;
                string dealers = data.dealers.Count == 0 ? "--None--" : String.IsNullOrEmpty(data.dealers[0].geoSiteCode) ? "no dealer defined" : data.dealers[0].geoSiteCode;
                string mobilePhone = data.customer.personalMobilePhone == null ? "" : String.IsNullOrEmpty(data.customer.personalMobilePhone) ? "" : data.customer.personalMobilePhone;

                string url = "https://webto.salesforce.com/servlet/servlet.WebToLead?eencoding=UTF-8&orgId=00D7Q000004shjs" +
                    "&salutation=" + salutation +
                    "&first_name=" + data.customer.firstname +
                    "&last_name=" + data.customer.lastname +
                    "&email=" + data.customer.email +
                    "&mobile=" + mobilePhone +
                    "&submit=submit&oid=00D7Q000004shjs&retURL=" +
                    "&00N7Q00000KWlx2=" + requestType +
                    "&lead_source=www.citroen.com.mk" +
                    "&description=" + description +
                    "&00N7Q00000KWlx7=" + prefered +
                    "&00N7Q00000KWlxC=" + model +
                    "&00N7Q00000KWlxH=" + dealers;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Add("Cookie", "BrowserId=asdasdasdasdasda");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                sl.GitId = data.gitId;
                sl.Status = 200;
                sl.SentDate = DateTime.Now;

                _context.StatusLeads.Add(sl);

                _context.SaveChanges();

                callLimit = 0;

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && callLimit < 3 && ex.InnerException.Message.Contains("No such host is known."))
                {
                    callLimit++;
                    PostAsync(data, prefered);
                }

                sl.GitId = data.gitId;
                sl.Status = 500;
                sl.SentDate = DateTime.Now;

                _context.StatusLeads.Add(sl);

                _context.SaveChanges();

            }
        }

        //[HttpPost]
        //public async Task CallFunction()
        //{
        //    if (timer != null)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        DateTime now = DateTime.Now;
        //        DateTime nextExecution = now.AddMinutes(1);
        //        TimeSpan delay = nextExecution - now;

        //        int delayMilliseconds = (int)delay.TotalMilliseconds;

        //        await Task.Delay(delayMilliseconds);

        //        var res = "";
        //        ApiCalls apiCalls = new ApiCalls();

        //        try
        //        {
        //            await Post();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.Write(ex.Message);
        //            res = HttpStatusCode.InternalServerError.ToString();
        //        }

        //        apiCalls.CallDateTime = DateTime.Now;
        //        apiCalls.Status = res;

        //        try
        //        {
        //            _context.ApiCalls.Add(apiCalls);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.Write(ex.Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error setting up timer: " + ex.Message);
        //    }

        //}
    }
}
