using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Mvvm;
using OxyPlot;

namespace BeagleBoneAnalogMonitor.Model
{
    public class AnalogChannel : BindableBase
    {
        public AnalogChannel(UInt16 sensorId, double minValue, double maxValue)
        {
            SensorId = sensorId;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public AnalogChannel(UInt16 sensorId, double lastValue, double minValue, double maxValue)
        {
            SensorId = sensorId;
            LastValue = lastValue;
            //if (Math.Abs(lastValue) < double.Epsilon) Debug.WriteLine("{0} Zero!", DateTime.Now);
            MinValue = minValue;
            MaxValue = maxValue;
            LastValueTime = DateTime.Now;
            _statsTimer.AutoReset = true;
            _statsTimer.Interval = 1000.0;
            _statsTimer.Elapsed += (s, e) =>
            {
                var samples = _statsSamples.ToArray();
                _statsSamples.Clear();
                var zeros = (from sample in samples
                             where sample == 0
                             select sample).Count();
                Statistics = String.Format("% zeros: {0:0.00}", (zeros / ((float)samples.Length)) * 100.0f);
            };
            _statsTimer.Start();
        }
        private bool _calculateStatistics;

        public bool CalculateStatistics
        {
            get { return _calculateStatistics; }
            set
            {
                if (_calculateStatistics == value) return;
                _calculateStatistics = value;
                if (_calculateStatistics)
                {
                    _statsTimer.Interval = 1000;
                    _statsTimer.Elapsed += CalculationTimer;
                }
            }
        }

        private readonly System.Timers.Timer _statsTimer = new System.Timers.Timer();
        private readonly List<int> _statsSamples = new List<int>();

        private void CalculationTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            var samples = _statsSamples.ToArray();
            _statsSamples.Clear();
            var zeros = (from sample in samples
                         where sample == 0
                         select sample).Count();
            Statistics = String.Format("% zeros: {0:0.00}", (zeros / ((float)samples.Length)) * 100.0f);
        }

        private readonly List<DataPoint> _dataPoints = new List<DataPoint>();
        public DataPoint[] DataPoints
        {
            get
            {
                var result = _dataPoints.ToArray();
                _dataPoints.Clear();
                return result;
            }
        }

        public string Statistics { get; private set; }

        public UInt16 SensorId { get; private set; }

        private double _lastValue;
        public double LastValue
        {
            get { return _lastValue; }
            set
            {
                _lastValue = value;
                LastValueHex = String.Format("0x{0:X}", (int)_lastValue);
                LastValueTime = DateTime.Now;
                _statsSamples.Add((int)_lastValue);
                _dataPoints.Add(new DataPoint(_dataPoints.Count, _lastValue));
            }
        }

        public string LastValueHex { get; private set; }

        public DateTime LastValueTime { get; private set; }

        public double MinValue { get; private set; }

        public double MaxValue { get; private set; }
    }
}