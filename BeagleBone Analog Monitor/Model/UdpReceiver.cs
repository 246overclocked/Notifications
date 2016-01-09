using System.Net.Sockets;
using System.Threading.Tasks;

namespace BeagleBoneAnalogMonitor.Model
{
    public class UdpReceiver
    {
        public UdpReceiver(int port)
        {
            Port = port;
            _unicastUdpClient = new UdpClient(Port);
        }

        public int Port { get; private set; }

        public Task<UdpReceiveResult> ReceiveAsync()
        {
            return _unicastUdpClient.ReceiveAsync();
        }

        private readonly UdpClient _unicastUdpClient;
    }
}