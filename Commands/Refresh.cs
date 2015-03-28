using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WifiCatcher.Commands
{
    internal class RefreshCommand : ICommand
    {
        private readonly ViewModel _vm;

        public RefreshCommand(ViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _vm.RefreshNetworks();
        }

        public event EventHandler CanExecuteChanged;
    }
}
