using System;

namespace Calculator_jyw_
{
    public class DerivedCalculatorVM : CalculatorViewModel
    {
        public double operand;

        public double Operand {
            get { return operand; }
            set
            {
                operand = value;
                OnPropertyChanged(nameof(Operand));
            }
        }

        public DerivedCalculatorVM()
        {
            
        }
        public DerivedCalculatorVM(double operand)
        {
            this.operand = operand;
        }


        // + 연산자 재정의
        public static DerivedCalculatorVM operator +(DerivedCalculatorVM operand1, DerivedCalculatorVM operand2 )
        {
            DerivedCalculatorVM result = new DerivedCalculatorVM(operand1.operand + operand2.operand);
            return result;
        }

        // - 연산자 재정의
        public static DerivedCalculatorVM operator -(DerivedCalculatorVM operand1, DerivedCalculatorVM operand2)
        {
            DerivedCalculatorVM result = new DerivedCalculatorVM(operand1.operand - operand2.operand);
            return result;
        }

        

        // 계산 메서드 수정
        private void Calculate()
        {
            string tmp = InputText;
            ResultText = $"{InputText} = ";

            InputText = CalculatePostfix_(ConvertToPostfix(tmp)).ToString();
            OnPropertyChanged(nameof(InputText)); // InputText 업데이트를 위해 OnPropertyChanged 호출
            OnPropertyChanged(nameof(ResultList));
        }


    }
}
