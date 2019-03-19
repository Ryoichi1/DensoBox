using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using static DensoBoxTester.General;
using static System.Threading.Thread;

namespace DensoBoxTester
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {

        Uri uriTestPage = new Uri("Page/Test/Test.xaml", UriKind.Relative);
        Uri uriConfPage = new Uri("Page/Config/Conf.xaml", UriKind.Relative);
        Uri uriHelpPage = new Uri("Page/Help/Help.xaml", UriKind.Relative);

        public MainWindow()
        {
            InitializeComponent();
            App._naviTest = FrameTest.NavigationService;
            App._naviConf = FrameConf.NavigationService;
            App._naviHelp = FrameHelp.NavigationService;
            App._naviInfo = FrameInfo.NavigationService;

            FrameTest.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameConf.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameHelp.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameInfo.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする

            this.DataContext = State.VmMainWindow;


            GetInfo();

            //カレントディレクトリの取得
            State.CurrDir = Directory.GetCurrentDirectory();

            //試験用パラメータのロード
            State.LoadConfigData();

            InitDevices();//非同期処理です

            InitMainWindow();//メインフォーム初期

            SpeechRecognition.Init();
            SpeechRecognition.set();
            Flags.VoiceOn = true;
            SpeechRecognition.音声認識();

        }

        private void CloseProc()
        {
            try
            {
                while (Flags.Initializing周辺機器) ;

                if (Flags.StateMbed)
                    LPC1768.ClosePort();

                SpeechRecognition.Close();
            }
            catch
            {
                MessageBox.Show("例外発生");
            }
            finally
            {
                //データ保存を確実に行う
                if (!State.Save個別データ())
                {
                    MessageBox.Show("個別データの保存に失敗しました");
                }
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            CloseProc();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Flags.Testing)
            {
                e.Cancel = true;
            }
            else
            {
                Flags.StopInit周辺機器 = true;
            }
        }


        private void cbOperator_DropDownClosed(object sender, EventArgs e)
        {
            if (cbOperator.SelectedIndex == -1)
                return;
            Flags.SetOperator = true;
            SetFocus();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            if (Flags.Testing)
                return;

            Flags.SetOperator = false;
            Flags.SetOpecode = false;

            //いったん編集禁止にする
            State.VmMainWindow.ReadOnlyOpecode = true;

            SetFocus();
        }

        private void tbOpecode_TextChanged(object sender, TextChangedEventArgs e)
        {
            var length = State.VmMainWindow.Opecode.Length;
            if (length == 0)
                return;
            else if (length == 1)
            {
                State.VmMainWindow.Opecode += "-";
                tbOpecode.Select(tbOpecode.Text.Length, 0);
            }

            else if (2 <= length && length <= 3)
                return;
            else if (length == 4)
            {
                State.VmMainWindow.Opecode += "-";
                tbOpecode.Select(tbOpecode.Text.Length, 0);
            }
            else if (5 <= length && length <= 8)
                return;
            else if (length == 9)
            {
                State.VmMainWindow.Opecode += "-";
                tbOpecode.Select(tbOpecode.Text.Length, 0);
            }
            else if (9 <= length && length <= 12)
                return;
            else if (length == 13)
            {
                //以降は工番が正しく入力されているかどうかの判定
                if (System.Text.RegularExpressions.Regex.IsMatch(
                    State.VmMainWindow.Opecode, @"^\d-\d\d-\d\d\d\d-\d\d\d$",
                    System.Text.RegularExpressions.RegexOptions.ECMAScript))
                {
                    Flags.SetOpecode = true;
                    SetFocus();
                }

            }

        }

        //アセンブリ情報の取得
        private void GetInfo()
        {
            //アセンブリバージョンの取得
            var asm = Assembly.GetExecutingAssembly();
            var M = asm.GetName().Version.Major.ToString();
            var N = asm.GetName().Version.Minor.ToString();
            var B = asm.GetName().Version.Build.ToString();
            State.AssemblyInfo = M + "." + N + "." + B;

            //コンピュータ名の取得
            State.MachineName = Environment.MachineName;

        }

        //フォームのイニシャライズ
        private void InitMainWindow()
        {
            TabInfo.Header = "";//実行時はエラーインフォタブのヘッダを空白にして作業差に見えないようにする
            TabInfo.IsEnabled = false; //作業差がTABを選択できないようにします

            State.VmMainWindow.EnableOtherButton = true;

            State.VmMainWindow.OperatorEnable = true;
            State.VmMainWindow.ReadOnlyOpecode = true;

        }

        //フォーカスのセット
        public void SetFocus()
        {
            if (!Flags.SetOperator)
            {

                if (!cbOperator.IsFocused)
                    cbOperator.Focus();
                return;
            }

            if (!Flags.SetOpecode)
            {
                if (!tbOpecode.IsFocused)
                    tbOpecode.Focus();
                return;
            }
        }

        private async void TabMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = TabMenu.SelectedIndex;
            if (index == 0)
            {
                Flags.OtherPage = false;//フラグを初期化しておく

                App._naviConf.Refresh();
                App._naviHelp.Refresh();
                App._naviTest.Navigate(uriTestPage);
                SetFocus();//テスト画面に移行する際にフォーカスを必須項目入力欄にあてる

                if (Flags.Testing)
                    return;

                //高速にページ切り替えボタンを押すと異常動作する場合があるので、ページが遷移してから500msec間は、他のページに遷移できないようにする
                State.VmMainWindow.EnableOtherButton = false;
                await Task.Delay(500);
                State.VmMainWindow.EnableOtherButton = true;
            }
            else if (index == 1)
            {
                Flags.OtherPage = true;
                App._naviConf.Navigate(uriConfPage);
                App._naviHelp.Refresh();
                //高速にページ切り替えボタンを押すと異常動作する場合があるので、ページが遷移してから500msec間は、他のページに遷移できないようにする
                State.VmMainWindow.EnableOtherButton = false;
                await Task.Delay(500);
                State.VmMainWindow.EnableOtherButton = true;
            }
            else if (index == 2)
            {
                Flags.OtherPage = true;
                App._naviHelp.Navigate(uriHelpPage);
                App._naviConf.Refresh();
                //高速にページ切り替えボタンを押すと異常動作する場合があるので、ページが遷移してから500msec間は、他のページに遷移できないようにする
                State.VmMainWindow.EnableOtherButton = false;
                await Task.Delay(500);
                State.VmMainWindow.EnableOtherButton = true;

            }
            else if (index == 3)//Infoタブ 作業者がこのタブを選択することはない。 TEST画面のエラー詳細ボタンを押した時にこのタブが選択されるようコードビハインドで記述
            {
                //Flags.OtherPage = true;
                App._naviInfo.Navigate(State.uriOtherInfoPage);
                App._naviConf.Refresh();
                App._naviHelp.Refresh();
            }


        }

        private async void Label_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Flags.Testing)
            {
                return;
            }
            else
            {
                Flags.StopInit周辺機器 = true;
            }

            lbExit.Foreground = Brushes.DodgerBlue;
            await Task.Run(() =>
            {
                Sleep(300);
                CloseProc();
                Environment.Exit(0);
            });


        }
    }
}
