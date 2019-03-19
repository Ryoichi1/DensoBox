using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Threading.Thread;

namespace DensoBoxTester
{
    public static class UiControl
    {

        //インスタンス変数の宣言
        public static SoundPlayer player = null;
        public static SoundPlayer soundEdy = null;
        public static SoundPlayer soundFail = null;
        public static SoundPlayer soundSuccess = null;
        public static SoundPlayer soundNotice = null;
        public static SoundPlayer soundStart = null;


        public static SolidColorBrush OnBrush = new SolidColorBrush();
        public static SolidColorBrush OffBrush = new SolidColorBrush();
        public static SolidColorBrush NgBrush = new SolidColorBrush();

        static UiControl()
        {

            //オーディオリソースを取り出す
            soundEdy = new SoundPlayer(@"Resources\Wav\Edy.wav");
            soundFail = new SoundPlayer(@"Resources\Wav\Fail.wav");
            soundSuccess = new SoundPlayer(@"Resources\Wav\Success.wav");
            soundNotice = new SoundPlayer(@"Resources\Wav\Notice.wav");
            soundStart = new SoundPlayer(@"Resources\Wav\Start.wav");

            OffBrush.Color = Colors.Transparent;

            OnBrush.Color = Colors.DodgerBlue;
            OnBrush.Opacity = Constants.PanelOpacity;

            NgBrush.Color = Colors.Magenta;
            NgBrush.Opacity = Constants.PanelOpacity;
        }
        public static void ClearCommLog()
        {
            State.VmComm.MbedLog = "";
        }

        public static void Show()
        {
            var T = 0.3;
            var t = 0.005;

            State.setting.OpacityTheme = State.VmMainWindow.ThemeOpacity;
            //10msec刻みでT秒で元のOpacityに戻す
            int times = (int)(T / t);

            State.VmMainWindow.ThemeOpacity = 0;
            Task.Run(() =>
            {
                while (true)
                {

                    State.VmMainWindow.ThemeOpacity += State.setting.OpacityTheme / (double)times;
                    Sleep((int)(t * 1000));
                    if (State.VmMainWindow.ThemeOpacity >= State.setting.OpacityTheme) return;

                }
            });
        }

        public static void SetRadius(bool sw)
        {
            var R = 20;
            var T = 0.45;//アニメーションが完了するまでの時間（秒）
            var t = 0.005;//（秒）

            //5msec刻みでT秒で元のOpacityに戻す
            int times = (int)(T / t);


            Task.Run(() =>
            {
                if (sw)
                {
                    while (true)
                    {
                        State.VmMainWindow.ThemeBlurEffectRadius += R / (double)times;
                        Sleep((int)(t * 1000));
                        if (State.VmMainWindow.ThemeBlurEffectRadius >= R) return;

                    }
                }
                else
                {
                    var CurrentRadius = State.VmMainWindow.ThemeBlurEffectRadius;
                    while (true)
                    {
                        CurrentRadius -= R / (double)times;
                        if (CurrentRadius > 0)
                        {
                            State.VmMainWindow.ThemeBlurEffectRadius = CurrentRadius;
                        }
                        else
                        {
                            State.VmMainWindow.ThemeBlurEffectRadius = 0;
                            return;
                        }
                        Sleep((int)(t * 1000));
                    }
                }

            });
        }

        //WAVEファイルを再生する（非同期で再生）
        public static void PlaySoundAsync(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.Play();
        }

        //再生されているWAVEファイルを止める
        public static void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }

        public static void ResetViewModel()//TODO:
        {
            //ViewModelの初期化 判定表示
            State.VmTestStatus.進捗度 = 0;
            State.VmTestStatus.OkCount = State.setting.TodayOkCount.ToString() + "台";
            State.VmTestStatus.NgCount = State.setting.TodayNgCount.ToString() + "台";
            State.VmTestStatus.FailInfo = "";
            State.VmTestStatus.Spec = "";
            State.VmTestStatus.MeasValue = "";

            //通信ログのクリア
            ClearCommLog();

            ////ViewModelの初期化 作業者へのメッセージ
            //State.VmTestStatus.Message = Constants.MessSet;

            //各種可視化の設定
            State.VmTestStatus.DecisionVisibility = System.Windows.Visibility.Hidden;

            //他ページへの遷移を許可する
            State.VmMainWindow.EnableOtherButton = true;

            //各種フラグの初期化
            Flags.PowOn = false;
            Flags.Testing = false;

            State.VmMainWindow.OperatorEnable = true;

            State.VmTestStatus.音声認識率 = "";
            State.VmTestStatus.PicPath = null;
            SetRadius(false);

        }





    }
}
