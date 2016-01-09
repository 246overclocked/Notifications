// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using PostSharp.Patterns.Model;

namespace BeagleBoneAnalogMonitor.ViewModels
{
    [NotifyPropertyChanged]
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            SubmitCommand = new DelegateCommand<object>(OnSubmit);
            AnalogChannelsViewModel = new AnalogChannelsViewModel();
            ResetCommand = new DelegateCommand(OnReset);
        }

        public ICommand SubmitCommand { get; private set; }

        public ICommand ResetCommand { get; private set; }

        public AnalogChannelsViewModel AnalogChannelsViewModel { get; set; }

        private void OnSubmit(object obj) {}

        private void OnReset() {}
    }
}
