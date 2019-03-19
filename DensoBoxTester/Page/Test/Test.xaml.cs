using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static DensoBoxTester.UiControl;
using static System.Threading.Thread;

namespace DensoBoxTester
{
    /// <summary>
    /// Test.xaml の相互作用ロジック
    /// </summary>
    public partial class Test
    {
        private SolidColorBrush ButtonBrush = new SolidColorBrush();
        private const double ButtonOpacity = 0.3;

        public Test()
        {
            this.InitializeComponent();

            //スタートボタンのデザイン
            ButtonBrush.Color = Colors.DodgerBlue;
            ButtonBrush.Opacity = ButtonOpacity;

            // オブジェクト作成に必要なコードをこの下に挿入します。
            this.DataContext = State.VmTestStatus;

            (FindResource("Blink") as Storyboard).Begin();



            //デートコード表記の設定
            var 年 = System.DateTime.Now.ToString("yy");
            var 月 = (Int32.Parse(System.DateTime.Now.ToString("MM")) * 4).ToString("D2");
            var 日 = System.DateTime.Now.ToString("dd");
            State.VmTestStatus.Dc = 年 + 月 + "Ne" + 日;

        }




        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //エラーインフォメーションページからテストページに遷移する場合は、
            //下記のif文を有効にする
            //フォームの初期化
            SetUnitTest();
            ResetViewModel();

            await State.TestCommand.StartCheck();
        }



        private void SetUnitTest()
        {
            var list = new List<string>();
            var GroupingItems = State.テスト項目.GroupBy(t => t.Key);
            foreach (var g in GroupingItems)
            {
                var addItem = g.First();
                list.Add($"{(addItem.Key * 10).ToString()}_{addItem.Value}");
            }
        }


    }

}


