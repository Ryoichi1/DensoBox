using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static DensoBoxTester.UiControl;

namespace DensoBoxTester
{
    /// <summary>
    /// EditOpeList.xaml の相互作用ロジック
    /// </summary>
    public partial class EditOpeList
    {
        private ViewModelEdit vmEdit;

        public EditOpeList()
        {
            this.InitializeComponent();
            vmEdit = new ViewModelEdit();

            canvasEdit.DataContext = vmEdit;
            canvasVoice.DataContext = State.VmTestStatus;
            canvasTheme.DataContext = State.VmMainWindow;
            SliderOpacity.Value = State.setting.OpacityTheme;


        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (vmEdit.Name == "") return;
            // 入力された名前を追加
            vmEdit.ListOperator.Add(vmEdit.Name);
            vmEdit.ListOperator = new List<string>(vmEdit.ListOperator);
            vmEdit.Name = "";
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (vmEdit.SelectIndex == -1) return;
            // 選択された項目を削除
            vmEdit.ListOperator.RemoveAt(vmEdit.SelectIndex);
            if (vmEdit.ListOperator.Count == 0)
            {
                vmEdit.ListOperator = new List<string>();
            }
            else
            {
                vmEdit.ListOperator = new List<string>(vmEdit.ListOperator);
            }
        }


        private async void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.Background = Brushes.DodgerBlue;
            //保存する処理
            State.VmMainWindow.ListOperator = new List<string>(vmEdit.ListOperator);
            PlaySoundAsync(soundSuccess);
            await Task.Delay(150);
            buttonSave.Background = Brushes.Transparent;
            //App._navi.Refresh();
        }

        private void Pic1_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.VmMainWindow.Theme = "Resources/Pic/taki.jpg";
            Show();
        }

        private void Pic2_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.VmMainWindow.Theme = "Resources/Pic/moon.jpg";
            Show();
        }

        private void Pic3_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.VmMainWindow.Theme = "Resources/Pic/sumetal.jpg";
            Show();
        }

        private void Pic4_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            State.VmMainWindow.Theme = "Resources/Pic/baby4.jpg";
            Show();
        }

        private void SliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            State.setting.OpacityTheme = State.VmMainWindow.ThemeOpacity;
        }

        private void RbVoiceOn_Checked(object sender, RoutedEventArgs e)
        {
            if (!Flags.VoiceOn)
            {
                SpeechRecognition.音声認識();
                Flags.VoiceOn = true;
            }
        }

        private void RbVoiceOff_Checked(object sender, RoutedEventArgs e)
        {
            Flags.VoiceOn = false;
            Flags.FlagStop音声認識 = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Flags.VoiceOn)
                rbVoiceOn.IsChecked = true;
            else
                rbVoiceOff.IsChecked = true;
        }
    }
}
