using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUI.Model;
using GUI.View;
using GUI.ViewModel;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace GUI.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private LoginViewModel LoginViewModel;

        public LoginWindow()
        {
            LoginViewModel = new LoginViewModel();
            this.DataContext = LoginViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// when the user clicks the register button, we open a new window for this action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(LoginViewModel.Controller);
            registerWindow.Show();

        }

        /// <summary>
        /// when the user clicks the login button, we try log the user in to the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserModel u = LoginViewModel.Login();
            if (u != null)
            {
                BoardWindow boardWindow = new BoardWindow(LoginViewModel.Controller,u);
                boardWindow.Show();
                this.Close();
            }
        }
    }
}