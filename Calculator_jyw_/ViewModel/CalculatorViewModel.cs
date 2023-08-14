using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;


namespace Calculator_jyw_
{

    public class CalculatorViewModel : INotifyPropertyChanged
    {

        public const Double Epsilon = 4.94065645841247E-324;


        private string inputText = "";
        private string resultText = "";
        private string Operator;
        private bool doublePointEntered = false;
        private Stack<string> operatorStack = new Stack<string>();
        private List<string> outputList = new List<string>();
        public string InputText
        {
            get { return inputText; }
            set
            {
                inputText = value;
                OnPropertyChanged("inputText");
            }
        }
        public string ResultText
        {
            get { return resultText; }
            set
            {
                resultText = value;
                OnPropertyChanged("resultText");
            }
        }
        public List<string> OutputList
        {
            get { return outputList; }
            set
            {
                outputList = value;
                OnPropertyChanged("outputList");
            }
        }

        public ICommand NumberButtonCommand { get; private set; }
        public ICommand OperatorButtonCommand { get; private set; }
        public ICommand ResultButtonCommand { get; private set; }
        public ICommand ClearButtonCommand { get; private set; }
        public ICommand ShowHistoryCommand { get; private set; }


        public CalculatorViewModel()
        {
            NumberButtonCommand = new RelayCommand(NumberButtonCommandExecute);
            OperatorButtonCommand = new RelayCommand(OperatorButtonCommandExecute);
            ResultButtonCommand = new RelayCommand(ResultButtonCommandExecute);
            ClearButtonCommand = new RelayCommand(ClearButtonCommandExecute);
            ShowHistoryCommand = new RelayCommand(ShowHistoryExecute);
        }


        private string ConvertToPostfix(string infixExpression)
        {
            Dictionary<string, int> precedence = new Dictionary<string, int>
            {
                { "+", 1 }, { "-", 1 },
                { "*", 2 }, { "/", 2 }, { "x", 2 }
            };
            string[] tokens = infixExpression.Split(' ');

            foreach (string token in tokens)
            {
                if (double.TryParse(token, out double _))
                {
                    outputList.Add(token);
                }
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        outputList.Add(operatorStack.Pop());
                    }
                    operatorStack.Pop(); // Pop (
                }
                else
                {
                    while (operatorStack.Count > 0 && precedence.ContainsKey(operatorStack.Peek()) &&
                           precedence[token] <= precedence[operatorStack.Peek()])
                    {
                        outputList.Add(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
               
            }

            while (operatorStack.Count > 0)
            {
                outputList.Add(operatorStack.Pop());
            }

            return string.Join(" ", outputList);
        
    }

        //private double AdjustValue(double value, double epsilon)
        //{
        //    double roundedValue = Math.Round(value, 2); // 예시로 소수점 아래 2자리에서 반올림
        //    double ceilingValue = Math.Ceiling(value * 100) / 100; // 소수점 아래 2자리에서 올림
        //    double floorValue = Math.Floor(value * 100) / 100; // 소수점 아래 2자리에서 내림

        //    double diffFromRounded = Math.Abs(value - roundedValue);
        //    double diffFromCeiling = Math.Abs(value - ceilingValue);
        //    double diffFromFloor = Math.Abs(value - floorValue);

        //    double minDiff = Math.Min(diffFromRounded, Math.Min(diffFromCeiling, diffFromFloor)); //이렇게 작은 수는 동일하게 취급하는데 어떻게 이걸로 부동 소수점 오차를 보정하나요?

        //    if (minDiff < epsilon)
        //    {
        //        if (minDiff == diffFromRounded)
        //            return roundedValue;
        //        else if (minDiff == diffFromCeiling)
        //            return ceilingValue;
        //        else
        //            return floorValue;
        //    }
        //    else
        //    {
        //        return value; // 그대로 반환
        //    }
        //}
        private double CalculatePostfix(string postfixExpression)
        {
            Stack<double> stack = new Stack<double>();

            string[] tokens = postfixExpression.Split(' ');

            foreach (string token in tokens)
            {
                if (double.TryParse(token, out double number))
                {
                    stack.Push(number);
                }
                else
                {
                    double operand2 = stack.Pop();
                    double operand1 = stack.Pop();
                    double result = PerformOperation(operand1, operand2, token);

                    //AdjustValue(result, Epsilon);

                    stack.Push(result);
                }
            }

            return stack.Pop();
        }

  
        private double PerformOperation(double operand1, double operand2, string operation)
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
        /*
        * @brief 숫자 버튼을 누르면 InputTextBox에 해당 숫자가 입력됩니다.  
        * @param parameter: 누른 숫자의 값
        * @return 반환값 없음
        * @note Patch-notes
        * 2023-08-10|조예원|설명작성
        * @warning 없음
        */
        private void NumberButtonCommandExecute(object parameter)
        {

            if (parameter is string number)
            {
                if (number == ".")
                {
                    if (!doublePointEntered)
                    {
                        InputText = $"{inputText}{number}";
                        doublePointEntered = true;
                    }
                }
                else
                {
                    InputText = $"{inputText}{number}";
                }
            }
            
        }

        /*
        * @brief 연산자 버튼을 누르면 resulttextbox에 피연산자와 연산자가 입력됩니다.  
        * @param parameter: 누른 연산자의 값
        * @return 반환값 없음
        * @note patch-notes
        * 2023-08-10|조예원|설명작성
        * @warning 없음
        */

        public void OperatorButtonCommandExecute(object parameter)
        {
            doublePointEntered = false;

            if (!string.IsNullOrEmpty(Operator)) // 이전에 연산자가 입력되었을 경우
            {
                // 이전 연산자를 지우고 새로운 연산자 입력
                inputText = inputText.Remove(inputText.Length - 2);
            
            }


            Operator = parameter.ToString();
            inputText=$"{inputText}{" "}{parameter}{" "}";
            InputText = inputText;

        }
        /*
        * @brief = 버튼을 누르면 resulttextbox에 식이 입력되고, inputtextbox에 결과값이 입력됩니다.  
        * @param parameter: 사용되지 않음
        * @return 반환값 없음
        * @note patch-notes
        * 2023-08-10|조예원|설명작성
        * @warning 없음
        */

        /*
        * @brief c 버튼을 누르면 inputtextbox와 resulttextbox가 비워집니다.  
        * @param parameter: 사용되지 않음
        * @return 반환값 없음
        * @note patch-notes
        * 2023-08-10|조예원|설명작성
        * @warning 없음
        */
        private void ClearButtonCommandExecute(object parameter)
        {
            InputText = "";
            ResultText = "";
            Operator = null;
            doublePointEntered = false;
         
        }

        private void ResultButtonCommandExecute(object parameter)
        {   string tmp = InputText;
            ResultText = $"{InputText}{ " = " }";
            InputText=CalculatePostfix(ConvertToPostfix(tmp)).ToString();

        }
        /*
        * @brief 바뀐 프로퍼티가 있으면 그 변화를 반영합니다.  
        * @param propertyname: 값이 바뀐 프로퍼티의 이름
        * @return 반환값 없음
        * @note patch-notes
        * 2023-08-10|조예원|설명작성
        * @warning 없음
        */
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShowHistoryExecute(object parameter)
        {
            History history = new History();
            history.ShowDialog();
        }


      

        #region [중첩된 클래스]
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


    }

}



