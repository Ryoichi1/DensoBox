using System.Windows.Media;

namespace DensoBoxTester
{
    public static class Flags
    {
        static SolidColorBrush OnBrush = new SolidColorBrush();
        static SolidColorBrush OffBrush = new SolidColorBrush();
        static SolidColorBrush NgBrush = new SolidColorBrush();


        public static bool OkPushed { get; set; }//
        public static bool CancelPushed { get; set; }//

        public static bool VoiceOn { get; set; } = true;


        public static bool DoGetDeviceName { get; set; }
        public static bool OtherPage { get; set; }

        //試験開始時に初期化が必要なフラグ
        public static bool StopInit周辺機器 { get; set; }
        public static bool Initializing周辺機器 { get; set; }
        public static bool Testing { get; set; }
        public static bool PowOn { get; set; }//メイン電源ON/OFF


        public static bool FlagStopSwCheck { get; set; }
        public static bool FlagStop音声認識 { get; set; } = true;

        static Flags()
        {
            OffBrush.Color = Colors.Transparent;

            OnBrush.Color = Colors.DodgerBlue;
            OnBrush.Opacity = Constants.PanelOpacity;

            NgBrush.Color = Colors.Magenta;
            NgBrush.Opacity = Constants.PanelOpacity;

        }


        //周辺機器のステータス

        private static bool _StateMbed;
        public static bool StateMbed
        {
            get { return _StateMbed; }
            set
            {
                _StateMbed = value;
                State.VmTestStatus.ColorMbed = value ? OnBrush : NgBrush;
            }
        }


        private static bool _StateMultimeter;
        public static bool StateMultimeter
        {
            get { return _StateMultimeter; }
            set
            {
                _StateMultimeter = value;
                State.VmTestStatus.ColorMultimeter = value ? OnBrush : NgBrush;
            }
        }



        public static bool AllOk周辺機器接続 { get; set; }

        //フラグ
        private static bool _SetOperator;
        public static bool SetOperator
        {
            get { return _SetOperator; }
            set
            {
                _SetOperator = value;
                if (value)
                {
                    if (SetOpecode)
                        return;

                    //工番入力許可する
                    State.VmMainWindow.ReadOnlyOpecode = false;
                }
                else
                {
                    State.VmMainWindow.Operator = "";
                    State.VmMainWindow.ReadOnlyOpecode = true;//工番 入力不可とする
                    State.VmMainWindow.SelectIndexOperatorCb = -1;
                }
            }
        }


        private static bool _SetOpecode;
        public static bool SetOpecode
        {
            get { return _SetOpecode; }

            set
            {
                _SetOpecode = value;

                if (value)
                {
                    State.VmMainWindow.ReadOnlyOpecode = true;//工番が確定したので、編集不可とする
                }
                else
                {
                    State.VmMainWindow.ReadOnlyOpecode = false;
                    State.VmMainWindow.Opecode = "";
                }

            }
        }


    }
}
