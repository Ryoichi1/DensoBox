
namespace DensoBoxTester
{
    public static class Constants
    {
        //作業者へのメッセージ
        public const string MessOperator = "作業者名を選択してください";
        public const string MessOpecode = "工番を入力してください";
        public const string MessPushStart = "製品をセットして開始ボタン（青）を押して下さい";
        public const string MessRemove = "製品を取り外してください";
        public const string MessWait = "しばらくお待ちください・・・";
        public const string MessCheckConnectMachine = "周辺機器の接続を確認してください！";

        //Conf File
        public static readonly string RootPath = State.MachineName == "TSPCDP00059" ? @"D:\試験機用設定ファイル\電装Box" : @"C:\電装Box";
        public static readonly string filePath_TestSpec = $@"{RootPath}\ConfigData\TestSpec.config";
        public static readonly string filePath_Configuration = $@"{RootPath}\ConfigData\Configuration.config";


        //検査データフォルダのパス
        public static readonly string PassDataPath = $@"{RootPath}\検査データ\合格品データ\";
        public static readonly string FailDataPath = $@"{RootPath}\検査データ\不合格品データ\";
        public static readonly string fileName_RetryLog = $@"{RootPath}\検査データ\リトライ履歴.txt";
        public static readonly string fileName_FormatPass = $@"{RootPath}\検査データ\FormatPass.csv";
        public static readonly string fileName_FormatFail = $@"{RootPath}\検査データ\FormatFail.csv";


        public static readonly double PanelOpacity = 0.8;

    }
}
