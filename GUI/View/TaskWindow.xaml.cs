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
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        private TaskViewModel taskViewModel;

        public TaskWindow(TaskModel taskModel, BoardModel board)
        {
            InitializeComponent();
            taskViewModel = new TaskViewModel(taskModel, board.Controller, board);
            DataContext = taskViewModel;
        }

        /// <summary>
        /// when the user clicks the advance task button, we try to advance the task to the next column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanceTask_Click(object sender, RoutedEventArgs e)
        {
            if(taskViewModel.AdvanceTask())
                this.Close();
        }

        /// <summary>
        /// when the user clicks the delete task button, we try to delete the task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (taskViewModel.DeleteTask())
            {
                this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (taskViewModel.Save())
                this.Close();
            
        }
    }
}
