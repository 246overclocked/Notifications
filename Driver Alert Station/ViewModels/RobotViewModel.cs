using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonCode;
using Driver_Alert_Station.Model;
using Microsoft.Practices.Prism.Mvvm;
using PostSharp.Patterns.Model;

namespace Driver_Alert_Station.ViewModels
{
    [NotifyPropertyChanged]
    public class RobotViewModel : BindableBase
    {
        private double _sideViewLeft;
        private double _sideViewBottom;
        private double _shoulderAngle;
        private double _elbowAngle;
        private double _wristAngle;
        private double _actualWidth;
        private double _actualHeight;
        private Dispatcher _dispatcher;
        private readonly TelemetryData _telemetryData;

        [UsedImplicitly] PropertyObserver<TelemetryData> _propertyObserver;
        public RobotViewModel(TelemetryData telemetryData)
        {
            SideViewLeft = 200;
            SideViewBottom = 900;
            ShoulderAngle = 90;
            ElbowAngle = 0;
            WristAngle = 90;
            SideViewScale = 0.5;
            LeftGetterViewModel = new GetterViewModel();
            RightGetterViewModel = new GetterViewModel();
            _telemetryData = telemetryData;
            _propertyObserver = new PropertyObserver<TelemetryData>(_telemetryData)
                .RegisterHandler(t => t.ShoulderAngle,
                                 () => ShoulderAngle = _telemetryData.ShoulderAngle)
                .RegisterHandler(t => t.ElbowAngle,
                                 () => ElbowAngle = _telemetryData.ElbowAngle)
                .RegisterHandler(t => t.WristAngle,
                                 () => WristAngle = _telemetryData.WristAngle);
        }

        private const double RobotBaseHeight = 66;
        private const double RobotBaseWidth = 415;
        private const double ElevatorVerticalOffsetX = 100;
        private const double ShoulderSupportOffsetX = 320;
        private const double ElevatorBraceOffsetX = 130;
        private const double ShoulderJointOffsetX = 355;
        private const double ShoulderJointOffsetY = 332;
        private const double UpperArmLength = 240;
        private const double ForearmLength = 240;

        private void SetStructureOffsets()
        {
            ShoulderJointX = SideViewLeft + ShoulderJointOffsetX;
            ShoulderJointY = SideViewBottom - ShoulderJointOffsetY;
            ShoulderSupportX = SideViewLeft + ShoulderSupportOffsetX;
            ShoulderSupportY = SideViewBottom - RobotBaseHeight;
            ElevatorVerticalX = SideViewLeft + ElevatorVerticalOffsetX;
            ElevatorVerticalY = SideViewBottom - RobotBaseHeight;
            ElevatorBraceX = SideViewLeft + ElevatorBraceOffsetX;
            ElevatorBraceY = SideViewBottom - RobotBaseHeight;
            ForkX = ElevatorVerticalX;
            ForkY = ElevatorVerticalY;
            SetArmOffsets();
        }

        private void SetArmOffsets()
        {
            ShoulderRotateTransformAngle = ShoulderAngle - 90;
            ElbowJointX = ShoulderJointX + Math.Cos(ShoulderRotateTransformAngle * Math.PI / 180.0) * UpperArmLength;
            ElbowJointY = ShoulderJointY + Math.Sin(ShoulderRotateTransformAngle * Math.PI / 180.0) * UpperArmLength;
            ElbowRotateTransformAngle = ElbowAngle - 90;
            WristJointX = ElbowJointX + Math.Cos(ElbowRotateTransformAngle * Math.PI / 180.0) * ForearmLength;
            WristJointY = ElbowJointY + Math.Sin(ElbowRotateTransformAngle * Math.PI / 180.0) * ForearmLength;
            WristRotateTransformAngle = WristAngle - 90;
        }

        public double ShoulderSupportX { get; private set; }
        public double ShoulderSupportY { get; private set; }
        public double ElevatorVerticalX { get; private set; }
        public double ElevatorVerticalY { get; private set; }
        public double ElevatorBraceX { get; private set; }
        public double ElevatorBraceY { get; private set; }
        public double ShoulderRotateTransformAngle { get; private set; }
        public double ShoulderJointX { get; private set; }
        public double ShoulderJointY { get; private set; }
        public double ElbowRotateTransformAngle { get; private set; }
        public double ElbowJointX { get; private set; }
        public double ElbowJointY { get; private set; }
        public double WristRotateTransformAngle { get; private set; }
        public double WristJointX { get; private set; }
        public double WristJointY { get; private set; }
        public double ForkX { get; private set; }
        public double ForkY { get; private set; }
        public GetterViewModel LeftGetterViewModel { get; private set; }
        public GetterViewModel RightGetterViewModel { get; private set; }

        public double SideViewScale { get; set; }
        public double SideViewCenterX { get; set; }
        public double SideViewCenterY { get; set; }

        public double ActualWidth
        {
            get { return _actualWidth; }
            set
            {
                _actualWidth = value;
                SideViewLeft = (_actualWidth - RobotBaseWidth)/2;
                SideViewCenterX = _actualWidth/2;
                _dispatcher = Dispatcher.CurrentDispatcher;
            }
        }

        public double ActualHeight
        {
            get { return _actualHeight; }
            set
            {
                _actualHeight = value;
                SideViewBottom = _actualHeight - 50;
                SideViewCenterY = _actualHeight;
            }
        }

        public double ShoulderAngle
        {
            get { return _shoulderAngle; }
            set
            {
                _shoulderAngle = value;
                SetArmOffsets();
            }
        }

        public double ElbowAngle
        {
            get { return _elbowAngle; }
            set
            {
                _elbowAngle = value;
                SetArmOffsets();
            }
        }

        public double WristAngle
        {
            get { return _wristAngle; }
            set
            {
                _wristAngle = value;
                SetArmOffsets();
            }
        }

        public double SideViewLeft
        {
            get { return _sideViewLeft; }
            set
            {
                _sideViewLeft = value;
                SetStructureOffsets();
            }
        }

        public double SideViewBottom
        {
            get { return _sideViewBottom; }
            set
            {
                _sideViewBottom = value;
                SetStructureOffsets();
            }
        }
    }

    public enum GetterState
    {
        Off,
        PullingIn,
        PushingOut,
    }

    public class GetterViewModel : BindableBase
    {
        public GetterViewModel()
        {
            GetterState = GetterState.Off;
        }

        GetterState _getterState;

        public GetterState GetterState
        {
            get { return _getterState; }
            set
            {
                _getterState = value;
                switch (_getterState)
                {
                    case GetterState.Off:
                        PullingInArrowVisibility = Visibility.Collapsed;
                        PushingOutArrowVisibility = Visibility.Collapsed;
                        break;
                    case GetterState.PullingIn:
                        PullingInArrowVisibility = Visibility.Visible;
                        PushingOutArrowVisibility = Visibility.Collapsed;
                        break;
                    case GetterState.PushingOut:
                        PullingInArrowVisibility = Visibility.Collapsed;
                        PushingOutArrowVisibility = Visibility.Visible;
                        break;
                }
            }
        }

        public Visibility PullingInArrowVisibility { get; private set; }
        public Visibility PushingOutArrowVisibility { get; private set; }
    }

    public class SwerveModuleViewModel : BindableBase
    {
        readonly double _wheelWidth;
        readonly double _wheelHeight;
        readonly double _maxSpeed;
        double _speed;

        public SwerveModuleViewModel(double wheelWidth, double wheelHeight, double maxSpeed)
        {
            _wheelWidth = wheelWidth;
            _wheelHeight = wheelHeight;
            _maxSpeed = maxSpeed;
            WheelPath = new Path {Data = Geometry.Parse(string.Format("m -{0},-{1} h {2} v {3} h -{2} z", _wheelWidth / 2, _wheelHeight / 2, _wheelWidth, _wheelHeight))};
            SpeedArrowPath = new Path();
            SpeedArrowVisibility = Visibility.Collapsed;
        }

        public Visibility SpeedArrowVisibility { get; private set; }
        public double Angle { get; set; }
        public Path SpeedArrowPath { get; private set; }
        public Path WheelPath { get; private set; }
        public double Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                if (Math.Abs(Math.Abs(_speed)) < .01) SpeedArrowVisibility = Visibility.Collapsed;
                else
                {
                    var arrowShaftWidth = _wheelWidth;
                    var arrowShaftHeight = (_wheelHeight / 2) * (Math.Abs(_speed) / _maxSpeed);
                    SpeedArrowPath.Data = Geometry.Parse(string.Format("m {0},0 v {1} h {0} l {2}, {2} l {2}, {3}, h {0} v {4} z", -arrowShaftWidth / 2, -arrowShaftHeight, arrowShaftWidth, -arrowShaftHeight, arrowShaftHeight));
                    SpeedArrowPath.RenderTransform = new ScaleTransform(1.0, _speed < 0 ? -1.0 : 1.0);
                    OnPropertyChanged("SpeedArrowPath");
                    SpeedArrowVisibility = Visibility.Visible;
                }
            }
        }
    }
}
