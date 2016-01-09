using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Mvvm;
using PostSharp.Patterns.Model;

namespace Driver_Alert_Station.Model
{
    [NotifyPropertyChanged]
    public class TelemetryData : BindableBase
    {
        private TelemetryData(int udpPort, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _dispatcher.Invoke(() => Status = "Starting");

            _udpReceiver = new UdpReceiver(udpPort);
            Debug.WriteLine("Starting telemetry receiver on port " + udpPort);
            _lastReceiveTime = DateTime.Now;
        }

        private DateTime _lastReceiveTime;
        private void Receiver()
        {
            var task = _udpReceiver.ReceiveAsync();
            while (!_shutdownReceiver)
            {
                if (!task.Wait(1000))
                {
                    _dispatcher.Invoke(() => _instance.Status = string.Format("Idle {0:hh\\:mm\\:ss}", (DateTime.Now - _lastReceiveTime)));
                    continue;
                }
                try
                {
                    _lastReceiveTime = DateTime.Now;
                    UpdateFromDatagram(task.Result);
                    task = _udpReceiver.ReceiveAsync();
                }
                catch (Exception e)
                {
                    _dispatcher.Invoke(() => _instance.Status = e.Message);
                    Debug.WriteLine("Caught exception: " + e.Message);
                    _shutdownReceiver = true;
                }
            }
        }

        private static TelemetryData _instance;
        private readonly Dispatcher _dispatcher;
        private readonly UdpReceiver _udpReceiver;
        bool _shutdownReceiver;

        public static TelemetryData Initialize(int udpPort, Dispatcher dispatcher)
        {
            if (_instance != null) throw new TelemetryAlreadyInitializedException();
            _instance = new TelemetryData(udpPort, dispatcher);
            Task.Run(() => _instance.Receiver());
            return _instance;
        }

        public static TelemetryData GetInstance()
        {
            return _instance;
        }

        public static void ShutdownReceiver()
        {
            _instance._shutdownReceiver = true;
        }

        private const Int32 TelemetryVersionNumber = 2;
        public void UpdateFromDatagram(UdpReceiveResult receiveResult)
        {
            var buffer = receiveResult.Buffer;
            var byteIndex = 0;
            // Note: This version number must match the version that this decoder expects to see
            //Debug.WriteLine("{0:x} {1:x} {2:x} {3:x}", buffer[0], buffer[1], buffer[2], buffer[3]);
            var telemetryVersionNumber = BitConverter.ToInt32(buffer, byteIndex);
            if (telemetryVersionNumber != TelemetryVersionNumber) throw new TelemetryVersionMismatchException("Telemetry version mismatch. Expected: " + TelemetryVersionNumber + ", got " + telemetryVersionNumber);
            byteIndex += sizeof (Int32);
            _dispatcher.Invoke(() =>
            {
                // Telemetry data starts here
                FrontLeftWheelCommandedSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                FrontLeftWheelActualSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                FrontLeftWheelSteeringSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                FrontLeftWheelActualAngle = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                FrontRightWheelCommandedSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                FrontRightWheelActualSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                FrontRightWheelSteeringSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                FrontRightWheelActualAngle = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                RearWheelCommandedSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                RearWheelActualSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                RearWheelSteeringSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                RearWheelActualAngle = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                ShoulderJointSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                ShoulderJointAngle = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                ElbowJointSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                ElbowJointAngle = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                WristJointSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                WristJointAngle = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                GrabberCommandedSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof (double);
                GrabberTickCount = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof (double);
                LiftCommandedSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof (double);
                LiftActualHeight = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof (double);
                LeftRangeSensorValue = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                RightRangeSensorValue = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                LeftGetterCommandedSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                RightGetterCommandedSpeed = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxPitch = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxRoll = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxYaw = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxXAccel = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxYAccel = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxZAccel = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxQuaternionW = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxQuaternionX = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxQuaternionY = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);
                NavxQuaternionZ = BitConverter.ToDouble(buffer, byteIndex);
                byteIndex += sizeof(double);

                ElbowWristAngleConstraintViolation = buffer[byteIndex] != 0;
                ForearmLiftConstraintViolation = buffer[byteIndex] != 0;
                GrabberCeilingConstraintViolation = buffer[byteIndex] != 0;
                GrabberGroundConstraintViolation = buffer[byteIndex] != 0;
                GrabberLiftConstraintViolation = buffer[byteIndex] != 0;
                ShoulderAngleConstraintViolation = buffer[byteIndex] != 0;
                ShoulderElbowAngleConstraintViolation = buffer[byteIndex] != 0;
                ShoulderWristAngleConstraintViolation = buffer[byteIndex] != 0;
                WristCeilingConstraintViolation = buffer[byteIndex] != 0;
                WristGroundConstraintViolation = buffer[byteIndex] != 0;

                LastUpdate = DateTime.Now;
                Status = "Receiving";
                //Debug.WriteLine("Telemetry receiving");
            });
        }
        public string Status { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public double FrontLeftWheelCommandedSpeed { get; private set; }
        public double FrontLeftWheelActualSpeed { get; private set; }
        public double FrontLeftWheelSteeringSpeed { get; private set; }
        public double FrontLeftWheelActualAngle { get; private set; }
        public double FrontRightWheelCommandedSpeed { get; private set; }
        public double FrontRightWheelActualSpeed { get; private set; }
        public double FrontRightWheelSteeringSpeed { get; private set; }
        public double FrontRightWheelActualAngle { get; private set; }
        public double RearWheelCommandedSpeed { get; private set; }
        public double RearWheelActualSpeed { get; private set; }
        public double RearWheelSteeringSpeed { get; private set; }
        public double RearWheelActualAngle { get; private set; }
        public double ShoulderJointSpeed { get; private set; }
        public double ShoulderJointAngle { get; private set; }
        public double ElbowJointSpeed { get; private set; }
        public double ElbowJointAngle { get; private set; }
        public double WristJointSpeed { get; private set; }
        public double WristJointAngle { get; private set; }
        public double GrabberCommandedSpeed { get; private set; }
        public double GrabberTickCount { get; private set; }
        public double LiftCommandedSpeed { get; private set; }
        public double LiftActualHeight { get; private set; }
        public double LeftRangeSensorValue { get; private set; }
        public double RightRangeSensorValue { get; private set; }
        public double LeftGetterCommandedSpeed { get; private set; }
        public double RightGetterCommandedSpeed { get; private set; }
        public double NavxPitch { get; private set; }
        public double NavxRoll { get; private set; }
        public double NavxYaw { get; private set; }
        public double NavxXAccel { get; private set; }
        public double NavxYAccel { get; private set; }
        public double NavxZAccel { get; private set; }
        public double NavxQuaternionW { get; private set; }
        public double NavxQuaternionX { get; private set; }
        public double NavxQuaternionY { get; private set; }
        public double NavxQuaternionZ { get; private set; }

        public bool ElbowWristAngleConstraintViolation { get; private set; }
        public bool ForearmLiftConstraintViolation { get; private set; }
        public bool GrabberCeilingConstraintViolation { get; private set; }
        public bool GrabberGroundConstraintViolation { get; private set; }
        public bool GrabberLiftConstraintViolation { get; private set; }
        public bool ShoulderAngleConstraintViolation { get; private set; }
        public bool ShoulderElbowAngleConstraintViolation { get; private set; }
        public bool ShoulderWristAngleConstraintViolation { get; private set; }
        public bool WristCeilingConstraintViolation { get; private set; }
        public bool WristGroundConstraintViolation { get; private set; }
    }
}
