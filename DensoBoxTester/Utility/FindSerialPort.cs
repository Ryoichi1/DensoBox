using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace DensoBoxTester
{
    public class ComPortStatus
    {
        public string comNo { get; set; }
        public string comName { get; set; }
        public bool PortOpen { get; set; }
    }

    public static class FindSerialPort
    {
        public static List<ComPortStatus> deviceNameList;

        public static void GetDeviceNames()
        {
            deviceNameList = new List<ComPortStatus>();
            var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");


            ManagementClass mcPnPEntity = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection manageObjCol = mcPnPEntity.GetInstances();

            //全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
            foreach (ManagementObject manageObj in manageObjCol)
            {
                //Nameプロパティを取得
                var namePropertyValue = manageObj.GetPropertyValue("Name");
                if (namePropertyValue == null)
                {
                    continue;
                }

                //Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
                string name = namePropertyValue.ToString();
                if (check.IsMatch(name))
                {
                    int i = name.LastIndexOf("(");
                    int j = name.LastIndexOf(")");

                    var Name = name.Substring(0, i);
                    var Number = name.Substring(i + 1, j - i - 1);
                    deviceNameList.Add(new ComPortStatus() { comName = Name, comNo = Number, PortOpen = false });
                }
            }

        }

        //ポート名が一致して、かつ使用されていないポート番号のリストを返す
        public static string[] GetComNo(string ID_NAME)
        {
            //Comポートの取得
            return deviceNameList.Where(a => a.comName.Contains(ID_NAME) && !a.PortOpen).Select(a => a.comNo).ToArray();
            /*ID_NAMEが微妙に違う場合があるため（末尾にスペースが入っているなど）、Containsメソッドで判定する*/
        }

        public static void SetComOpenStatus(string comNumber, bool sw)
        {
            int index = 0;
            foreach (var c in deviceNameList.Select((v, i) => new { v, i }))
            {
                if (c.v.comNo == comNumber) index = c.i;
            }

            deviceNameList[index].PortOpen = sw;

        }


    }
}
