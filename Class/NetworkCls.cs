using System.Net.NetworkInformation;

namespace ATD_ID4P.Class
{
    public static class NetworkCls
    {
        public static bool CheckPing(string IP)
        {
            bool CanPing;
            var ping = new Ping();
            var reply = ping.Send(IP, 3 * 100); // 1 minute time out (in ms)            
            CanPing = (reply.Status == IPStatus.Success) ? true : false;
            return CanPing;
        }
    }
}
