using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonCode;
using PostSharp.Patterns.Model;

namespace Driver_Alert_Station.ViewModels
{
    [NotifyPropertyChanged]
    public class SwerveViewModel
    {
        private PropertyObserver<SwerveViewModel> _observer; 
        public SwerveViewModel(float maxSpeed)
        {
            MaxSpeed = maxSpeed;
            _observer = new PropertyObserver<SwerveViewModel>(this)
                .RegisterHandler(p => p.Speed, UpdateSpeedArrow);
        }

        public float RotationAngle { get; set; }
        public float Speed { get; set; }
        public float MaxSpeed { get; set; }

        public double SpeedArrowHeight { get; private set; }
        public Visibility SpeedArrowVisibility { get; private set; }
        private void UpdateSpeedArrow()
        {
            if (Math.Abs(Speed) < float.Epsilon)
            {
                SpeedArrowHeight = 0;
                SpeedArrowVisibility = Visibility.Collapsed;
            }
            else
            {
                SpeedArrowHeight = (Speed/MaxSpeed)*40;
                SpeedArrowVisibility = Visibility.Visible;
            }
        }
    }
}
