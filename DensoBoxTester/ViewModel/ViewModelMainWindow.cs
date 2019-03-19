using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;



namespace DensoBoxTester
{

    public class ViewModelMainWindow : BindableBase
    {
        public ViewModelMainWindow()
        {
            SelectIndexOperatorCb = -1;
        }


        //試験中は作業者名を変更できないようにする
        private bool _OperatorEnable = true;
        public bool OperatorEnable
        {

            get { return _OperatorEnable; }
            set { SetProperty(ref _OperatorEnable, value); }
        }


        //プロパティ
        private List<string> _ListOperator;
        public List<string> ListOperator
        {

            get { return _ListOperator; }
            set { SetProperty(ref _ListOperator, value); }

        }


        private string _Theme;
        public string Theme
        {
            get { return _Theme; }
            set { SetProperty(ref _Theme, value); }
        }


        private double _ThemeBlurEffectRadius;
        public double ThemeBlurEffectRadius
        {
            get { return _ThemeBlurEffectRadius; }
            set { SetProperty(ref _ThemeBlurEffectRadius, value); }
        }

        private double _ThemeOpacity;
        public double ThemeOpacity
        {
            get { return _ThemeOpacity; }
            set { SetProperty(ref _ThemeOpacity, value); }
        }

        private int _SelectIndexOperatorCb;
        public int SelectIndexOperatorCb
        {

            get { return _SelectIndexOperatorCb; }
            set { SetProperty(ref _SelectIndexOperatorCb, value); }

        }

        private string _Operator;
        public string Operator
        {
            get { return _Operator; }
            set { SetProperty(ref _Operator, value); }
        }



        private string _Opecode;
        public string Opecode
        {
            get { return _Opecode; }
            set { SetProperty(ref _Opecode, value); }
        }

        private bool _ReadOnlyOpecode;
        public bool ReadOnlyOpecode
        {
            get { return _ReadOnlyOpecode; }
            set { SetProperty(ref _ReadOnlyOpecode, value); }
        }


        private bool _EnableOtherButton;
        public bool EnableOtherButton //MainWindowのタブコントロールの各TabItemのイネーブルにバインドする
        {
            get { return _EnableOtherButton; }
            set { SetProperty(ref _EnableOtherButton, value); }
        }


        private int _TabIndex;
        public int TabIndex
        {

            get { return _TabIndex; }
            set { SetProperty(ref _TabIndex, value); }

        }

    }
}
