using System;
using System.Threading.Tasks;
using System.Windows;
using static DensoBoxTester.General;
using static DensoBoxTester.UiControl;

namespace DensoBoxTester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class Decision
    {
        public Decision()
        {
            this.InitializeComponent();
            this.DataContext = State.VmTestStatus;
        }

        private void ClosingProc()
        {
            StopSound();

            //テーマ透過度を元に戻す
            State.VmMainWindow.ThemeOpacity = State.CurrentThemeOpacity;
            ResetViewModel();
            State.VmMainWindow.TabIndex = 0;
            State.VmTestStatus.Message = "";
            //PlaySoundAsync(soundSuccess);
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (CheckOkCancel())
                        break;
                }
            });
            ClosingProc();
        }

    }
}
