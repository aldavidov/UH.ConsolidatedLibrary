using System;
using System.Windows.Input;

namespace UH.UserProfileTools
{
    public class RelayCommand : ICommand
    {
        #region private members
        private Action<object> _Execute;
        readonly Predicate<object> _CanExecute;
        #endregion

        #region public events

        public bool CanExecute(object parameter)
        {
            return _CanExecute == null || _CanExecute(parameter);
        }
        public void RaiseCanExecuteChanged() { CanExecuteChanged(this, EventArgs.Empty); }

        public event EventHandler CanExecuteChanged = (sender, e) => { };
        public void Execute(object parameter) { _Execute(parameter); }

        #endregion

        #region Constructors

        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _Execute = execute;
            _CanExecute = canExecute;
        }

        #endregion

    }
}
