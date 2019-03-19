using System.IO.Ports;
using System.Linq;

namespace DensoBoxTester
{
    public static class LPC1768
    {
        const string ID_LPC1768 = "DensoBoxChecker";
        //private const string ComName = "mbed Serial Port ";
        private const string ComName = "Bluetooth リンク経由の標準シリアル";
        //変数の宣言（インスタンスメンバーになります）
        //private static SerialPort portUsb;
        private static SerialPort port;//Bluetooth
        public static string ReceiveData { get; set; }//LPC1768から受信した生データ


        static LPC1768()
        {
            port = new SerialPort();
        }


        public static bool Init()
        {
            var result = false;
            try
            {
                var comNum = FindSerialPort.GetComNo(ComName);
                if (comNum.Count() != 1) return false;

                if (!port.IsOpen)
                {
                    //Agilent34401A用のシリアルポート設定
                    port.PortName = comNum[0]; //この時点で既にポートが開いている場合COM番号は設定できず例外となる（イニシャライズは１回のみ有効）
                    port.BaudRate = 9600;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    port.NewLine = ("\r\n");
                    port.Open();
                }

                //クエリ送信
                if (!SendData("*IDN?", setLog:false)) return false;
                return result = ReceiveData.Contains(ID_LPC1768);
            }
            catch
            {
                return result = false;
            }
            finally
            {
                if (!result)
                {
                    ClosePort();
                }
            }
        }

        public static bool SendData(string cmd, int Wait = 4000/*GDM8351からの返信が遅いので注意*/, bool setLog = true)
        {
            //送信処理
            try
            {
                ReceiveData = "";

                ClearBuff();//受信バッファのクリア

                port.WriteLine(cmd);// \r\n は自動的に付加されます
                if (setLog)
                {
                    State.VmComm.MbedLog += $"Tx:{cmd}\n";
                }

                //受信処理
                return ReadRecieveData();
            }
            catch
            {
                State.VmComm.MbedLog += "Tx:TX_Error\n";
                return false;
            }
        }

        private static bool ReadRecieveData(int time = 4000, bool setLog = true)
        {

            port.ReadTimeout = time;
            try
            {

                ReceiveData = port.ReadLine();
                if (setLog)
                {
                    State.VmComm.MbedLog += $"Rx:{ ReceiveData}\n";
                }
                return true;
            }
            catch
            {
                State.VmComm.MbedLog += "Rx:RX_Error\n";
                return false;
            }
        }



        public static bool ClosePort()
        {
            try
            {
                //ポートが開いているかどうかの判定
                if (port.IsOpen)
                {
                    SendData("ResetIo");//製品を初期化して終了
                    port.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ClearBuff()
        {
            if (port.IsOpen)
                port.DiscardInBuffer();
        }

    }

}

