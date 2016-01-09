using System;
using System.Runtime.Serialization;

namespace Driver_Alert_Station
{
    [Serializable]
    public class TelemetryVersionMismatchException : Exception
    {
        public TelemetryVersionMismatchException() { }
        public TelemetryVersionMismatchException(string message) : base(message) { }
        public TelemetryVersionMismatchException(string message, Exception innerException) : base(message, innerException) { }
        public TelemetryVersionMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TelemetryAlreadyInitializedException : Exception
    {
        public TelemetryAlreadyInitializedException() {}
        public TelemetryAlreadyInitializedException(string message) : base(message) {}
        public TelemetryAlreadyInitializedException(string message, Exception inner) : base(message, inner) {}
        protected TelemetryAlreadyInitializedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
