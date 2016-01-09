using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;
using BeagleBoneAnalogMonitor.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OxyPlot;
using PostSharp.Patterns.Model;

namespace BeagleBoneAnalogMonitor.ViewModels
{
    [NotifyPropertyChanged]
    public class AnalogChannelsViewModel : BindableBase
    {
        public AnalogChannels AnalogChannels { get; private set; }
        public AnalogChannelsViewModel()
        {
            AnalogChannels = new AnalogChannels(5800);
            ClickedOnChannelCommand = new DelegateCommand<object>(o =>
            {
                Debug.WriteLine(o);
                DataPoints = null;
            });
        }
        public ICommand ClickedOnChannelCommand { get; private set; }
        public DataPoint[] DataPoints { get; set; }
    }
}   
