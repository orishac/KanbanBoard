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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LimitTasksWindow : Window
    {
        
        private LimitTaskViewModel LimitTaskViewModel;
        private UserModel user;
        public LimitTasksWindow(BackendController controller, UserModel user)
        {
            InitializeComponent();
            LimitTaskViewModel = new LimitTaskViewModel(controller, user);
            DataContext = LimitTaskViewModel;
        }

        /// <summary>
        /// when the user clicks the apply button, we limit the tasks on the requested column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyLimit_Click(object sender, RoutedEventArgs e)
        {
            if (LimitTaskViewModel.LimitTasks())
            {
                this.Close();
            }
        }
    
    }
}
