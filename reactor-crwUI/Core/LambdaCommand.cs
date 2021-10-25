using System;

namespace reactor_crwUI.Core
{
    internal class LambdaCommand : CommandCore
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public LambdaCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object p) => canExecute?.Invoke(p) ?? true;

        public override void Execute(object p) => execute(p);
    }
}
