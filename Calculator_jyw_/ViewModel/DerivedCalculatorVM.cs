using System;

namespace Calculator_jyw_
{
    public class DerivedCalculatorVM : CalculatorViewModel
    {
        #region 필드
        public double operand;
        #endregion

        #region 속성
         public double Operand {
                    get { return operand; }
                    set
                    {
                        operand = value;
                        OnPropertyChanged(nameof(Operand));
                    }
                }
        #endregion

        #region 생성자
        public DerivedCalculatorVM()
        {

        }
        public DerivedCalculatorVM(double operand)
        {
            this.operand = operand;
        }
        #endregion

        #region 메서드
        // + 연산자 재정의
        public static DerivedCalculatorVM operator +(DerivedCalculatorVM operand1, DerivedCalculatorVM operand2)
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
        #endregion



    }
}
