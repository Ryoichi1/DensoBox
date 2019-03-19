using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;



namespace DensoBoxTester
{

    public class ViewModelEdit : BindableBase
    {
        public ViewModelEdit()
        {
            ListOperator = new List<string>(State.VmMainWindow.ListOperator);
            SelectIndex = -1;//デフォルトは未選択とする
        }

        //プロパティ

        private List<string> _ListOperator;
        public List<string> ListOperator
        {

            get { return _ListOperator; }
            set { SetProperty(ref _ListOperator, value); }

        }

        private int _SelectIndex;
        public int SelectIndex
        {

            get { return _SelectIndex; }
            set { SetProperty(ref _SelectIndex, value); }

        }


        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

    }
}
