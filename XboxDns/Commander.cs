using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XboxDns
{
    public class Commander : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        Action<object?> action;

        public Commander(Action<object?> action)
        {
            this.action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action(parameter);
        }
    }
}
