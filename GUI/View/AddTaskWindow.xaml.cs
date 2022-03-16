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
using GUI.Model;
using GUI.ViewModel;

namespace GUI.View
{
    /// <summary>
    /// Interaction logic for AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        private AddTaskViewModel AddTaskViewModel;
        private UserModel user;
        public AddTaskWindow(BackendController controller, UserModel user)
        {
            InitializeComponent();
            AddTaskViewModel = new AddTaskViewModel(controller, user);
            DataContext = AddTaskViewModel;
        }
        /// <summary>
        /// when the user clicks the add task button, we try to add it, if it is successful the window closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewTask_Click(object sender, RoutedEventArgs e)
        {
            if (AddTaskViewModel.AddTask())
            {
                this.Close();
            }
        }
    }
}
