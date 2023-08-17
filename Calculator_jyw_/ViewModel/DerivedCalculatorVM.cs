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
        
            OnPropertyChanged(nameof(ResultList));
        }
        protected double PerformOperation(double operand1, double operand2, string operation)
        {
            switch (operation)
            {
                case "+": return operand1 + operand2;
                case "-": return operand1 - operand2;
                case "x": return operand1 * operand2;
                case "*": return operand1 * operand2;
                case "/": return operand1 / operand2;
                default: throw new ArgumentException("Invalid operation: " + operation);
            }
        }
    }
}
