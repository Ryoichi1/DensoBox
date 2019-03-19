using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace DensoBoxTester
{
    public class ViewModelTestStatus : BindableBase
    {
        private double _VoiceSpec;
        public double VoiceSpec
        {
            get { return _VoiceSpec; }
            set
            {
                SetProperty(ref _VoiceSpec, value);
                MessVoiceSpec = "音声認識レベル閾値 " + VoiceSpec.ToString("F0") + "％以上";
            }
        }

        private string _MessVoiceSpec;
        public string MessVoiceSpec
        {
            get { return _MessVoiceSpec; }
            set { SetProperty(ref _MessVoiceSpec, value); }
        }

        private string _音声認識率;
        public string 音声認識率
        {
            get { return _音声認識率; }
            set { SetProperty(ref _音声認識率, value); }
        }





        private string _PicPath;
        public string PicPath
        {
            get { return _PicPath; }
            set { SetProperty(ref _PicPath, value); }
        }





        //判定表示、進捗表示プログレスリング■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        //判定表示 可視性
        private Visibility _DecisionVisibility;
        public Visibility DecisionVisibility
        {
            get { return _DecisionVisibility; }
            set { SetProperty(ref _DecisionVisibility, value); }
        }
        //判定表示　PASS or FAIL
        private string _Decision;
        public string Decision
        {
            get { return _Decision; }
            set { SetProperty(ref _Decision, value); }
        }
        //判定表示の色
        private Brush _ColorDecision;
        public Brush ColorDecision
        {
            get { return _ColorDecision; }
            set { SetProperty(ref _ColorDecision, value); }
        }
        //判定表示の効果
        private DropShadowEffect _EffectDecision;
        public DropShadowEffect EffectDecision
        {
            get { return _EffectDecision; }
            set { SetProperty(ref _EffectDecision, value); }
        }



        //エラー詳細 可視性
        private Visibility _ErrInfoVisibility;
        public Visibility ErrInfoVisibility
        {
            get { return _ErrInfoVisibility; }
            set { SetProperty(ref _ErrInfoVisibility, value); }
        }

        //エラー詳細表示ボタンの可視切り替え
        private Visibility _EnableButtonErrInfo = Visibility.Hidden;
        public Visibility EnableButtonErrInfo
        {
            get { return _EnableButtonErrInfo; }

            set
            {
                SetProperty(ref _EnableButtonErrInfo, value);
            }
        }

        //FAIL判定時に表示するエラー情報
        private string _FailInfo;
        public string FailInfo
        {
            get { return _FailInfo; }
            set { SetProperty(ref _FailInfo, value); }
        }

        //FAIL判定時に表示する試験スペック
        private string _Spec;
        public string Spec
        {
            get { return _Spec; }
            set { SetProperty(ref _Spec, value); }
        }

        //FAIL判定時に表示する計測値
        private string _MeasValue;
        public string MeasValue
        {
            get { return _MeasValue; }
            set { SetProperty(ref _MeasValue, value); }
        }

        //プログレスリングのEndAngle
        private int _進捗度;
        public int 進捗度
        {
            get { return _進捗度; }
            set { SetProperty(ref _進捗度, value); }
        }


        private string _OkCount;
        public string OkCount
        {
            get { return _OkCount; }
            set { SetProperty(ref _OkCount, value); }
        }

        private string _NgCount;
        public string NgCount
        {
            get { return _NgCount; }
            set { SetProperty(ref _NgCount, value); }
        }



        //作業者へのメッセージ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }

        //デートコード■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        private string _Dc;
        public string Dc
        {
            get { return _Dc; }
            set { SetProperty(ref _Dc, value); }
        }


        //周辺機器ステータス表示部■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        private Brush _ColorMbed;
        public Brush ColorMbed
        {
            get { return _ColorMbed; }
            set { SetProperty(ref _ColorMbed, value); }
        }



        private Brush _ColorMultimeter;
        public Brush ColorMultimeter
        {
            get { return _ColorMultimeter; }
            set { SetProperty(ref _ColorMultimeter, value); }
        }



    }
}
