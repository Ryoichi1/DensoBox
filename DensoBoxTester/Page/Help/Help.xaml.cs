using System.Windows;

namespace DensoBoxTester
{
    /// <summary>
    /// VerInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class VerInfo
    {
        public VerInfo()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbAssemblyVer.Text = "アセンブリVer " + State.AssemblyInfo;
            tbParameterVer.Text = "";//RCC300A100はTestSpecファイル無し
            //tbParameterVer.Text = "パラメータファイルVer " + State.TestSpec.TestSpecVer;//TODO: configファイルにVer（string）を入れること
        }
    }
}
