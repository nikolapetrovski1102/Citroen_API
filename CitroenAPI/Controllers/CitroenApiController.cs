using Microsoft.AspNetCore.Mvc;
using ReptilApp.Api;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CitroenAPI.Models;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Data.Common;
using static CitroenAPI.Models.Enums;
using CitroenAPI.Models.DbContextModels;
using CitroenAPI.Logger;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using System.ServiceProcess;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitroenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CitroenApiController : ControllerBase
    {
        private static bool isLogCreated = false;
        private static readonly object logCreationLock = new object();
        IConfiguration configuration = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        private readonly object postLock = new object();
        private readonly CitroenDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        string absolutePath;
        string absolutePathKEY;
        int callLimit = 0;
        public IConfiguration _configuration { get; set; }
        private EmailConfiguration emailConfig;
        static RootObject callLogs = new RootObject();
        private readonly ILogger<CitroenApiController> _logger;
        private static bool isRunning = false;
        private readonly Emailer _emailer;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public CitroenApiController(CitroenDbContext context, IWebHostEnvironment hostingEnvironment, ILoggerFactory loggerFactory, ILogger<CitroenApiController> logger, IConfiguration configuration)
        {
            lock (logCreationLock)
            {
                if (!isLogCreated)
                {
                    loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logs"));
                    isLogCreated = true;
                }
            }

            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _configuration = configuration;

            emailConfig = new EmailConfiguration
            {
                From = _configuration["EmailConfiguration:From"],
                UserName = _configuration["EmailConfiguration:Username"],
                Password = _configuration["EmailConfiguration:Password"],
                SmtpServer = _configuration["EmailConfiguration:SmtpServer"],
                Port = int.TryParse(_configuration["EmailConfiguration:Port"], out int port) ? port : 25
            };

            if (string.IsNullOrEmpty(emailConfig.From) || string.IsNullOrEmpty(emailConfig.UserName) ||
                string.IsNullOrEmpty(emailConfig.Password) || string.IsNullOrEmpty(emailConfig.SmtpServer))
            {
                _logger.LogError("Email configuration is missing required values.");
                throw new InvalidOperationException("Email configuration is not valid.");
            }

            _emailer = new Emailer(emailConfig.SmtpServer, emailConfig.Port, emailConfig.UserName, emailConfig.Password);

            _logger.LogInformation("Constructor was executed");
        }

        HttpListener.ExtendedProtectionSelector ExtendedProtectionSelector { get; set; }
        static X509Certificate2 clientCertificate;

        // GET: api/<ValuesController>
        [HttpGet]
        private async Task<string> Get()
        { 
            try
            {

                _logger.LogInformation("--------------------------------------------------------------------------------");
                _logger.LogInformation($"{nameof(Get)}");
                string certificateFilePath = @".\certificate\MZPDFMAP.cer";
                string certificatePassword = @".\certificate\MZPDFMAP.pk";

                certificateFilePath = certificateFilePath.Replace(".\\", "");
                certificatePassword = certificatePassword.Replace(".\\", "");

                string currentDirectory = Environment.CurrentDirectory;

                absolutePath = System.IO.Path.Combine(_hostingEnvironment.ContentRootPath, certificateFilePath);
                absolutePathKEY = System.IO.Path.Combine(_hostingEnvironment.ContentRootPath, certificatePassword);

                clientCertificate = GetCert(absolutePath.ToString(), absolutePathKEY.ToString());
                _logger.LogInformation("Certificates passed");
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(clientCertificate);

                var loggingHandler = new HttpLoggingHandler(handler);

                var client = new HttpClient(loggingHandler);
                client.DefaultRequestHeaders.Add("User-Agent", "MiddleApiCitroenMacedoniaPullData");
                _logger.LogInformation("Created client and handler");
                var requestUri = "https://api-secure.forms.awsmpsa.com/oauth/v2/token?client_id=5f7f179e7714a1005d204b43_2w88uv9394aok4g8gs0ccc4w4gwsskowck0gs0oo0sggw0kog0&client_secret=619ffmx8sn0g8ossso44wwok8scgoww00s8sogkw8w08cgc0wg&grant_type=password&username=ACMKPR&password=N9zTQ6v1";
                var response = await client.GetAsync(requestUri);
                _logger.LogInformation($"Response: {response}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _emailer.SendEmail("Citroen Info - Get Method eception -", ex.ToString());
                _logger.LogError("Error was happening at get method " + ex.Message);
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
        private async Task<IActionResult> Post(CancellationToken cancellationToken)
        {
            if (isRunning)
            {
                return Ok("There is one instance Working");
            }

            isRunning = true;
            _logger.LogInformation("--------------------------------------------------------------------------------");
            _logger.LogInformation("Post method started");
            _logger.LogInformation("--------------------------------------------------------------------------------");

            var handler = new HttpClientHandler();
            var resp = Get().Result;
            var clientCertificate = GetCert(absolutePath.ToString(), absolutePathKEY.ToString());
            handler.ClientCertificates.Add(clientCertificate);

            using (var httpClient = new HttpClient(new HttpLoggingHandler(handler)))
            {
                try
                {
                    DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
                    DateTime sevenDays = date.AddDays(-1);

                    var dateRange = new
                    {
                        startDate = sevenDays.ToString("yyyy-MM-ddT00:00:00.fffzzz"),
                        endDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
                    };

                    string jsonDate = JsonConvert.SerializeObject(dateRange);
                    TokenAuth tokenObject = JsonConvert.DeserializeObject<TokenAuth>(resp);

                    var content = new StringContent(jsonDate, Encoding.UTF8, "application/json");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api-secure.forms.awsmpsa.com/formsv3/api/leads")
                    {
                        Content = content
                    };
                    request.Headers.Add("User-Agent", "MiddleApiCitroenMacedoniaPullData");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.access_token.Trim());

                    cancellationToken.ThrowIfCancellationRequested();

                    var response = await httpClient.SendAsync(request, cancellationToken);
                    _logger.LogInformation("Post method Response: " + response.StatusCode);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogInformation("Post method no Leads");
                        _logger.LogInformation("Post method " + response.StatusCode.ToString());
                        return Ok("No new leads");
                    }

                    RootObject responseData;
                    try
                    {
                        responseData = JsonConvert.DeserializeObject<RootObject>(responseBody);
                    }
                    catch (Exception ex)
                    {
                        _emailer.SendEmail("Citroen Info - Post Method exception -", ex.ToString());
                        _logger.LogError(ex.ToString());
                        return StatusCode(500, "Error processing response data.");
                    }

                    Logs logs = new Logs();
                    _logger.LogInformation("Check if there are any changes");
                    _logger.LogInformation(responseData?.message?.Count.ToString());
                    _logger.LogInformation(callLogs?.message?.Count.ToString());
                    _logger.LogInformation("Check if there are any changes");

                    if ((responseData?.message?.Count != callLogs?.message?.Count || callLogs?.message == null)
                        && (callLogs?.message == null || !responseData.message[0].gitId.Equals(callLogs.message[0]?.gitId)))
                    {
                        foreach (Message msg in responseData.message)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

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
                    }

                    callLogs = responseData;
                    return Ok(response.StatusCode.ToString());
                }
                catch (HttpRequestException e)
                {
                    _emailer.SendEmail("Citroen Info - Post Method exception -", e.ToString());
                    _logger.LogInformation("--------------------------------------------------------------------------------");
                    _logger.LogError("Post method error " + e.Message);
                    _logger.LogInformation("--------------------------------------------------------------------------------");
                    return StatusCode(500, e.Message.ToString());
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Post method was canceled");
                    return StatusCode(500, "Post method was canceled");
                }
                finally
                {
                    isRunning = false;
                }
            }
        }


        [HttpGet("GetLeads")]
        public async Task<IActionResult> GetLeads()
        {
            try
            {
                lock (postLock)
                {
                    if (isRunning)
                    {
                        _logger.LogInformation("There is one instance Working");
                        StopPost();
                        return Conflict("There is one instance Working");
                    }

                    _logger.LogInformation("Calling post method");
                    var res = Post(_cts.Token);

                    if (res.Equals("Post method was canceled"))
                    {
                        _logger.LogInformation("Post method was canceled");
                    }
                    else
                    {
                        _logger.LogInformation($"{res}");
                    }

                    _logger.LogInformation("Post method finished");
                    isLogCreated = true;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                _emailer.SendEmail("Citroen Info - Post Method exception -", ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("StopPost")]
        private IActionResult StopPost()
        {
            if (isRunning)
            {
                _cts.Cancel();
                _logger.LogInformation("Cancellation requested for post method");
                isRunning = false;
                return Ok("Cancellation requested");
            }
            return Conflict("No post method is running");
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
                    _emailer.SendEmail("Citroen Info - Post Method eception -", ex.ToString());
                    _logger.LogInformation("Error in AddLog " + ex.Message);
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

            DateTime twoDaysAgoStart = DateTime.Now.AddDays(-2).Date;

            DateTime now = DateTime.Now;

            List<Logs> gitIdLogs = _context.Logs
                .Where(model => model.CreatedDate >= twoDaysAgoStart && model.CreatedDate <= now)
                .ToList();

            bool res = false;

            foreach (Logs log in gitIdLogs) 
            {
                if (logsModel.GitId.Equals(log.GitId))
                {
                    return false;
                }
                else
                {
                    res = true;
                }
            }

            return res;

        }

        [HttpPost("SalesForce")]
        private async Task PostAsync(LeadData data, PreferredContactMethodEnum prefered)
        {
            _logger.LogInformation("--------------------------------------------------------------------------------");
            _logger.LogInformation("Started sending leads to SF");
            StatusLeads sl = new StatusLeads();

            try
            {
                string salutation = data.customer.civility == null ? "--None--" : String.IsNullOrEmpty(Enums.GetEnumValue(data.customer.civility)) ? "--None-- " : Enums.GetEnumValue(data.customer.civility);
                string requestType = data.requestType == null ? "--None--" : String.IsNullOrEmpty(Enums.GetEnumValue(data.requestType)) ? "--None--" : Enums.GetEnumValue(data.requestType);
                string commetns = data.comments == null ? "" : String.IsNullOrEmpty(data.comments) ? "" : data.comments;
                string model = data.interestProduct == null ? "--None--" : String.IsNullOrEmpty(data.interestProduct.lcdv) ? "" : data.interestProduct.lcdv;
                string dealers = data.dealers.Count == 0 ? "" : String.IsNullOrEmpty(data.dealers[0].geoSiteCode) ? "" : data.dealers[0].geoSiteCode;
                string mobilePhone = data.customer.personalMobilePhone == null ? "" : String.IsNullOrEmpty(data.customer.personalMobilePhone) ? "" : data.customer.personalMobilePhone;
                string consents = String.Empty;

                CarModelsEnum carenum;
                if (!model.Equals("--None--") && model.Length > 0)
                {
                    var split = model.Split('(');
                    if (split.Length > 1)
                    {
                        var replace = model.Replace('(', '_');
                        replace = replace.Replace(')', '_');
                        var split1 = replace.Split(' ');
                        replace = split1[0] + split1[1];
                        model = "Model_" + replace;
                    }
                    else
                    {
                        model = "Model_" + model;
                    }


                    Enum.TryParse(model, out carenum);
                    model = Enums.GetEnumValue(carenum).ToString();
                }




                if (data.consents.Count > 0)
                {
                    foreach (var consent in data.consents)
                    {
                        if (consent.consentValue.Equals(true))
                        {
                            consents += Enums.GetEnumValue(consent.consentName).ToString() + ", ";
                        }
                    }

                    consents = consents.TrimEnd(',', ' ');

                }
                else
                {
                    consents = "";
                }

                string url = "https://webto.salesforce.com/servlet/servlet.WebToLead?eencoding=UTF-8&orgId=00D7Q000004shjs" +
                    "&salutation=" + salutation +
                    "&first_name=" + data.customer.firstname +
                    "&last_name=" + data.customer.lastname +
                    "&email=" + data.customer.email +
                    "&mobile=" + mobilePhone +
                    "&submit=submit&oid=00D7Q000004shjs&retURL=" +
                    "&00N7Q00000KWlx2=" + requestType +
                    "&lead_source=www.citroen.com.mk" +
                    "&description=" + commetns +
                    "&00N7Q00000KWlx7=" + consents +
                    "&00N7Q00000KWlxC=" + model +
                    "&00N7Q00000KWlxH=" + dealers;

                _logger.LogInformation("Lead URL: " + url);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Add("Cookie", "BrowserId=asdasdasdasdasda");
                var response = await client.SendAsync(request);
                _logger.LogInformation("Lead response: " + response.StatusCode);
                response.EnsureSuccessStatusCode();

                sl.GitId = data.gitId;
                sl.Status = 200;
                sl.SentDate = DateTime.Now;

                _context.StatusLeads.Add(sl);

                _context.SaveChanges();
                _logger.LogInformation("Lead logs saved");
                callLimit = 0;

            }
            catch (Exception ex)
            {
                _emailer.SendEmail("Citroen Info - Post SalesForce Method eception -", ex.ToString());
                _logger.LogInformation("Lead to sf method exception: " + ex.Message);
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

    }
}