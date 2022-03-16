using GUI.Model;
using GUI.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System;

namespace GUI.View
{
    /// <summary>
    /// Interaction logic for BoardWindow.xaml
    /// </summary>
    public partial class BoardWindow : Window
    {
        private BoardViewModel BoardViewModel;
        private UserModel user;

        public BoardWindow(BackendController controller, UserModel user)
        {
            InitializeComponent();
            BoardViewModel = new BoardViewModel(controller, user);
            DataContext = BoardViewModel;
            this.user = user;
        }

        /// <summary>
        /// when the user double clicks on a task, we try to open an editing task window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TaskModel task = ((StackPanel) sender).DataContext as TaskModel;
                TaskWindow taskWindow = new TaskWindow(task, user.Board);
                taskWindow.Show();
            }
        }

        /// <summary>
        /// when the user clicks the add task button, we open new window for adding new task 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTask = new AddTaskWindow(BoardViewModel.controller, user);
            addTask.Show();
        }

        /// <summary>
        /// when the user clicks the sort button, we sort the tasks by dueDate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sort_Click(object sender, RoutedEventArgs e)
        {
            BoardViewModel.Sort(Columns);
        }

        /// <summary>
        /// when the user clicks the limitColumnTasks button, we open a new window for this action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimitTasks_Click(object sender, RoutedEventArgs e)
        {
            LimitTasksWindow limitTasks = new LimitTasksWindow(BoardViewModel.controller, user);
            limitTasks.Show();
        }

        /// <summary>
        /// when the user clicks the add column button, we open a new window for this action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addColumn_Click(object sender, RoutedEventArgs e)
        {
            AddColumnWindow addColumn = new AddColumnWindow(BoardViewModel.controller, user);
            addColumn.Show();
        }



        /// <summary>
        /// when the user clicks the filter button, we filter the tasks according his keywords
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            List<ColumnModel> newColumns = BoardViewModel.Filter(Columns);
            foreach (ColumnModel c in newColumns)
            {
                Columns.Items.Add(c);
            }
        }

        /// <summary>
        ///  when the user clicks the logout button, we try to logout from the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (BoardViewModel.Logout())
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        //to move column left you need to select a column first
        private void moveLeft_Click(object sender, RoutedEventArgs e)
        {
            int index = Columns.SelectedIndex;
            BoardViewModel.moveLeft(index);
        }

        //to move column right you need to select a column first
        private void moveRight_Click(object sender, RoutedEventArgs e)
        {
            int index = Columns.SelectedIndex;
            BoardViewModel.moveRight(index);
        }

        //to delete column you need to select a column first
        private void deleteColumn_Click(object sender, RoutedEventArgs e)
        {
            int index = Columns.SelectedIndex;
            BoardViewModel.deleteColumn(index);

        }
    }
}


