using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using GUI.Model;

namespace GUI.ViewModel
{
    public class BoardViewModel : NotifiableObjects
    {

        public BackendController controller { get; private set; }

        public UserModel user { get; set; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {

            }
        }

        private string _filterKeywords;

        public string FilterKeywords
        {
            get => _filterKeywords;
            set
            {
                _filterKeywords = value;
                RaisePropertyChanged("FilterKeywords");
            }
        }


        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                this._message = value;
                RaisePropertyChanged("Message");
            }
        }



        public BoardViewModel(BackendController controller, UserModel user)
        {
            this.controller = controller;
            this.user = user;
            _title = "Welcome " + user.Nickname;



        }

        /// <summary>
        /// sort the list by task's dueDate
        /// </summary>
        /// <param name="Columns">ListBox of the columns from the window</param>
        public void Sort(ListBox Columns)
        {
            List<ColumnModel> columns = new List<ColumnModel>();
            ObservableCollection<TaskModel> tasks = new ObservableCollection<TaskModel>();
            foreach (ColumnModel c in Columns.Items.Cast<ColumnModel>().ToList())
            {
                foreach (TaskModel t in c.tasks)
                {
                    tasks.Add(t);
                }
                columns.Add(c);
            }


            foreach (ColumnModel c in columns)
            {
                List<TaskModel> tasksToSort = new List<TaskModel>();
                foreach (TaskModel t in c.tasks)
                {
                    tasksToSort.Add(t);
                }
                tasksToSort.Sort((x, y) => x.DueDate.CompareTo(y.DueDate));
                c.tasks.Clear();
                foreach (TaskModel t in tasksToSort)
                {
                    c.tasks.Add(t);
                }
            }

            Columns.ItemsSource = null;
            Columns.Items.Clear();

            foreach (ColumnModel c in columns)
            {
                Columns.Items.Add(c);
            }


        }

        /// <summary>
        /// filter the list of tasks by given string keywords
        /// </summary>
        /// <param name="Columns">ListBox of the columns from the window</param>
        /// <returns>list of the updated window columns</returns>
        public List<ColumnModel> Filter(ListBox Columns)
        {
            List<ColumnModel> columns = new List<ColumnModel>(); // creating copy of the original data
            ObservableCollection<TaskModel> tasks = new ObservableCollection<TaskModel>();
            foreach (ColumnModel c in Columns.Items.Cast<ColumnModel>().ToList())
            {
                columns.Add(c);
                foreach (TaskModel t in c.tasks)
                {
                    if (t.Title.Contains(FilterKeywords) | t.Description.Contains(FilterKeywords))
                    {
                        tasks.Add(t); // filtering by the given keywords
                    }
                }
            }

            Columns.ItemsSource = null;
            Columns.Items.Clear();

            if (!string.IsNullOrEmpty(FilterKeywords))
            {
                ObservableCollection<TaskModel> newTasks = new ObservableCollection<TaskModel>();
                List<ColumnModel> newColumns = new List<ColumnModel>();
                foreach (ColumnModel c in columns)
                {

                    newTasks = FilterList(tasks, c.ColumnID); // takes all the relavent tasks
                    newColumns.Add(new ColumnModel(c.Controller, newTasks, c.ColumnName, c.UserEmail, c.ColumnID));

                }

                return newColumns;
            }
            else
            {
                return user.Board.Columns.Cast<ColumnModel>().ToList();
            }
        }

        /// <summary>
        /// moves column left in board
        /// </summary>
        /// <param name="index">column id</param>
        public void moveLeft(int index)
        {
            {
                Message = "";
                try
                {
                    user.Board.MoveColumnLeft(user.Email, index);
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }

        /// <summary>
        /// deletes column in board
        /// </summary>
        /// <param name="index">column id</param>
        public void deleteColumn(int index)
        {
            Message = "";
            try
            {
                user.Board.RemoveColumn(user.Email, index);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }

        /// <summary>
        /// moves column right in board
        /// </summary>
        /// <param name="index">column id</param>
        public void moveRight(int index)
        {
            {
                Message = "";
                try
                {
                    user.Board.MoveColumnRight(user.Email, index);
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }

        /// <summary>
        /// logout from the system
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        public bool Logout()
        {
            Message = "";
            try
            {
                controller.Logout(user.Email);
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }

        }

        /// <summary>
        /// private method to filter observablecollection
        /// </summary>
        /// <param name="tasks">all of the tasks, need to be filtered</param>
        /// <param name="ColumnID">filter criteria </param>
        /// <returns></returns>
        private ObservableCollection<TaskModel> FilterList(ObservableCollection<TaskModel> tasks, int ColumnID)
        {
            return new ObservableCollection<TaskModel>(tasks.Where(t => t.ColumnID == ColumnID));
        }

    }
}

