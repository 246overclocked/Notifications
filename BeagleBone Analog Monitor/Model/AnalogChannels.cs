using System;
using System.Threading.Tasks;
using CommonCode;

namespace BeagleBoneAnalogMonitor.Model
{
    public class AnalogChannels : ObservableList<AnalogChannel>
    {
        private readonly UdpReceiver _udpReceiver;
        bool _shutdownReceiver;
        public AnalogChannels(int udpPort)
        {
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
                    var dataCount = task.Result.Buffer[0];
                    for (var sensorIndex = 0; sensorIndex < dataCount; sensorIndex++)
                    {
                        var sensorId = task.Result.Buffer[(sensorIndex * 3) + 1];
                        var sensorValue = BitConverter.ToUInt16(task.Result.Buffer, (sensorIndex * 3) + 2);
                        var oldValue = Find(a => a.SensorId == sensorId);
                        if (oldValue == null)
                        {
                            Add(sensorId > 0
                                ? new AnalogChannel(sensorId, sensorValue, 0, 4095)
                                : new AnalogChannel(sensorId, sensorValue/64.0/60.0, 0.0, 6.0));
                        }
                        else
                        {
                            if (sensorId > 0) oldValue.LastValue = sensorValue;
                            else oldValue.LastValue = sensorValue/64.0/60.0;
                        }
                    }
                }
                task = _udpReceiver.ReceiveAsync();
            }            
        }

        public void Shutdown()
        {
            _shutdownReceiver = true;
        }
    }
}