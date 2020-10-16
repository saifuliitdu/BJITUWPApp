namespace BJITUWPApp.ViewModels
{
    using BJITUWPApp.Commands;
    using BJITUWPApp.Models;
    using BJITUWPApp.Services;
    using BJITUWPApp.Views;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;
    using Windows.UI.WindowManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Hosting;

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
        public async Task LoginAsync(string password)
        {
            User.Password = password;
            var isUser = _userService.IsAutheticateUSer(User);
            //AppWindow appWindow = await AppWindow.TryCreateAsync();
            //Frame appWindowContentFrame = new Frame();
            //appWindowContentFrame.Navigate(typeof(BlankPage1));
            //ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);
            //await appWindow.TryShowAsync();
            await DownloadWindow();
            if (isUser)
            {
                Message = "Successfull login";

            }
            else
            {
                Message = "Invalid login";
            }
        }

        private static async Task DownloadWindow()
        {
            DownloadListView view = new DownloadListView();
            int newViewId = 0;
            await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(DownLoadView), null);
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
        }
    }
}
