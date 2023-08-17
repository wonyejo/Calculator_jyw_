using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;


namespace Calculator_jyw_
{

    public class CalculationEntry : INotifyPropertyChanged
    {
        private string expression;
        private string result;

        public string Expression
        {
            get { return expression; }
            set
            {
                expression = value;
                OnPropertyChanged(nameof(Expression));
            }
        }

        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged(nameof(Result));
            }
        }
        private CalculationEntry selectedEntry;

        public CalculationEntry SelectedEntry
        {
            get { return selectedEntry; }
            set
            {
                selectedEntry = value;
                OnPropertyChanged(nameof(SelectedEntry));
            }
        }
        // INotifyPropertyChanged 이벤트 정의
        public event PropertyChangedEventHandler PropertyChanged;

        // PropertyChanged 이벤트 호출 메서드
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class CalculatorViewModel : INotifyPropertyChanged
    {

        private string inputText = "";
        private string resultText = "";
        private string Operator;
        private bool doublePointEntered = false;
        private Stack<string> operatorStack = new Stack<string>();
        private List<string> outputList = new List<string>();
        private ObservableCollection<CalculationEntry> resultList = new ObservableCollection<CalculationEntry>();
        private CalculationEntry selectedResult;

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
        public ObservableCollection<CalculationEntry> ResultList
        {
            get { return resultList; }
            set
            {
                resultList = value;
                OnPropertyChanged(nameof(ResultList));
            }
        }

      
        public CalculationEntry SelectedResult
        {
            get { return selectedResult; }
            set
            {
                selectedResult = value;
                OnPropertyChanged(nameof(SelectedResult));
                // SelectedResult가 변경될 때 InputText와 ResultText 업데이트
                if (selectedResult != null)
                {
                    ResultText = selectedResult.Expression;
                    InputText = selectedResult.Result;
                }
            }
        }
        public ICommand NumberButtonCommand { get; private set; }
        public ICommand OperatorButtonCommand { get; private set; }
        public ICommand ResultButtonCommand { get; private set; }
        public ICommand ClearButtonCommand { get; private set; }
        public ICommand ShowHistoryCommand { get; private set; }
        public ICommand ShowSelectedResultCommand { get; private set; }
      
        public CalculatorViewModel()
        {
            NumberButtonCommand = new RelayCommand(NumberButtonCommandExecute);
            OperatorButtonCommand = new RelayCommand(OperatorButtonCommandExecute);
            ResultButtonCommand = new RelayCommand(ResultButtonCommandExecute);
            ClearButtonCommand = new RelayCommand(ClearButtonCommandExecute);
            ShowSelectedResultCommand = new RelayCommand(ShowSelectedResultCommandExecute);
         
        }

        
        protected string ConvertToPostfix(string infixExpression)
        {
            Dictionary<string, int> precedence = new Dictionary<string, int>
            {
                { "+", 1 }, { "-", 1 },
                { "*", 2 }, { "/", 2 }, { "x", 2 }
            };
            string[] tokens = infixExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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

        protected double CalculatePostfix(string postfixExpression)
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


                    stack.Push(result);
                }
            }

            return stack.Pop();
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
        /*
        * @brief 숫자 버튼을 누르면 InputTextBox에 해당 숫자가 입력됩니다.  
        * @param parameter: 누른 숫자의 값
        * @return 반환값 없음
        * @note Patch-notes
        * 2023-08-10|조예원|설명작성
        * @warning 없음
        */
        protected void NumberButtonCommandExecute(object parameter)
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

        protected void OperatorButtonCommandExecute(object parameter)
        {
            doublePointEntered = false;

            if (inputText.EndsWith(" ") || inputText.EndsWith("+") || inputText.EndsWith("-") || inputText.EndsWith("*") || inputText.EndsWith("/"))
            {
                inputText = inputText.Remove(inputText.Length - 2);
            }

            Operator = parameter.ToString();
            inputText = $"{inputText.Trim()} {parameter} "; // 불필요한 공백 제거 후 연산자 추가

            InputText = inputText;

        }

        /*
        * @brief c 버튼을 누르면 inputtextbox와 resulttextbox가 비워집니다.  
        * @param parameter: 사용되지 않음
        * @return 반환값 없음
        * @note patch-notes
        * 2023-08-10|조예원|설명작성
        * @warning 없음
        */

        protected void ShowSelectedResultCommandExecute(object parameter)
        {
            if (SelectedResult != null)
            {
                InputText = SelectedResult.Result;
                ResultText = SelectedResult.Expression;
                SelectedResult = null;
            }
        }

        protected void ClearButtonCommandExecute(object parameter)
        {
            InputText = "";
            ResultText = "";
            Operator = null;
            doublePointEntered = false;


        }
        /*
       * @brief = 버튼을 누르면 resulttextbox에 식이 입력되고, inputtextbox에 결과값이 입력됩니다.
       * @param parameter: 사용되지 않음
       * @return 반환값 없음
       * @note patch-notes
       * 2023-08-10|조예원|설명작성
       * @warning 없음
       */

        protected void ResultButtonCommandExecute(object parameter)
        {

            ResultText = $"{InputText}{ " = " }";
            string expression = InputText;
            InputText = CalculatePostfix(ConvertToPostfix(expression)).ToString();
            string result = inputText;
            CalculationEntry entry = new CalculationEntry
            {
                Expression = expression,
                Result = result
            };

            ResultList.Add(entry);
            OnPropertyChanged(nameof(ResultList));

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



        #region [중첩된 클래스]
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


    }

}

//icommand 관련된 건 다 viewmodel
//뷰모델에서 연산하는 메소드 호출
//숫자 들어왔을 때 변수 저장하는 곳은 뷰모델..

