using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommonCode;

namespace Driver_Alert_Station.Model
{
    public class AlertMessageReceiver : ObservableList<AlertMessage>
    {
        private readonly UdpReceiver _udpReceiver;
        bool _shutdownReceiver;
        private readonly ConcurrentDictionary<AlertMessage, Timer> _duplicateFilter = new ConcurrentDictionary<AlertMessage, Timer>(); 
        //private Dispatcher _dispatcher;
        public AlertMessageReceiver(int udpPort)
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
                    Timer messageTimer;
                    if (_duplicateFilter.TryGetValue(newMessage, out messageTimer))
                    {
                        // Reset the timer because we got a duplicate message
                        messageTimer.Change(250, Timeout.Infinite);
                        //Debug.WriteLine("{0:HH:mm:ss.fff} Duplicate discarded for message: {1}", DateTime.Now, newMessage.Message);
                    }
                    else
                    {
                        if (newMessage.FilterDuplicateMessages)
                        {
                            var timer = new Timer(state =>
                            {
                                Timer timerToDiscard;
                                var tryRemoveSucceeded = _duplicateFilter.TryRemove((AlertMessage)state, out timerToDiscard);
                                //Debug.WriteLine(tryRemoveSucceeded
                                //                ? "{0:HH:mm:ss.fff} REMOVED duplicate filter for message: {1}"
                                //                : "{0:HH:mm:ss.fff} FAILED TO REMOVE duplicate filter for message: {1}",
                                //                DateTime.Now, ((AlertMessage) state).Message);
                                // Prevent this timer from firing again
                                timerToDiscard.Change(Timeout.Infinite, Timeout.Infinite);
                                // And explicitly dispose of it
                                timerToDiscard.Dispose();
                            }, newMessage, 250, Timeout.Infinite);
                            if (_duplicateFilter.TryAdd(newMessage, timer))
                            {
                                //Debug.WriteLine(
                                //    "{0:HH:mm:ss.fff} New message received. Will reject duplicates for 250ms. Message: {1}",
                                //    DateTime.Now, newMessage.Message);
                                AddAlertMessage(newMessage);
                                newMessage.PlaySoundIfPresent();
                            }
                            else
                            {
                                //Debug.WriteLine(
                                //    "{0:HH:mm:ss.fff} New message received but FAILED TO ADD TO DUPLICATE FILTER. This is presumably a duplicate of a messages recently received by another thread and will therefore be discarded. Message: {1}",
                                //    DateTime.Now, newMessage.Message);
                                AddAlertMessage(AlertMessage.Create(false,
                                    String.Format(
                                        "New message received but FAILED TO ADD TO DUPLICATE FILTER. This is presumably a duplicate of a messages recently received by another thread and will therefore be discarded. Message: {0}",
                                        newMessage.Message), null, "DEBUG"));
                                timer.Change(Timeout.Infinite, Timeout.Infinite);
                                timer.Dispose();
                            }
                        }
                        else
                        {
                            AddAlertMessage(newMessage);
                            newMessage.PlaySoundIfPresent();
                        }
                    }
                    task = _udpReceiver.ReceiveAsync();
                }
            }
        }

        private StreamWriter _writer = null;

        public void StartLogging()
        {
            if (_writer != null) return;
            var logFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Robot Alert Logs", string.Format("{0:yyyy-MM-dd}", DateTime.Now));
            if (!Directory.Exists(logFileFolder)) Directory.CreateDirectory(logFileFolder);
            var logFilePath = Path.Combine(logFileFolder, string.Format("{0:HH-mm-ss}.csv", DateTime.Now));
            var writeHeader = !File.Exists(logFilePath);
            _writer = new StreamWriter(logFilePath, true);
            if (writeHeader) _writer.WriteLine("ReceiveTime,SeverityLevel,Message,SoundFileToPlay,MessageCategory,SubsystemId,Details");
        }

        public void StopLogging()
        {
            if (_writer == null) return;
            _writer.Flush();
            _writer.Close();
            _writer = null;
        }

        private void AddAlertMessage(AlertMessage message)
        {
            if (_writer != null)
                _writer.WriteLine("{0:HH:mm:ss.fff},{1},{2},{3},{4},{5},{6}",
                    message.ReceiveTime,
                    string.IsNullOrEmpty(message.SeverityLevel) ? "" : message.SeverityLevel,
                    string.IsNullOrEmpty(message.Message) ? "" : message.Message,
                    string.IsNullOrEmpty(message.SoundFileToPlay) ? "" : Path.GetFileName(message.SoundFileToPlay),
                    string.IsNullOrEmpty(message.MessageCategory) ? "" : message.MessageCategory,
                    string.IsNullOrEmpty(message.SubsystemId) ? "" : message.SubsystemId,
                    string.IsNullOrEmpty(message.Details) ? "" : message.Details);
            Add(message);
#if false
            if (Count > 1000)
            {
                DisableCollectionChangedNotifications();
                RemoveRange(0, 200);
                EnableCollectionChangedNotifications();
            }
#endif
        }
        public void Shutdown()
        {
            _shutdownReceiver = true;
        }
    }
}