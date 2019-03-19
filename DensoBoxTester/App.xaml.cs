using System.Windows;
using System.Windows.Navigation;

namespace DensoBoxTester
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static NavigationService _naviTest;
        public static NavigationService _naviConf;
        public static NavigationService _naviHelp;
        public static NavigationService _naviInfo;

        const string AppName = "DensoBox TESTER";

        private System.Threading.Mutex mutex = new System.Threading.Mutex(false, AppName);
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // ミューテックスの所有権を要求
            if (!mutex.WaitOne(0, false))
            {
                // 既に起動しているため終了させる
                MessageBox.Show($"{AppName}は既に起動しています", "二重起動防止", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                mutex.Close();
                mutex = null;
                this.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
            }
        }


    }
}
