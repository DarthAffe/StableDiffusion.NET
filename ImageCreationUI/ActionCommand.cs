using System.Windows.Input;

namespace ImageCreationUI;

public class ActionCommand(Action command, Func<bool>? canExecute = null) : ICommand
{
    #region Events

    public event EventHandler? CanExecuteChanged;

    #endregion

    #region Methods

    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => command.Invoke();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    #endregion
}

public class ActionCommand<T>(Action<T> command, Func<T, bool>? canExecute = null) : ICommand
    where T : class
{
    #region Events

    public event EventHandler? CanExecuteChanged;

    #endregion

    #region Methods

    public bool CanExecute(object? parameter) => canExecute?.Invoke((T)parameter!) ?? true;

    public void Execute(object? parameter) => command.Invoke((T)parameter!);

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    #endregion
}