using BJITUWPApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace BJITUWPApp.Commands
{
    class DownloadCommand : ICommand
    {
        private DownloadListViewModel _ViewModel;
        private Action<string> _execute;
        public DownloadCommand(Action<string> execute)
        {
            _execute = execute;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter as string);
            //_ViewModel.DownloadLink();
        }
    }
}
