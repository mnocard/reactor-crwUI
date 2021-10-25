using System;
using System.Windows.Input;

namespace reactor_crwUI.Core
{
    internal abstract class CommandCore : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract bool CanExecute(object p);

        public abstract void Execute(object p);
    }
}
