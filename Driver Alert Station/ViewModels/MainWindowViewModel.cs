// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using Driver_Alert_Station.Model;
using Microsoft.Practices.Prism.Commands;
using PostSharp.Patterns.Model;

namespace Driver_Alert_Station.ViewModels
{
    [NotifyPropertyChanged]
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            ViewClosingCommand = new DelegateCommand<object>(OnViewClosing);
            ClearStatusMessagesCommand = new DelegateCommand<object>(o => AlertsViewModel.AlertMessages.Clear());
            ShowLogFolderCommand = new DelegateCommand<object>(o =>
            {
                var logFileRootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Robot Alert Logs");
                if (!Directory.Exists(logFileRootFolder)) Directory.CreateDirectory(logFileRootFolder);
                var dailyLogFileFolder = Path.Combine(logFileRootFolder, string.Format("{0:yyyy-MM-dd}", DateTime.Now));
                Process.Start(Directory.Exists(dailyLogFileFolder) ? dailyLogFileFolder : logFileRootFolder);
            });
            AlertsViewModel = new AlertsViewModel();
            if (Math.Abs(Properties.Settings.Default.WindowHeight) < double.Epsilon ||
                Math.Abs(Properties.Settings.Default.WindowWidth) < double.Epsilon)
            {
                Properties.Settings.Default.WindowTop = System.Windows.SystemParameters.PrimaryScreenHeight * 0.75;
                Properties.Settings.Default.WindowLeft = 0;
                Properties.Settings.Default.WindowHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 0.25;
                Properties.Settings.Default.WindowWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            }
            LogAlertMessages = Properties.Settings.Default.LogMessages;
            TelemetryData = TelemetryData.Initialize(5802, Dispatcher.CurrentDispatcher);
            RobotViewModel = new RobotViewModel(TelemetryData);
        }

        public TelemetryData TelemetryData { get; private set; }

        public ICommand ViewClosingCommand { get; private set; }

        public ICommand ClearStatusMessagesCommand { get; private set; }

        public ICommand ShowLogFolderCommand { get; set; }

        private bool _logAlertMessages;
        public bool LogAlertMessages
        {
            get { return _logAlertMessages; }
            set
            {
                _logAlertMessages = value;
                if (_logAlertMessages) AlertsViewModel.StartLogging();
                else AlertsViewModel.StopLogging();
                Properties.Settings.Default.LogMessages = _logAlertMessages;
            }
        }

        public AlertsViewModel AlertsViewModel { get; set; }

        public RobotViewModel RobotViewModel { get; private set; }

        private void OnViewClosing(object obj)
        {
            AlertsViewModel.StopLogging();
            Properties.Settings.Default.Save();
        }
    }
}
