using GUI.Model;
using GUI.ViewModel;
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

namespace GUI.View
{
    /// <summary>
    /// Interaction logic for AddColumnWindow.xaml
    /// </summary>
    public partial class AddColumnWindow : Window
    {
        private AddColumnViewModel AddColumnViewModel;
        private UserModel user;
        public AddColumnWindow(BackendController controller, UserModel user)
        {
            InitializeComponent();
            AddColumnViewModel = new AddColumnViewModel(controller, user);
            DataContext = AddColumnViewModel;
        }
        /// <summary>
        /// when the user clicks the add column button, we try to add it, if it is successful the window closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (AddColumnViewModel.AddColumn())
            {
                this.Close();
            }
        }
    }
}
