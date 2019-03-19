
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static DensoBoxTester.UiControl;
using static DensoBoxTester.General;

namespace DensoBoxTester
{
    public class Main
    {

        DropShadowEffect effect判定表示PASS;
        DropShadowEffect effect判定表示FAIL;

        public Main()
        {
            effect判定表示PASS = new DropShadowEffect();
            effect判定表示PASS.Color = Colors.Aqua;
            effect判定表示PASS.Direction = 0;
            effect判定表示PASS.ShadowDepth = 0;
            effect判定表示PASS.Opacity = 1.0;
            effect判定表示PASS.BlurRadius = 80;

            effect判定表示FAIL = new DropShadowEffect();
            effect判定表示FAIL.Color = Colors.HotPink; ;
            effect判定表示FAIL.Direction = 0;
            effect判定表示FAIL.ShadowDepth = 0;
            effect判定表示FAIL.Opacity = 1.0;
            effect判定表示FAIL.BlurRadius = 40;

        }

        public async Task StartCheck()
        {
            var dis = App.Current.Dispatcher;

            //while (true)
            //{
            await Task.Run(() =>
            {

                while (true)
                {

                    Thread.Sleep(200);
                    if (Flags.OtherPage) break;
                    Thread.Sleep(200);


                    //作業者名、工番が正しく入力されているかの判定
                    if (!Flags.SetOperator)
                    {
                        State.VmTestStatus.Message = Constants.MessOperator;
                        continue;
                    }

                    if (!Flags.SetOpecode)
                    {
                        State.VmTestStatus.Message = Constants.MessOpecode;
                        continue;
                    }


                    General.CheckAll周辺機器フラグ();
                    if (!Flags.AllOk周辺機器接続)
                    {
                        State.VmTestStatus.Message = Constants.MessCheckConnectMachine;
                        continue;
                    }


                    State.VmTestStatus.Message = Constants.MessPushStart;
                    Thread.Sleep(1000);
                    while (true)
                    {
                        CheckStartFlag();
                        while (true)
                        {
                            if (Flags.CancelPushed)
                                break;

                            if (Flags.OtherPage || Flags.OkPushed)
                            {
                                Flags.FlagStopSwCheck = true;
                                //Flags.FlagStop音声認識 = true;
                                return;
                            }

                            if (!Flags.SetOperator || !Flags.SetOpecode)
                            {
                                Flags.FlagStopSwCheck = true;
                                //Flags.FlagStop音声認識 = true;
                                break;//最初からやり直し
                            }
                        }
                    }
                }
            });

            //

            if (Flags.OtherPage)//待機中に他のページに遷移したらメソッドを抜ける
            {
                return;
            }


            State.VmMainWindow.EnableOtherButton = false;
            State.VmMainWindow.OperatorEnable = false;


            PlaySoundAsync(soundStart);
            await Test();//メインルーチンへ


            //試験合格後、ラベル貼り付けページを表示する場合は下記のステップを追加すること
            //if (Flags.ShowLabelPage) return;

            //日常点検合格、一項目試験合格、試験NGの場合は、Whileループを繰り返す
            //通常試験合格の場合は、ラベル貼り付けフォームがロードされた時点で、一旦StartCheckメソッドを終了します
            //その後、ラベル貼り付けフォームが閉じられた後に、Test.xamlがリロードされ、そのフォームロードイベントでStartCheckメソッドがコールされます

            //}

        }

        //メインルーチンで使用するメンバ
        int StepNo = 0;
        string Title = "";

        /// <summary>
        ///メインルーチン 
        /// </summary>
        /// <returns></returns>
        public async Task Test()
        {
            var IsTestNg = false;
            var IsOkSaveData = false;
            InitBeforeTest();
            var テスト項目最新 = ExtractInspectionItems();

            try
            {
                //IO初期化
                await Task.Delay(200);


                foreach (var d in テスト項目最新.Select((s, i) => new { i, s }))
                {
                    State.VmTestStatus.EnableButtonErrInfo = System.Windows.Visibility.Hidden;
                    StepNo = d.s.StepNo;
                    Title = d.s.Value;

                    if (!await d.s.testProc())
                    {
                        IsTestNg = true;
                        break;
                    }

                    await ProcAfeterOneItemPass(d.i, テスト項目最新.Count());
                }

                await Task.Delay(250);
                //↓↓すべての項目が合格した時の処理です↓↓
                await CommonProcAfterTesting();

                if (!IsTestNg)
                    IsOkSaveData = await SaveData();

                if (IsTestNg || !IsOkSaveData)
                {
                    await FailProc();
                }
                else
                {
                    await PassProc();
                }

            }
            catch
            {
                //例外なのでリセット処理とカメラ停止を強制的に行う
                System.Windows.Forms.MessageBox.Show("想定外の例外発生DEATH！！！\r\n申し訳ありませんが再起動してください");
                Environment.Exit(0);

            }
            finally
            {
                //SbRingLoad();

                State.uriOtherInfoPage = new Uri("Page/Test/Decision.xaml", UriKind.Relative);
                State.VmMainWindow.TabIndex = 3;
            }
        }





        /// <summary>
        /// 試験開始前に各種フラグ、ビューの初期化を行う
        /// </summary>
        private void InitBeforeTest()
        {
            State.VmMainWindow.EnableOtherButton = false;
            State.VmMainWindow.OperatorEnable = false;

            //Flags.ClickRunButton = false;
            Flags.Testing = true;
            State.VmTestStatus.Message = Constants.MessWait;

            //現在のテーマ透過度の保存
            State.CurrentThemeOpacity = State.VmMainWindow.ThemeOpacity;

            SetRadius(true);
        }

        /// <summary>
        /// テスト項目の抽出
        /// </summary>
        private List<TestDetail> ExtractInspectionItems()
        {
            var テスト項目最新 = new List<TestDetail>();

            テスト項目最新 = State.テスト項目;
            return テスト項目最新;
        }

        /// <summary>
        /// 各ステップが合格したあとの処理
        /// </summary>
        /// <returns></returns>
        private async Task ProcAfeterOneItemPass(int stepCount, int totalStepCount)
        {
            State.VmTestStatus.Spec = "規格値 : ---";
            State.VmTestStatus.MeasValue = "計測値 : ---";
            State.VmTestStatus.Message = Constants.MessWait;


            //プログレスバーの更新
            await Task.Run(() =>
            {
                var CurrentProgValue = State.VmTestStatus.進捗度;
                var NextProgValue = (int)(((stepCount + 1) / (double)totalStepCount) * 100);
                var 変化量 = NextProgValue - CurrentProgValue;
                foreach (var p in Enumerable.Range(1, 変化量))
                {
                    State.VmTestStatus.進捗度 = CurrentProgValue + p;

                }
            });
        }

        private async Task<bool> SaveData()
        {
            return await Task<bool>.Run(() =>
            {
                if (!General.SaveTestData())
                {
                    StepNo = 500;
                    Title = "検査データ保存";
                    return false;
                }

                return true;
            });
        }


        private async Task PassProc()
        {
            //当日試験不合格数をインクリメント ビューモデルはまだ更新しない
            State.setting.TodayOkCount++;

            State.VmTestStatus.ColorDecision = Brushes.AliceBlue;
            State.VmTestStatus.Decision = "PASS";
            State.VmTestStatus.EffectDecision = effect判定表示PASS;

            await Task.Delay(300);
            SetErrInfo(false);

            PlaySoundAsync(soundEdy);

        }

        private async Task FailProc()
        {
            //当日試験不合格数をインクリメント ビューモデルはまだ更新しない
            State.setting.TodayNgCount++;
            await Task.Delay(100);

            State.VmTestStatus.ColorDecision = Brushes.AliceBlue;
            State.VmTestStatus.Decision = "FAIL";
            State.VmTestStatus.EffectDecision = effect判定表示FAIL;

            SetErrorMessage(StepNo, Title);

            var NgDataList = new List<string>()
                                    {
                                        State.VmMainWindow.Opecode,
                                        State.VmMainWindow.Operator,
                                        System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                        State.VmTestStatus.FailInfo,
                                        State.VmTestStatus.Spec,
                                        State.VmTestStatus.MeasValue
                                    };

            General.SaveNgData(NgDataList);

            SetErrInfo(true);

            PlaySoundAsync(soundFail);

        }

        private async Task CommonProcAfterTesting()
        {
            ResetIo();
            await Task.Delay(500);
        }

        //フォームきれいにする処理いろいろ


        private void SetErrorMessage(int stepNo, string title)
        {
            State.VmTestStatus.FailInfo = "<エラーコード " + stepNo.ToString() + ">   " + title + "異常";
        }


        private void SetErrInfo(bool sw)
        {
            State.VmTestStatus.ErrInfoVisibility = sw ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }



    }
}
