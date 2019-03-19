using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Threading.Thread;

namespace DensoBoxTester
{


    public static class General
    {

        //**************************************************************************
        //検査データの保存　　　　
        //引数：なし
        //戻値：なし
        //**************************************************************************
        private static List<string> MakePassTestData()//TODO:
        {
            var ListData = new List<string>
            {
                "AssemblyVer " + State.AssemblyInfo,
                "TestSpecVer " + State.spec.TestSpecVer,
                System.DateTime.Now.ToString("yyyy年MM月dd日(ddd) HH：mm：ss"),
                State.VmMainWindow.Operator,
                State.VmMainWindow.Opecode,
                State.VmTestStatus.Dc,
                State.VolAc24v.ToString()
            };

            return ListData;
        }

        public static bool SaveTestData()
        {
            try
            {
                var dataList = new List<string>();
                dataList = MakePassTestData();

                var OkDataFilePath = Constants.PassDataPath + State.VmMainWindow.Opecode + ".csv";

                if (!System.IO.File.Exists(OkDataFilePath))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(Constants.fileName_FormatPass, OkDataFilePath);
                }

                // リストデータをすべてカンマ区切りで連結する
                string stCsvData = string.Join(",", dataList);

                // appendをtrueにすると，既存のファイルに追記
                // falseにすると，ファイルを新規作成する
                var append = true;

                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(OkDataFilePath, append, Encoding.GetEncoding("Shift_JIS")))
                {
                    sw.WriteLine(stCsvData);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SaveNgData(List<string> dataList)
        {
            try
            {
                var NgDataFilePath = Constants.FailDataPath + State.VmMainWindow.Opecode + ".csv";
                if (!File.Exists(NgDataFilePath))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(Constants.fileName_FormatFail, NgDataFilePath);
                }

                var stArrayData = dataList.ToArray();
                // リストデータをすべてカンマ区切りで連結する
                string stCsvData = string.Join(",", stArrayData);

                // appendをtrueにすると，既存のファイルに追記
                //         falseにすると，ファイルを新規作成する
                var append = true;

                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(NgDataFilePath, append, Encoding.GetEncoding("Shift_JIS")))
                {
                    sw.WriteLine(stCsvData);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }



        public static void CheckAll周辺機器フラグ()
        {
            Flags.AllOk周辺機器接続 = (Flags.StateMbed && Flags.StateMultimeter);
        }

        /// <summary>
        /// 周辺機器の初期化
        /// </summary>
        public static void InitDevices()//TODO:
        {

            Flags.Initializing周辺機器 = true;

            Task.Run(() =>
            {
                Flags.DoGetDeviceName = true;
                while (Flags.DoGetDeviceName)
                {
                    FindSerialPort.GetDeviceNames();
                    Thread.Sleep(400);
                }

            });



            //LPC1768の初期化
            bool StopMbed = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                        break;

                    Flags.StateMbed = LPC1768.Init();
                    if (Flags.StateMbed)
                        break;

                    Thread.Sleep(500);
                }
                StopMbed = true;
            });

            //マルチメータの初期化
            bool StopMulti = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                        break;

                    Flags.StateMultimeter = InitGdm();
                    if (Flags.StateMultimeter)
                        break;

                    Thread.Sleep(500);
                }
                StopMulti = true;
            });

            Task.Run(() =>
            {
                while (true)
                {
                    CheckAll周辺機器フラグ();
                    var IsAllStopped = StopMbed && StopMulti;

                    if (Flags.AllOk周辺機器接続 || IsAllStopped) break;
                    Thread.Sleep(400);

                }
                Flags.DoGetDeviceName = false;
                Thread.Sleep(500);
                Flags.Initializing周辺機器 = false;
            });

        }

        /// <summary>
        /// 非同期でSWチェック
        /// </summary>
        public static void CheckSw()
        {
            Task.Run(() =>
            {
                try
                {
                    var buff = "";
                    while (true)
                    {
                        if (Flags.FlagStopSwCheck)
                        {
                            Flags.CancelPushed = true;
                            return;
                        }

                        buff = ReadInput();
                        if (buff == null || buff.Length != 3)
                        {
                            Sleep(400);
                            continue;
                        }

                        if (buff[0] == '0' && buff[1] == '0')
                        {
                            Sleep(400);
                            continue;
                        }

                        if (buff[0] == '1')
                            Flags.OkPushed = true;
                        else
                            Flags.CancelPushed = true;
                        return;
                    }
                }
                finally
                {
                }
            });
        }

        /// <summary>
        /// mbedにResetIoコマンド送信しIOの出力を全OFFする
        /// </summary>
        public static void ResetIo() => LPC1768.SendData("ResetIo");


        /// <summary>
        /// mbed内部のOK/Cancelスイッチのステートを初期化する
        /// </summary>
        public static void ResetSwState() => LPC1768.SendData("ResetSwState");
        public static bool InitGdm()
        {
            LPC1768.SendData("InitGdm");
            return LPC1768.ReceiveData == "OK" ? true : false;
        }


        /// <summary>
        /// OKスイッチ、Cancelスイッチ、CN4-3の状態をまとめて読み込む
        /// </summary>
        public static string ReadInput()
        {
            LPC1768.SendData("ReadInput");
            return LPC1768.ReceiveData;//ex　100 → OKボタン押されてる、Cancelボタン押されてない、CN4-3-4間開放状態
        }

        public static double MeasAc24vR100()
        {
            LPC1768.SendData("MeasR100");

            if (Double.TryParse(LPC1768.ReceiveData, out var result))
                return result;
            else
                return -999.0;
        }

        public static double MeasAc24vR10()
        {
            LPC1768.SendData("MeasR10");

            if (Double.TryParse(LPC1768.ReceiveData, out var result))
                return result;
            else
                return -999.0;
        }

        public static void PowSupply(bool sw, int wait = 500)
        {

            if (Flags.PowOn == sw) return;

            if (sw)
            {
                SetK1(true);
                Flags.PowOn = true;
                Sleep(wait);
            }
            else
            {
                SetK1(false);
                Flags.PowOn = false;
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>
        private static void ResetOkCancelFlag()
        {
            Flags.OkPushed = false;
            Flags.CancelPushed = false;
            Flags.FlagStopSwCheck = false;
        }



        public static bool CheckOkCancel()
        {
            try
            {
                ResetSwState();
                ResetOkCancelFlag();
                Sleep(1000);
                CheckSw();
                //CheckOkCancelByVoice();
                while (true)
                {
                    if (Flags.OkPushed)
                        return true;
                    if (Flags.CancelPushed)
                        return false;
                }
            }
            finally
            {
                Flags.FlagStopSwCheck = true;
            }
        }

        public static void CheckStartFlag()
        {
            ResetSwState();
            ResetOkCancelFlag();
            CheckSw();
            //CheckOkCancelByVoice();
        }

        ///////////////////////////////////////////////////////////////////////////////////////



        public static void SetK1(bool sw) => LPC1768.SendData("Out,P21," + (sw ? "1" : "0"));
        public static void SetK2(bool sw) => LPC1768.SendData("Out,P22," + (sw ? "1" : "0"));
        public static void SetK3(bool sw) => LPC1768.SendData("Out,P23," + (sw ? "1" : "0"));
        public static void SetRL1(bool sw) => LPC1768.SendData("Out,P24," + (sw ? "1" : "0"));
        public static void SetRL2(bool sw) => LPC1768.SendData("Out,P25," + (sw ? "1" : "0"));
        public static void SetRL3(bool sw) => LPC1768.SendData("Out,P26," + (sw ? "1" : "0"));
        public static void SetRL4(bool sw) => LPC1768.SendData("Out,P27," + (sw ? "1" : "0"));
        public static void SetRL5(bool sw) => LPC1768.SendData("Out,P28," + (sw ? "1" : "0"));

    }

}

