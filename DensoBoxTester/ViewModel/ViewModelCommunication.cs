using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Media;

namespace DensoBoxTester
{

    public class ViewModelCommunication : BindableBase
    {

        //LPC1768通信ログ
        private string _MbedLog;
        public string MbedLog
        {
            get { return _MbedLog; }
            set { SetProperty(ref _MbedLog, value); }
        }

    }
}
