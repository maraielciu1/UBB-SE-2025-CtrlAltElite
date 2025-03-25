using System;
using System.Threading.Tasks;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;
    private readonly Action<object> _executeWithParameter;
    private readonly Func<object, bool> _canExecuteWithParameter;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _executeWithParameter = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecuteWithParameter = canExecute;
    }

    public RelayCommand(Func<Task> execute, Func<bool> canExecute = null!)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        if (_canExecuteWithParameter != null)
        {
            return _canExecuteWithParameter(parameter);
        }
        return _canExecute == null || _canExecute();
    }

    public async void Execute(object? parameter)
    {
        if (_executeWithParameter != null)
        {
            _executeWithParameter(parameter);
        }
        else if (_execute != null)
        {
            await _execute();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}