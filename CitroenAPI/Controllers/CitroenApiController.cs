using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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
            X509Certificate2 certificate = new X509Certificate2("C:\\Users\\nikpe\\source\\repos\\CitroenAPI\\CitroenAPI\\Certificate\\MZPDFMAP.cer", "C:\\Users\\nikpe\\source\\repos\\CitroenAPI\\CitroenAPI\\Certificate\\MZPDFMAP.pk");
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "YourUserAgent");
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api-secure.forms.awsmpsa.com/oauth/v2/token?client_id=5f7f179e7714a1005d204b43_2w88uv9394aok4g8gs0ccc4w4gwsskowck0gs0oo0sggw0kog0&client_secret=619ffmx8sn0g8ossso44wwok8scgoww00s8sogkw8w08cgc0wg&grant_type=password&username=ACMKPR&password=N9zTQ6v1");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
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
