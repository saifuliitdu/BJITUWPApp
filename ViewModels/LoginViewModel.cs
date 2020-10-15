namespace BJITUWPApp.ViewModels
{
    using BJITUWPApp.Commands;
    using BJITUWPApp.Models;
    using BJITUWPApp.Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    class LoginViewModel : INotifyPropertyChanged
    {
        UserService _userService;

        #region INotifyPropertyChanged_Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public LoginViewModel()
        {
            _userService = new UserService();
            LoginCommand = new LoginCommand(this);
            _user = new User();
        }

        private User _user;
        public User User
        {
            get
            {
                return _user;
            }

        }
        public ICommand LoginCommand
        {
            get;
            private set;
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged("Message"); }
        }
        public void Login(string password)
        {
            User.Password = password;
            var isUser = _userService.IsAutheticateUSer(User);
            if (isUser)
            {
                Message = "Successfull login";
            }
            else
            {
                Message = "Invalid login";
            }
        }
    }
}
