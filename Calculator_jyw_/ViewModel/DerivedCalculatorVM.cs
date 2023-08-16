using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator_jyw_.ViewModel
{
    class DerivedCalculatorVM: CalculatorViewModel
    {
        // + 연산자 재정의
        public static DerivedCalculatorVM operator +(DerivedCalculatorVM calculator, double value)
        {
            calculator.InputText = $"{calculator.InputText} + {value}";
            calculator.Calculate();
            return calculator;
        }

        // - 연산자 재정의
        public static DerivedCalculatorVM operator -(DerivedCalculatorVM calculator, double value)
        {
            calculator.InputText = $"{calculator.InputText} - {value}";
            calculator.Calculate();
            return calculator;
        }

        // 계산 메서드
        private void Calculate()
        {
            string tmp = InputText;
            ResultText = $"{InputText} = ";
            InputText = CalculatePostfix(ConvertToPostfix(tmp)).ToString();
            ResultList.Add(ResultText + InputText);
            OnPropertyChanged(nameof(ResultList));
        }
    }
}
