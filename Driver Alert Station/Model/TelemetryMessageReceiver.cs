using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommonCode;

namespace Driver_Alert_Station.Model
{
    public class TelemetryMessageReceiver 
    {
        private readonly UdpReceiver _udpReceiver;
        bool _shutdownReceiver;
        //private Dispatcher _dispatcher;
        public TelemetryMessageReceiver(int udpPort)
        {
            //_dispatcher = Dispatcher.CurrentDispatcher;
            _udpReceiver = new UdpReceiver(udpPort);
            Task.Run(() => ProcessUdpPackets());
        }

        void ProcessUdpPackets()
        {
            var task = _udpReceiver.ReceiveAsync();
            while (!_shutdownReceiver)
            {
                if (task.Wait(1000))
                {
                    var newMessage = AlertMessage.FromDatagram(task.Result);
                    task = _udpReceiver.ReceiveAsync();
                }
            }
        }

        public void Shutdown()
        {
            _shutdownReceiver = true;
        }
    }
}