using System.Windows.Input;
using System;

namespace KGuiV2.Helpers
{
    internal class RelayCommand : ICommand
    {
        /// <summary>
        /// The predicate that gets executed when <see cref="CanExecute(object)"/> is called.
        /// </summary>
        readonly Predicate<object> _canExecute;

        /// <summary>
        /// The action that gets executed when <see cref="Execute(object)"/> is called.
        /// </summary>
        readonly Action<object> _execute;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
            => _canExecute(parameter);

        /// <inheritdoc/>
        public void Execute(object? parameter)
            => _execute(parameter);

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="RelayCommand"/> using the supplied values.
        /// </summary>
        /// <param name="canExecute">The predicate used for <see cref="CanExecute(object)"/>.</param>
        /// <param name="execute">The action used for <see cref="Execute(object)"/>.</param>
        public RelayCommand(Predicate<object> canExecute, Action<object> execute)
            => (_canExecute, _execute) = (canExecute, execute);
    }
}
