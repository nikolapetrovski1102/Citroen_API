using System.Net;

namespace CitroenAPI
{
    internal static class Utility
    {

        internal static string GetMachineIPAddress()
        {
            string? ip = null;
            string Hostname = System.Environment.MachineName;

            IPHostEntry Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = $"{IP}";
                    break;
                }
            }
            if (ip is null)
            {
                Console.WriteLine("Wifi/LAN IP address not found try restart ... ");

                ip = "ip_not_found";

            }

            return ip;

        }
    }
}
