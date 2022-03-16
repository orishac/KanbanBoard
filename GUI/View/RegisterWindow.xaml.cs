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
using System.Windows.Shapes;
using GUI.ViewModel;

namespace GUI.View
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private RegisterViewModel RegisterViewModel;
        public RegisterWindow(BackendController controller)
        {
            RegisterViewModel = new RegisterViewModel(controller);
            this.DataContext = RegisterViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// when the user clicks the register button, we try to register the user to the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (RegisterViewModel.Register())
            {
                this.Close();
                
            }
        }

    }
}
