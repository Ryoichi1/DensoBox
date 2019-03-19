using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DensoBoxTester
{
    public delegate Task<bool> TestProc();

    public class TestDetail
    {
        public int StepNo;//
        public int Key;//同じカテゴリの試験項目に、同一Keyを設定する Keyは 0 からスタート
        public string Value;//検査項目名
        public TestProc testProc;//テストメソッドへのデリゲート

        public TestDetail(int key, string value, TestProc proc)
        {
            this.Key = key;
            this.Value = value;
            this.testProc = proc;
        }
    }

    public static class State
    {

        //データソース（バインディング用）
        public static ViewModelMainWindow VmMainWindow = new ViewModelMainWindow();
        public static ViewModelTestStatus VmTestStatus = new ViewModelTestStatus();
        public static Main TestCommand = new Main();
        public static ViewModelCommunication VmComm = new ViewModelCommunication();

        //パブリックメンバ
        public static Configuration setting { get; set; }
        public static TestSpec spec { get; set; }


        public static string MachineName { get; set; }

        public static double CurrentThemeOpacity { get; set; }

        public static string CurrDir { get; set; }

        public static string AssemblyInfo { get; set; }

        public static Uri uriOtherInfoPage { get; set; }

        public static double VolAc24v_1 { get; set; }
        public static double VolAc24v_2 { get; set; }
        public static double VolAc24vOff_1 { get; set; }
        public static double VolAc24vOff_2 { get; set; }

        //リトライ履歴保存用リスト
        public static List<string> RetryLogList = new List<string>();

        public static List<TestDetail> テスト項目;

        static State()
        {
            SetTestProp();
        }


        /// <summary>
        /// 試験項目の設定
        /// </summary>
        static void SetTestProp()
        {
            テスト項目 = new List<TestDetail>()
            {

                new TestDetail(1, "LD1点灯確認", TestItems.SequenceCheck.CheckLd1On),
                new TestDetail(2, "LD2点灯確認", TestItems.SequenceCheck.CheckLd2On),
                new TestDetail(3, "CN4 3-4導通確認", TestItems.SequenceCheck.CheckCn4_3_4_Short),
                new TestDetail(4, "LD2消灯確認", TestItems.SequenceCheck.CheckLd2Off),
                new TestDetail(5, "CN4 3-4非導通確認", TestItems.SequenceCheck.CheckCn4_3_4_Open),
                new TestDetail(6, "LD3点灯確認", TestItems.SequenceCheck.CheckLd3On),
                new TestDetail(7, "LD4点灯/ブザーON確認", TestItems.SequenceCheck.CheckLd4OnBzOn),
                new TestDetail(8, "ブザーOFF確認", TestItems.SequenceCheck.CheckBzOff),
                new TestDetail(9, "CN2 1-3 AC24V出力確認", TestItems.SequenceCheck.CheckCn2_1_3_Ac24vOn),
                new TestDetail(10, "CN2 2-4 AC24V出力確認", TestItems.SequenceCheck.CheckCn2_2_4_Ac24vOn),
                new TestDetail(11, "CN2 1-3 AC24V無電圧確認", TestItems.SequenceCheck.CheckCn2_1_3_Ac24vOff),
                new TestDetail(12, "CN2 2-4 AC24V無電圧確認", TestItems.SequenceCheck.CheckCn2_2_4_Ac24vOff),

            };

            int i = 0;
            var oldKey = 0;

            テスト項目.ForEach(t =>
            {
                if (oldKey != t.Key)
                {
                    oldKey = t.Key;
                    i = 0;//初期化
                }

                t.StepNo = t.Key * 10 + i++;

            });
        }


        //個別設定のロード
        public static void LoadConfigData()
        {
            //各種設定ファイルのロード
            setting = Deserialize<Configuration>(Constants.filePath_Configuration);
            spec = Deserialize<TestSpec>(Constants.filePath_TestSpec);


            //コンフィグ設定
            VmMainWindow.ListOperator = setting.作業者リスト;
            VmMainWindow.Theme = setting.PathTheme;
            VmMainWindow.ThemeOpacity = setting.OpacityTheme;

            if (setting.日付 != DateTime.Now.ToString("yyyyMMdd"))
            {
                setting.日付 = DateTime.Now.ToString("yyyyMMdd");
                setting.TodayOkCount = 0;
                setting.TodayNgCount = 0;
            }

            VmTestStatus.OkCount = setting.TodayOkCount.ToString() + "台";
            VmTestStatus.NgCount = setting.TodayNgCount.ToString() + "台";

            State.VmMainWindow.Opecode = setting.Opecode;

            State.VmTestStatus.VoiceSpec = setting.音声認識一致率;
        }

        public static void LoadTestSpec()//リトライ前にコールすると便利（デバッグ時、試験スペックを調整するときに使用する）
        {
            //TestSpecファイルのロード
            spec = Deserialize<TestSpec>(Constants.filePath_TestSpec);
        }

        //インスタンスをXMLデータに変換する
        public static bool Serialization<T>(T obj, string xmlFilePath)
        {
            try
            {
                //XmlSerializerオブジェクトを作成
                //オブジェクトの型を指定する
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(T));
                //書き込むファイルを開く（UTF-8 BOM無し）
                System.IO.StreamWriter sw = new System.IO.StreamWriter(xmlFilePath, false, new System.Text.UTF8Encoding(false));
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(sw, obj);
                //ファイルを閉じる
                sw.Close();

                return true;

            }
            catch
            {
                return false;
            }

        }

        //XMLデータからインスタンスを生成する
        public static T Deserialize<T>(string xmlFilePath)
        {
            System.Xml.Serialization.XmlSerializer serializer;
            using (var sr = new System.IO.StreamReader(xmlFilePath, new System.Text.UTF8Encoding(false)))
            {
                serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }

        //********************************************************
        //個別設定データの保存
        //********************************************************
        public static bool Save個別データ()
        {
            try
            {
                //Configファイルの保存
                setting.作業者リスト = VmMainWindow.ListOperator;
                setting.PathTheme = VmMainWindow.Theme;
                setting.OpacityTheme = VmMainWindow.ThemeOpacity;
                setting.Opecode = VmMainWindow.Opecode == "" ? setting.Opecode : VmMainWindow.Opecode;
                setting.音声認識一致率 = State.VmTestStatus.VoiceSpec;
                Serialization<Configuration>(setting, Constants.filePath_Configuration);

                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}
