using System;
using System.Threading.Tasks;
using static RBC250Tester.Constants;
using static RBC250Tester.GeneralFuncForTest;
using static System.Threading.Thread;

namespace RBC250Tester.TestItems
{
    static class デジタル入力_Bcr
    {
        public enum 外部信号入力1 { 風圧スイッチ, ガス圧スイッチLH }
        public enum 外部信号入力2 { 開放_OFF, 短絡_OFF, 開放_ON, 短絡_ON }


        private static void Off入力信号()
        {
            Din風圧スイッチ(false);//CN709
            Dinガス圧LH(false);//CN706 CN708
            Din感震器(false);//CN703
            Din缶体サーモ(false);//CN705
            Din圧力スイッチ(false);//CN707

        }

        /// <summary>
        /// 該当ビットが立っていたらtrueを返す　メソッドのコール側で戻り値が短絡なのか開放なのかを判断する
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private static (bool reMain, bool reSub) CheckBcr(short refData, int address)
        {

            if (!TargetBCR.SendCommand(TargetBCR.CORE.MAIN, TargetBCR.RW.READ, address, "0001")) return (false, false);
            var dataMain = Convert.ToInt32(TargetBCR.AllWordData.Substring(0, 4), 16);

            if (!TargetBCR.SendCommand(TargetBCR.CORE.SUB, TargetBCR.RW.READ, address, "0001")) return (false, false);
            var dataSub = Convert.ToInt32(TargetBCR.AllWordData.Substring(0, 4), 16);

            var reMain = (dataMain & refData) == refData;
            var reSub = (dataSub & refData) == refData;

            return (reMain, reSub);
        }


        public static async Task<bool> Check外部信号入力1()
        {
            var AllResult = false;
            var IsTesting短絡 = false;
            var Result = (reMain: false, reSub: false);

            Flags.AddDecision = false;
            short RefData = 0;

            if (!await CheckBcrComm()) return false;

            return await Task<bool>.Run(() =>
            {
                try
                {
                    foreach (var n in Enum.GetValues(typeof(外部信号入力1)))
                    {
                        var Name = (外部信号入力1)n;

                        State.VmTestStatus.TestLog += $"\n    {Name.ToString()} 短絡";
                        IsTesting短絡 = true;
                        //短絡処理
                        switch (Name)
                        {
                            case 外部信号入力1.風圧スイッチ:
                                RefData = RefB1;
                                Din風圧スイッチ(true);
                                break;
                            case 外部信号入力1.ガス圧スイッチLH:
                                RefData = RefB0;
                                Dinガス圧LH(true);
                                break;
                        }
                        Sleep(400);
                        Result = CheckBcr(RefData, 3026);
                        AllResult = Result.reMain && Result.reSub;
                        if (AllResult)
                        {
                            State.VmTestStatus.TestLog += "---PASS";
                        }
                        else
                        {
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }


                        State.VmTestStatus.TestLog += $"\n {Name.ToString()} 開放";
                        IsTesting短絡 = false;
                        //開放処理
                        Off入力信号();
                        Sleep(400);

                        Result = CheckBcr(RefData, 3026);

                        AllResult = !Result.reMain && !Result.reSub;//開放なので論理が逆になります
                        if (AllResult)
                        {
                            State.VmTestStatus.TestLog += "---PASS";
                        }
                        else
                        {
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    State.VmTestStatus.TestLog += "\n";
                    //スイッチをOFFする処理
                    Off入力信号();

                    if (!AllResult)//どちらかがNGなら下記ビューモデルを更新する
                    {
                        var 規格 = IsTesting短絡 ? "短絡 [1]" : "開放 [0]";

                        var m = Result.reMain ? "1" : "0";
                        var s = Result.reSub ? "1" : "0";
                        var 計測値 = $"Main [{m}]  Sub [{s}]";

                        State.VmTestStatus.Spec = "規格値 : " + 規格;
                        State.VmTestStatus.MeasValue = "計測値 : " + 計測値;
                    }
                }
            });
        }


        private static bool Set高水位許可(bool sw) => TargetAPP.SendAscii(904, sw ? 0 : 0x0010);

        public static async Task<bool> Check外部信号入力2()
        {
            var AllResult = false;
            var IsTesting短絡 = false;
            var Result = (reMain: false, reSub: false);

            Flags.AddDecision = false;

            if (!await CheckBcrComm()) return false;

            return await Task<bool>.Run(() =>
            {
                try
                {
                    //事前にCN703とCN705をONする
                    Din感震器(true);
                    Din缶体サーモ(true);

                    foreach (var n in Enum.GetValues(typeof(外部信号入力2)))
                    {
                        var Name = (外部信号入力2)n;

                        var 圧力スイッチ = Name.ToString().Split('_')[0];
                        var 高水位許可 = Name.ToString().Split('_')[1];
                        State.VmTestStatus.TestLog += $"\n    圧力スイッチ{圧力スイッチ}/高水位許可{高水位許可}";
                        IsTesting短絡 = true;
                        //短絡処理
                        switch (Name)
                        {
                            case 外部信号入力2.開放_OFF:
                                Din圧力スイッチ(false);
                                Set高水位許可(false);
                                IsTesting短絡 = false;
                                break;
                            case 外部信号入力2.短絡_OFF:
                                Din圧力スイッチ(true);
                                Set高水位許可(false);
                                IsTesting短絡 = false;
                                break;
                            case 外部信号入力2.開放_ON:
                                Din圧力スイッチ(false);
                                Set高水位許可(true);
                                IsTesting短絡 = false;
                                break;
                            case 外部信号入力2.短絡_ON:
                                Din圧力スイッチ(true);
                                Set高水位許可(true);
                                IsTesting短絡 = true;
                                break;
                        }
                        Sleep(1000);

                        Result = CheckBcr(RefB2, 3026);

                        if (IsTesting短絡)
                        {
                            AllResult = Result.reMain && Result.reSub;
                            if (AllResult)
                            {
                                State.VmTestStatus.TestLog += "---PASS";
                            }
                            else
                            {
                                State.VmTestStatus.TestLog += "---FAIL";
                                return false;
                            }
                        }
                        else
                        {
                            AllResult = !Result.reMain && !Result.reSub;
                            if (AllResult)
                            {
                                State.VmTestStatus.TestLog += "---PASS";
                            }
                            else
                            {
                                State.VmTestStatus.TestLog += "---FAIL";
                                return false;
                            }
                        }
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    State.VmTestStatus.TestLog += "\n";
                    //スイッチをOFFする処理
                    Off入力信号();
                    Set高水位許可(false);

                    if (!AllResult)//どちらかがNGなら下記ビューモデルを更新する
                    {
                        var 規格 = IsTesting短絡 ? "短絡 [1]" : "開放 [0]";

                        var m = Result.reMain ? "1" : "0";
                        var s = Result.reSub ? "1" : "0";
                        var 計測値 = $"Main [{m}]  Sub [{s}]";

                        State.VmTestStatus.Spec = "規格値 : " + 規格;
                        State.VmTestStatus.MeasValue = "計測値 : " + 計測値;
                    }
                }
            });
        }















    }
}
