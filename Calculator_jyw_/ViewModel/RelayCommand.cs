using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


public class RelayCommand : ICommand
{
    private readonly Action<object> _executeWithParam;
    private readonly Action _executeWithoutParam;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action<object> execute, Func<bool> canExecute = null)
    {
        _executeWithParam = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _executeWithoutParam = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object parameter)
    {
        if (_executeWithParam != null)
        {
            _executeWithParam(parameter);
        }
        else if (_executeWithoutParam != null)
        {
            _executeWithoutParam();
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}
