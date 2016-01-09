using System.ComponentModel;
using System.Windows.Data;
using CommonCode;
using Driver_Alert_Station.Model;
using Microsoft.Practices.Prism.Mvvm;
using PostSharp.Patterns.Model;
using System.Threading.Tasks.Dataflow;

namespace Driver_Alert_Station.ViewModels
{
    [NotifyPropertyChanged]
    public class AlertsViewModel : BindableBase
    {
        private readonly AlertMessageReceiver _receiver;
        public AlertsViewModel()
        {
            _receiver = new AlertMessageReceiver(5801);
            AlertMessageView = (ListCollectionView)CollectionViewSource.GetDefaultView(AlertMessages);
            AlertMessageView.SortDescriptions.Add(new SortDescription("ReceiveTime", ListSortDirection.Descending));
            IsTimestampColumnVisible = true;
            IsMessageColumnVisible = true;
            //AlertMessages.Add(AlertMessage.Create(true, "Initiating startup sequence", "startup2.wav", "DEBUG"));
        }

        public ObservableList<AlertMessage> AlertMessages { get { return _receiver; }  }
        public CollectionView AlertMessageView { get; private set; }
        public bool IsTimestampColumnVisible { get; set; }
        public bool IsMessageColumnVisible { get; set; }
        
        public void StartLogging() { _receiver.StartLogging(); }
        public void StopLogging() { _receiver.StopLogging(); }
    }
}
