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
using Org.BouncyCastle.Utilities;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Components;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitroenAPI.Controllers
{

 
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]

    public class CitroenApiController : ControllerBase
        {
            private static bool isLogCreated = false;
            private static readonly object logCreationLock = new object();
            IConfiguration configuration = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
            private readonly object postLock = new object();
            private readonly CitroenDbContext _context;
            private readonly IWebHostEnvironment _hostingEnvironment;
            private static string absolutePath;
            private static string absolutePathKEY;
            int callLimit = 0;
            public IConfiguration _configuration { get; set; }
            private EmailConfiguration emailConfig;
            static RootObject callLogs = new RootObject();
            private readonly ILogger<CitroenApiController> _logger;
            private IsRunningInstance _isRunningInstance;
            private readonly Emailer _emailer;
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();
            private readonly int dateMinus = -2;

            public CitroenApiController(CitroenDbContext context, IWebHostEnvironment hostingEnvironment, ILoggerFactory loggerFactory, ILogger<CitroenApiController> logger, IConfiguration configuration)
            {
                _isRunningInstance = new IsRunningInstance();
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

                dateMinus = int.Parse(_configuration["dateMinus"]);
                _emailer = new Emailer(emailConfig.SmtpServer, emailConfig.Port, emailConfig.UserName, emailConfig.Password);

                _logger.LogInformation("Constructor was executed");

                // Log user profile info
                _logger.LogInformation("User: " + Environment.UserName);
                _logger.LogInformation("User profile path: " + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }

            HttpListener.ExtendedProtectionSelector ExtendedProtectionSelector { get; set; }
            static X509Certificate2 clientCertificate;


        [HttpGet]
            private string Get()
            {
                Emailer emailer = new Emailer(emailConfig.SmtpServer, emailConfig.Port, emailConfig.UserName, emailConfig.Password);

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

                    // Check if files exist and log if not
                    if (!System.IO.File.Exists(absolutePath))
                    {
                        throw new FileNotFoundException("Certificate file not found", absolutePath);
                    }

                    if (!System.IO.File.Exists(absolutePathKEY))
                    {
                        throw new FileNotFoundException("Certificate key file not found", absolutePathKEY);
                    }

                    clientCertificate = GetCert(absolutePath.ToString(), absolutePathKEY.ToString());
                    _logger.LogInformation("Certificates passed");

                    var handler = new HttpClientHandler();
                    handler.ClientCertificates.Add(clientCertificate);

                    var loggingHandler = new HttpLoggingHandler(handler);
                    var client = new HttpClient(loggingHandler);

                    var requestUri = "https://api-secure.forms.awsmpsa.com/oauth/v2/token?client_id=5f7f179e7714a1005d204b43_2w88uv9394aok4g8gs0ccc4w4gwsskowck0gs0oo0sggw0kog0&client_secret=619ffmx8sn0g8ossso44wwok8scgoww00s8sogkw8w08cgc0wg&grant_type=password&username=ACMKPR&password=N9zTQ6v1";
                    var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                    client.DefaultRequestHeaders.Add("User-Agent", "MiddleApiCitroenMacedoniaPullData");

                    _logger.LogInformation("Created client and handler");
                    var response = client.Send(request);
                    _logger.LogInformation($"Response: {response}");
                    response.EnsureSuccessStatusCode();

                    string text = response.Content.ReadAsStringAsync().Result;
                    TokenAuth tokenObject = new TokenAuth();
                try
                {
                    tokenObject = JsonConvert.DeserializeObject<TokenAuth>(text);
                }
                catch (Exception ex)
                {
                    _logger.LogError("File not found: " + ex.Message);
                    emailer.SendEmail("Citroen Info - File not found", ex.ToString());
                    return "Error: " + ex.Message;
                }
                    return tokenObject.access_token;
                }
                catch (FileNotFoundException fileEx)
                {
                    _logger.LogError("File not found: " + fileEx.Message);
                    emailer.SendEmail("Citroen Info - File not found", fileEx.ToString());
                    return "Error: " + fileEx.Message;
                }
                catch (UnauthorizedAccessException authEx)
                {
                    _logger.LogError("Unauthorized access: " + authEx.Message);
                    emailer.SendEmail("Citroen Info - Unauthorized access", authEx.ToString());
                    return "Error: " + authEx.Message;
                }
                catch (Exception ex)
                {
                    emailer.SendEmail("Citroen Info - Get Method exception", ex.ToString());
                    _logger.LogError("Error was happening at get method " + ex.Message);
                    return "Error: " + ex.Message + " looking folder for: " + absolutePath;
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

            // Other methods (Post, PostAsync, AddLog, etc.) remain unchanged
        
    





    // POST api/<ValuesController>
    [HttpPost]
        private async Task<IActionResult> Post()
        {
            lock (postLock)
            {
                if (_isRunningInstance.getIsRunning())
                {
                    return Ok("There is one instance Working");
                }
                _isRunningInstance.SetIsRunning();
            }

            _isRunningInstance.SetIsRunning();
            _logger.LogInformation("--------------------------------------------------------------------------------");
            _logger.LogInformation("Post method started");
            _logger.LogInformation("--------------------------------------------------------------------------------");

            var handler = new HttpClientHandler();
            var resp = Get();
            var clientCertificate = GetCert(absolutePath.ToString(), absolutePathKEY.ToString());
            handler.ClientCertificates.Add(clientCertificate);

            using (var httpClient = new HttpClient(new HttpLoggingHandler(handler)))
            {
                try
                {
                    DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
                    DateTime sevenDays = date.AddDays(dateMinus);

                    var dateRange = new
                    {
                        startDate = sevenDays.ToString("yyyy-MM-ddT00:00:00.fffzzz"),
                        endDate = date.ToString("yyyy-MM-ddT23:59:ss.fffzzz")
                    };

                    string jsonDate = "";
                    TokenAuth tokenObject = new TokenAuth();

                    jsonDate = JsonConvert.SerializeObject(dateRange);

                    var content = new StringContent(jsonDate, Encoding.UTF8, "application/json");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api-secure.forms.awsmpsa.com/formsv3/api/leads")
                    {
                        Content = content
                    };
                    request.Headers.Add("User-Agent", "MiddleApiCitroenMacedoniaPullData");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", resp.Trim().ToString()) ;

                    var response = httpClient.Send(request);
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
                            if (msg.gitId == "MTcyNzE1OTAwNFZXOTFZ")
                            {
                                Console.WriteLine();
                            }
                            logs.GitId = msg.gitId;
                            logs.DispatchDate = msg.dispatchDate;
                            logs.CreatedDate = DateTime.Now;

                            bool inserted = await AddLog(logs);
                            if (inserted)
                            {
                               msg.leadData.gitId = msg.gitId;
                              await PostAsync(msg.leadData, msg.preferredContactMethod, logs);
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
                    lock (postLock)
                    {
                        _isRunningInstance.SetIsRunning();
                    }
                }
            }
        }


        [HttpGet("GetLeads")]
        public async Task<IActionResult> GetLeads()
        {
            lock (postLock)
            {
                if (_isRunningInstance.getIsRunning())
                {
                    _logger.LogInformation("There is one instance Working");
                    return Conflict("There is one instance Working");
                }

                _logger.LogInformation("Calling post method");
                var res = Post();

                _logger.LogInformation("Post method finished");
                isLogCreated = true;
                return Ok(res);
            }
        }

        [HttpPost("AddLog")]
        private async Task<bool> AddLog(Logs logModel)
        {
            if (CheckLogs(logModel))
            {
                return true;

            }
            else
            {
                return false;
            }

        }

        private List<Logs> gitIdLogs;

        bool CheckLogs(Logs logsModel)
        {

            if (logsModel == null)
            {
                return false;
            }

            DateTime twoDaysAgoStart = DateTime.Now.AddDays(dateMinus - 1).Date;

            DateTime now = DateTime.Now;
            if (gitIdLogs == null)
                gitIdLogs = _context.Logs
                    .Where(model => model.CreatedDate >= twoDaysAgoStart && model.CreatedDate <= now)
                    .ToList();

            bool res = true;

            if (gitIdLogs.Count == 0) return res;

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
        private async Task PostAsync(LeadData data, PreferredContactMethodEnum prefered, Logs logModel)
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
                        if (split1.Length > 1)
                        {
                            replace = split1[0] + split1[1];
                            model = "Model_" + replace;
                        }
                        else
                        {
                            model = "Model_" + replace;
                        }
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
                
                try
                {
                    logModel.Email = data.customer.email;
                    logModel.Phone = mobilePhone;
                    logModel.Name = data.customer.firstname;
                    logModel.FamilyName = data.customer.lastname;
                    logModel.Comments = commetns;
                    logModel.Consents = consents;
                    logModel.Model = model;
                    logModel.Dealer = dealers;
                    logModel.RequestType = requestType;
                    _context.Logs.Add(logModel);

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _emailer.SendEmail("Citroen Info - Post Method eception -", ex.ToString());
                    _logger.LogInformation("Error in AddLog " + ex.Message);
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
                    PostAsync(data, prefered, logModel);
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