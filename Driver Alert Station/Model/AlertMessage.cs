using System;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.Timers;

namespace Driver_Alert_Station.Model
{
    [Serializable]
    public class AlertMessage : IEquatable<AlertMessage>
    {
        private AlertMessage()
        {
            ReceiveTime = DateTime.Now;
            ForegroundBrush = SystemColors.ControlTextBrush;
            BackgroundBrush = SystemColors.ControlLightLightBrush;
            FontWeight = FontWeights.Normal;
            FilterDuplicateMessages = true;
        }

        private static readonly SoundPlayer SoundPlayer;
        public static Dispatcher Dispatcher;

        public static string LogfileName;

        static AlertMessage()
        {
            SoundPlayer = new SoundPlayer();
        }

        public static AlertMessage Create(bool doNotLog, string message, string soundFile, string severity)
        {
            var result = new AlertMessage
            {
                DoNotLog = doNotLog,
                Message = message,
                SoundFileToPlay = soundFile,
                SeverityLevel = severity,
            };
            result.Initialize();
            return result;
        }
        public void Initialize()
        {
            // If we got a sound file, validate that we have a file with that name. If we don't, set the filename to null 
            if (!string.IsNullOrEmpty(SoundFileToPlay))
            {
                try
                {
                    var file = Path.GetFileName(SoundFileToPlay);
                    file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sounds", file);
                    if (File.Exists(file)) SoundFileToPlay = file;
                    else
                    {
                        var soundNotFoundError = "Oops: Requested sound file \"" + SoundFileToPlay +
                                                 "\" was not found. Playing \"uhoh.wav\" instead";
                        if (string.IsNullOrEmpty(Message)) Message = soundNotFoundError;
                        else Message += "\n" + soundNotFoundError;
                        file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                            "Sounds", "uhoh.wav");
                        if (File.Exists(file)) SoundFileToPlay = file;
                        else Message += "\n" + "Backup sound not found either. I guess there's NO SOUND FOR YOU!";
                    }
                }
                catch (Exception)
                {
                    SoundFileToPlay = null;
                }
            }
            switch (SeverityLevel.ToUpper())
            {
                case "CHITCHAT":
                    ForegroundBrush = Brushes.DarkGray;
                    break;
                case "DEBUG":
                    ForegroundBrush = Brushes.Black;
                    break;
                case "WARNING":
                    BackgroundBrush = Brushes.Gold;
                    break;
                case "ERROR":
                    FilterDuplicateMessages = false;
                    BackgroundBrush = Brushes.Orange;
                    break;
                case "FATAL":
                    FilterDuplicateMessages = false;
                    BackgroundBrush = Brushes.Red;
                    FontWeight = FontWeights.Bold;
                    break;
            }
#if false
            Dispatcher.Invoke(() =>
            {
                if (_fileWriter == null && LogfileName != null)
                {
                    // If I have no file open, and the caller decides to start logging (set LogfileName)
                    // Open or create the named file
                    _fileWriter = new StreamWriter(LogfileName, true);
                    // Remember the name of the opened file
                    _currentLogfileName = LogfileName;
                }
                else if (_fileWriter != null && LogfileName == null)
                {
                    // If I have a file open, and the caller decides to stop logging (set LogfileName to null)
                    // Close the currently open logfile
                    _fileWriter.Close();
                    // Set the stream to null to prevent future writing
                    _fileWriter = null;
                    // And just set this because we should
                    _currentLogfileName = null;
                }
                else if (_fileWriter != null && LogfileName != null && LogfileName != _currentLogfileName)
                {
                    // If I have a file open, and the caller wants to log messages, but for whatever reason
                    // has changed LogfileName to point to some other file
                    // Close the currently open logfile
                    _fileWriter.Close();
                    // Open the newly-named file for append (creates if the file doesn't exist)
                    _fileWriter = new StreamWriter(LogfileName, true);
                    // Remember the name of the file we're currently writing
                    _currentLogfileName = LogfileName;
                }
                if (_fileWriter != null) _fileWriter.WriteLine("{0:HH:mm:ss.fff} {1} {2} {3} {4} {5} {6}", ReceiveTime, Sender, Message, SeverityLevel, SoundFileToPlay, MessageCategory, SubsystemId);                
            }, DispatcherPriority.Background);
#endif
        }

        public void PlaySoundIfPresent()
        {
            if (SoundFileToPlay == null) return;
            SoundPlayer.SoundLocation = SoundFileToPlay;
            SoundPlayer.Play();
        }

        public static AlertMessage FromDatagram(UdpReceiveResult receiveResult)
        {
            var xml = System.Text.Encoding.UTF8.GetString(receiveResult.Buffer);
            var serializer = new XmlSerializer(typeof (AlertMessage));

            using (var reader = new StringReader(xml))
            {
                var result = (AlertMessage) serializer.Deserialize(reader);
                result.Sender = receiveResult.RemoteEndPoint.Address;
                result.Port = receiveResult.RemoteEndPoint.Port;
                result.Initialize();
                return result;
            }
        }

        [XmlIgnore]
        public IPAddress Sender { get; private set; }

        [XmlIgnore]
        public int Port { get; private set; }

        [XmlIgnore]
        public bool FilterDuplicateMessages { get; private set; }
        
        [XmlIgnore]
        public DateTime ReceiveTime { get; private set; }

        [XmlIgnore]
        public Brush ForegroundBrush { get; private set; }

        [XmlIgnore]
        public Brush BackgroundBrush { get; private set; }

        [XmlIgnore]
        public FontWeight FontWeight { get; private set; }

        [XmlIgnore]
        public bool DoNotLog { get; set; }

        [XmlElement("Message")]
        public string Message { get; set; }

        [XmlElement("SoundFileToPlay")]
        public string SoundFileToPlay { get; set; }

        [XmlElement("SeverityLevel")]
        public string SeverityLevel { get; set; }

        [XmlElement("MessageCategory")]
        public string MessageCategory { get; set; }

        [XmlElement("SubsystemId")]
        public string SubsystemId { get; set; }

        [XmlElement("Details")]
        public string Details { get; set; }

        public void Dump()
        {
            Console.WriteLine(@"Alert from: {0} at {1}", Sender, ReceiveTime);
            Console.WriteLine(@"     Message: {0}", String.IsNullOrEmpty(Message) ? "(none)" : Message);
            Console.WriteLine(@"  Sound File: {0}", String.IsNullOrEmpty(SoundFileToPlay) ? "(none)" : SoundFileToPlay);
            Console.WriteLine(@"    Severity: {0}", String.IsNullOrEmpty(SeverityLevel) ? "(none)" : SeverityLevel);
            Console.WriteLine(@"    Category: {0}", String.IsNullOrEmpty(MessageCategory) ? "(none)" : MessageCategory);
            Console.WriteLine(@"   Subsystem: {0}", String.IsNullOrEmpty(SubsystemId) ? "(none)" : SubsystemId);
            Console.WriteLine(@"     Details: {0}", String.IsNullOrEmpty(Details) ? "(none)" : Details);
            if (!string.IsNullOrEmpty(SoundFileToPlay))
            {
                var player = new System.Media.SoundPlayer(SoundFileToPlay);
                player.PlaySync();
            }
        }

        public bool Equals(AlertMessage other)
        {
            if (other == null) return false;
            return Message == other.Message && SoundFileToPlay == other.SoundFileToPlay &&
                   SeverityLevel == other.SeverityLevel;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AlertMessage);
        }

        public override int GetHashCode()
        {
            if (Message == null && SoundFileToPlay == null && SeverityLevel == null) return 0;
            if (Message == null && SoundFileToPlay == null && SeverityLevel != null) return SeverityLevel.GetHashCode();
            if (Message == null && SoundFileToPlay != null && SeverityLevel == null) return SoundFileToPlay.GetHashCode();
            if (Message != null && SoundFileToPlay != null && SeverityLevel == null) return Message.GetHashCode();
            if (Message == null && SoundFileToPlay != null && SeverityLevel != null) return SoundFileToPlay.GetHashCode() ^ SeverityLevel.GetHashCode();
            if (Message != null && SoundFileToPlay != null && SeverityLevel == null) return Message.GetHashCode() ^ SoundFileToPlay.GetHashCode();
            if (Message != null && SoundFileToPlay == null && SeverityLevel != null) return Message.GetHashCode() ^ SeverityLevel.GetHashCode();
            return Message.GetHashCode() ^ SoundFileToPlay.GetHashCode() ^ SeverityLevel.GetHashCode();
        }

        public static bool operator ==(AlertMessage a, AlertMessage b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
            if (ReferenceEquals(a, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(AlertMessage a, AlertMessage b)
        {
            return !(a == b);
        }
    }
}