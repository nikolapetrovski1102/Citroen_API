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
using Org.BouncyCastle.Asn1.IsisMtt.Ocsp;
using System.IO;
using Org.BouncyCastle.Crypto;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ConstrainedExecution;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitroenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitroenApiController : ControllerBase
    {
        private readonly CitroenDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private static int lastCount;
        string absolutePath;
        string absolutePathKEY;
        int callLimit = 0;
        private Timer timer;
        private ILogger<CitroenApiController> _logger;
     
        public CitroenApiController(CitroenDbContext context, IWebHostEnvironment hostingEnvironment,ILoggerFactory loggerFactory, ILogger<CitroenApiController> logger)
        {
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logs"));
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
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
                string certificateFilePath = @".\Certificate\MZPDFMAP.cer";
                string certificatePassword = @".\Certificate\MZPDFMAP.pk"; // If the certificate is password-protected
                certificateFilePath = certificateFilePath.Replace(".\\", "");
                certificatePassword = certificatePassword.Replace(".\\", "");
              

                string currentDirectory = Environment.CurrentDirectory;

                absolutePath = System.IO.Path.Combine(_hostingEnvironment.ContentRootPath, certificateFilePath);
                absolutePathKEY = System.IO.Path.Combine(_hostingEnvironment.ContentRootPath, certificatePassword);
              
                var certificate =await GetCert(absolutePath.ToString(), absolutePathKEY.ToString());
                _logger.LogInformation("Certificates passed");
                var handler = new HttpClientHandler();
                _logger.LogInformation("Creating hnadler successs");
                _logger.LogInformation("Clientcertificate: "+ certificate.ToString());
                handler.ClientCertificates.Add(certificate);
                _logger.LogInformation("Add Certificate");
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
                _logger.LogError("Error was happening at get method " + ex.Message);
                return ex.Message + " looking folder for: " + absolutePath;

            }
        }

        private async Task<X509Certificate2> GetCert(string certPath, string keyPath)
        {//h:\root\home\cyberlink-001\www\citroenapi\certificate\
            //h:\root\home\cyberlink-001\www\citroenapi\logs\
            /* try
             {
                   X509Certificate2 cert = new X509Certificate2(certPath);
               StreamReader reader = new StreamReader(keyPath);
               PemReader pemReader = new PemReader(reader);
               RsaPrivateCrtKeyParameters keyPair = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();


               var parms = DotNetUtilities.ToRSAParameters(keyPair);
               var rsa = RSA.Create();
               rsa.ImportParameters(parms);

               //RSA rsa = DotNetUtilities.ToRSA(keyPair);
               cert = cert.CopyWithPrivateKey(rsa);
                return new X509Certificate2(cert.Export(X509ContentType.Pfx));
             }catch(Exception ex)
             {
                 _logger.LogError(ex.Message + " "+ex.StackTrace.ToString());
                 if (ex.InnerException != null)
                     _logger.LogError(ex.InnerException.Message);

                 return null;
             }*/
            try
            {
             
                //      string pemContent = System.IO.File.ReadAllText(keyPath);
                //  const string header = "-----BEGIN PRIVATE KEY-----";
                //    const string footer = "-----END PRIVATE KEY-----";
                //    string privateKeyBase64 = pemContent.Replace(header, "").Replace(footer, "").Trim();

                string pkkey = "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC+xzFNk8oH/E1Z\r\nmVLw1JDrRsKDbR0sGszB6jzq+MWw6MjZonLnUHLxKDCPGkItu6d6x8/vukUTd5U+\r\nQkzLMKtHj2k6TQkYhveIieXfilt2IdU5w6x2XbFLaDhE053UygZwHiD57f+614zP\r\nU186wh0FJx/lBKzj2tePp85W13JeU/+FHNSbYkaZ1Ew5E1yDHqG7FzmyVgLJl4F5\r\na5h6aFArMpHL6iC16kC82TlKnSlBfWKjDf5cUbk0Ucd7PZgEoShiUaZc/G9kwm4b\r\nj+/zE7mJkgILrSQbZt8dg33K6E6u5pIdq5+63T2lYFMC3pqil3PFGVdQJ7ZGgU0g\r\nW6QsSENzAgMBAAECggEAOBqKJD9HFawb7zKI0qwgZiBsCYxoHNVQy+IfwhYgxRLg\r\nJiiA1Aezlbn09dCKD7r02MW0H9LOh5gOOr3yqXqJlETXdD69Ywol93BeOqwMk6QF\r\nebRSnoiVIBDpI0x8SNyUohoqJnXYU9EZ5sqk4bm+IQrNdWM3mNZ9uBBoPN+lEZnu\r\nHb9Sk3SspcB5wbl62jHOSCjkjk0SCaOQEgutypApbLBWMdQSw4X0otyKvhmJZrg1\r\n4yKhGJEu58mC5cEKuRH7VqJvjRzjHwcYuP4bc3EFrmeggVONhTpsJRX6NzFJfOL5\r\n9f7P1jbqSXzgjx88MxmDlM9E/U4+WtN+8yh5qxv9AQKBgQDmYI0DXxnsHJtMwF5s\r\nvKDjLNAPfyS2V2XXblfPeHEqCPWpbpXBUocBbZt0IQKNp8v/6emH7nSL867v4b/9\r\nk43cq1ohyPbDTGHshxKtiKemAjQP4mNlBACaFuP+H5ZpVZw+rBHegMVzUG1yy0kp\r\nPJrPZciZ9OnYzezytE19aDv+gQKBgQDT/ybJIsogF5gVPUofKNeA3uptipXU9SQ2\r\na+LJeCgcET0zkdwP6LzdTEbfyTu4zk+1e+NwqWukYvd4pn7OQaxBYsG4TrurpQ1z\r\nssNcPlfssog2jiFr+8/SxgODsRCP2Etqy5urdL1rrEtg9E8XwurY6jdHdrG1jiBR\r\nP41ZoZwv8wKBgQCT+91ZFxPduZqAuOluy9RFWZnk+nUotAd1VSoO2X1H1S2IwsJr\r\nxol2f2PmOvYa7Hh8UVNzv9cZt1TePpNHXis4XtGs9hyc40nb2ABFfIzEdJHgyjmy\r\nv9lrId0edkf2LcoJ8BoiZXFwW2+S18aNOKLxKp6rVsmIPO9CEFPehMBVAQKBgQC+\r\nb95voIjh+0/rBHupMg8k/Rqp/GxMOUqmeJPpV77wN6w6vzRoNjIyuWqRbTvw76Q6\r\n62eMtSS1LxIPl8Ehl20d75EF9/QuZL6IyHUmT/q77kTefR8Y2cP/G1Hc4xp1nV9i\r\nODVG+D+Qkd3E4rKKLda5tOyjjcRly57MXcKkWcDscQKBgQCo0p9bEf4d86AcdxCo\r\n5sPmGy468t3Z5QQi7ndWIfLQ/j4z+xRjGDLUfcVqA6KPHn+wVN2cFE6PxsnmJYJY\r\nsozDsBjcFLIjHKSc32GmmrBcAwTJbfxiA/QZ3WpxVth/hmaIku7IreQcPbTT+4U+\r\nzkjirOGl2vCSA0zVvEklvDNp0Q==";
                byte[] privateKeyBytes = Convert.FromBase64String(pkkey.Trim());
                //byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
                RSA rsa1 = RSA.Create();
                _logger.LogInformation("Key Created");
                rsa1.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                _logger.LogInformation("Key Imported");
                if (rsa1 != null)
                {
                   

                    _logger.LogInformation("Key Not NUll");
                    string certificate = "MIIF5zCCBM+gAwIBAgIMaMtPh3uEDcOGT4cBMA0GCSqGSIb3DQEBCwUAMIGmMQswCQYDVQQGEwJG\r\nUjEOMAwGA1UEBxMFUGFyaXMxHDAaBgNVBAoTE1BTQSBQZXVnZW90IENpdHJvZW4xFzAVBgNVBAsT\r\nDjAwMDIgMzE5MTg3MzA4MSAwHgYDVQQLExdDZXJ0aWZpY2F0ZSBBdXRob3JpdGllczEuMCwGA1UE\r\nAxMlUFNBIFBldWdlb3QgQ2l0cm9lbiBQcm9ncmFtcyBQYXJ0bmVyczAeFw0yMzEyMTgxNTIwMjda\r\nFw0yNDEyMTcxNTIwMjdaMGsxCzAJBgNVBAYTAkZSMQ8wDQYDVQQHDAZQT0lTU1kxHDAaBgNVBAoM\r\nE1BTQSBQZXVnZW90IENpdHJvZW4xGjAYBgNVBAsMEVByb2dyYW1zIFBhcnRuZXJzMREwDwYDVQQD\r\nDAhNWlBERk1BUDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAL7HMU2Tygf8TVmZUvDU\r\nkOtGwoNtHSwazMHqPOr4xbDoyNmicudQcvEoMI8aQi27p3rHz++6RRN3lT5CTMswq0ePaTpNCRiG\r\n94iJ5d+KW3Yh1TnDrHZdsUtoOETTndTKBnAeIPnt/7rXjM9TXzrCHQUnH+UErOPa14+nzlbXcl5T\r\n/4Uc1JtiRpnUTDkTXIMeobsXObJWAsmXgXlrmHpoUCsykcvqILXqQLzZOUqdKUF9YqMN/lxRuTRR\r\nx3s9mAShKGJRplz8b2TCbhuP7/MTuYmSAgutJBtm3x2DfcroTq7mkh2rn7rdPaVgUwLemqKXc8UZ\r\nV1AntkaBTSBbpCxIQ3MCAwEAAaOCAk0wggJJMB0GA1UdDgQWBBRUa0YTFVXlcnUMYLl3RxzPy+le\r\ndTAfBgNVHSMEGDAWgBQIeKr5h5yZceV2aVWbVkR1+576FzCB2wYDVR0gBIHTMIHQMIHNBgoqgXoB\r\nF84QAQEHMIG+MGoGCCsGAQUFBwICMF4wGhYTUFNBIFBldWdlb3QgQ2l0cm9lbjADAgEHGkBQb2xp\r\ndGlxdWUgZGUgQ2VydGlmaWNhdGlvbiBQU0EgUGV1Z2VvdCBDaXRyb2VuIFByb2dyYW1zIFBhcnRu\r\nZXJzMFAGCCsGAQUFBwIBFkRodHRwOi8vaW5mb2NlcnQucHNhLXBldWdlb3QtY2l0cm9lbi5jb20v\r\nUENfUFNBX1Byb2dyYW1zX1BhcnRuZXJzLnBkZjAnBgNVHSUEIDAeBggrBgEFBQcDAgYIKwYBBQUH\r\nAwEGCCsGAQUFBwMEMA4GA1UdDwEB/wQEAwIDuDAdBgNVHREEFjAUgRJydW4tZm9ybXNAbXBzYS5j\r\nb20wYgYDVR0fBFswWTBXoFWgU4ZRaHR0cDovL2luZm9jZXJ0LnBzYS1wZXVnZW90LWNpdHJvZW4u\r\nY29tL1BTQV9QZXVnZW90X0NpdHJvZW5fUHJvZ3JhbXNfUGFydG5lcnMuY3JsMG0GCCsGAQUFBwEB\r\nBGEwXzBdBggrBgEFBQcwAoZRaHR0cDovL2luZm9jZXJ0LnBzYS1wZXVnZW90LWNpdHJvZW4uY29t\r\nL1BTQV9QZXVnZW90X0NpdHJvZW5fUHJvZ3JhbXNfUGFydG5lcnMuY3J0MA0GCSqGSIb3DQEBCwUA\r\nA4IBAQCOgfrLRo9oA8Bu+QpIaaTYHBJllKKVh/0whuajY0dr8IYTshYdnJozeYX2/Aame5JlB2zH\r\n6je2f3YbP8OpxOUfpacqhuioIlEiR/Gkqak8+LJ4ua/BLefB3DQ/Ke+YbeqIrjl53gKQyVwpErJu\r\nM7eQTW9DGZq6Vk1w0LdfFd0zHjjX2o1tpYks4DvvJH/+nLSMSD0IKze2iqIzNb3wRQI7Kw3HAXnH\r\nMDmIk/TkgsT5O7u8v7Mzra9NLUrZGZYF+ni4ew15IAg1M7AY1EJojW5/U/r9qvlmTu1G3wp9NtLn\r\nulKZdSuZ5uz42cV+1mxuLCsbKCnh2pe8m6EyNspXy06e";
                    byte[] certData = Convert.FromBase64String(certificate.Trim());
                    X509Certificate2 cert1 = new X509Certificate2(certData);
                    _logger.LogInformation("Cert Created");
                    //X509Certificate2 cert1 = new X509Certificate2(certPath);
                    cert1 = cert1.CopyWithPrivateKey(rsa1);
                    _logger.LogInformation("Cert with key Created");
                    clientCertificate = new X509Certificate2(cert1.Export(X509ContentType.Pfx));
                    _logger.LogInformation("X was created "+ clientCertificate.ToString());

                    if (clientCertificate != null)
                    {
                        _logger.LogInformation("x is not null");
                        return clientCertificate;
                    }
                    else
                    {
                        _logger.LogInformation("x is null");
                        return null;
                    }
                }
                else
                {
                    _logger.LogInformation("x is null because of RSA");
                    return null;
                }
            }
            catch(Exception ex)
            {
                _logger.LogInformation("ERROR IN GETCERT " + ex.Message);
                if(ex.InnerException != null)
                {
                    _logger.LogInformation("ErrorInner: " + ex.InnerException.Message);
                }
                return null;
            }
        }


        // POST api/<ValuesController>
        [HttpPost]
        public async Task<string> Post()
        {
            _logger.LogInformation("--------------------------------------------------------------------------------");
            _logger.LogInformation("Post method started");
            _logger.LogInformation("--------------------------------------------------------------------------------");

            var handler = new HttpClientHandler();
            var resp = Get().Result;
            var certificate = await GetCert(absolutePath.ToString(), absolutePathKEY.ToString());
            handler.ClientCertificates.Add(certificate);
            using (var httpClient = new HttpClient(new HttpLoggingHandler(handler)))
            {
                try
                {
                    DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));


                    DateTime sevenDays = date.AddDays(-5);


                    var dateRange = new
                    {
                        startDate = sevenDays.ToString("yyyy-MM-ddT00:00:00.fffzzz"),
                        endDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
                    };

                    string jsonDate = JsonConvert.SerializeObject(dateRange);
                    TokenAuth tokenObject = JsonConvert.DeserializeObject<TokenAuth>(resp);
                   
                    var content = new StringContent(jsonDate, Encoding.UTF8, "application/json");
                    AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Authorization", "Bearer "+ tokenObject.access_token.Trim());
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api-secure.forms.awsmpsa.com/formsv3/api/leads");
                    request.Content = content;
                    request.Headers.Add("User-Agent", "MiddleApiCitroenMacedoniaPullData");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.access_token.Trim());
                   // httpClient.DefaultRequestHeaders.Add("User-Agent", "MiddleApiCitroenMacedoniaPullData");


                    //httpClient.DefaultRequestHeaders.Authorization = authHeader;


                    //var response = await httpClient.PostAsync("https://api-secure.forms.awsmpsa.com/formsv3/api/leads", content);

                    var response= await httpClient.SendAsync(request);
                    _logger.LogInformation("Post method Response: "+response.StatusCode);
                    string responseBody = await response.Content.ReadAsStringAsync();


                    if (response.StatusCode != HttpStatusCode.OK)
                    {

                       _logger.LogInformation("Post method no Leads");

                        _logger.LogInformation("Post method " + response.StatusCode.ToString());

                        return "No new leads";
                    }

                    RootObject responseData;

                    try
                    {
                        responseData = JsonConvert.DeserializeObject<RootObject>(responseBody);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return null;
                    }

                    Logs logs = new Logs();
                    _logger.LogInformation("--------------------------------------------------------------------------------");
                    _logger.LogInformation("Post method creating Logs");
                    _logger.LogInformation("--------------------------------------------------------------------------------");
                    foreach (Message msg in responseData.message)
                    {
                        if (msg.gitId == "MTcxNTA3NTE2NE4yTmd1")
                        {
                            Console.WriteLine("asd");
                        }
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
                    _logger.LogInformation("--------------------------------------------------------------------------------");
                    _logger.LogInformation("Post method formethod exit - Logs");
                    _logger.LogInformation("--------------------------------------------------------------------------------");

                    return response.StatusCode.ToString();
                }
                catch (HttpRequestException e)
                {
                    _logger.LogInformation("--------------------------------------------------------------------------------");
                    _logger.LogError("Post method error "+e.Message);
                    _logger.LogInformation("--------------------------------------------------------------------------------");

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
                    if (split.Length >1) {
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

                    
                    Enum.TryParse(model,out carenum);
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
                _logger.LogInformation("Lead response: "+response.StatusCode );
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
