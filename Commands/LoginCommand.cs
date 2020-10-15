using BJITUWPApp.ViewModels;
using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace BJITUWPApp.Commands
{
    internal class LoginCommand : ICommand
    {
        private LoginViewModel _ViewModel;
        public LoginCommand(LoginViewModel viewModel)
        {
            _ViewModel = viewModel;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            _ViewModel.Login(password);
        }
    }
}