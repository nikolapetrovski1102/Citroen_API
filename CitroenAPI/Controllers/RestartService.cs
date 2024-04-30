using Microsoft.AspNetCore.Mvc.Controllers;
using System.ServiceProcess;

namespace CitroenAPI.Controllers
{
    public class RestartService : IDisposable
    {
        private ServiceController _service;
        private string _serviceName = "SchadulerService";

        public RestartService()
        {
            _service = new ServiceController(_serviceName);
        }

        public void Dispose()
        {
            // Stop the service before disposing
            if (_service.Status == ServiceControllerStatus.Running)
            {
                _service.Stop();
                _service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)); // Wait for the service to stop
            }

            // Start the service again
            _service.Start();
            _service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)); // Wait for the service to start
        }
    }
}