using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace GUI.Model
{
    public class BoardModel : NotifiableModelObject
    {
        private ObservableCollection<ColumnModel> _columns;

        private string _creatorEmail;

        public ObservableCollection<ColumnModel> Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                RaisePropertyChanged("Columns");

            }
        }

        public BoardModel(BackendController controller, ServiceBoard serviceBoard, string loggedInUser) : base(controller)
        {
            _columns = controller.GetColumnsByNames(serviceBoard.emailCreator, serviceBoard.ColumnsNames, loggedInUser);
            _columns.CollectionChanged += HandleChange;
            _creatorEmail = serviceBoard.emailCreator;

        }

        public BoardModel (ObservableCollection<ColumnModel> columns, BackendController controller) : base(controller)
        {
            _columns = columns;
        }

        /// <summary>
        /// remove column from the board
        /// </summary>
        /// <param name="c">column to remove</param>
        public void RemoveColumn(string email, int index)
        {
            Columns.RemoveAt(index);
            Controller.RemoveColumn(email, index);
            
        }

        public void AddColumn(string email, int columnID, string columnName, string loggedInUser)
        {
            ColumnModel column = Controller.AddColumn(email, columnID, columnName, loggedInUser);
            Columns.Insert(columnID, column);
        }

        public void MoveColumnRight(string email, int columnID)
        {
            Controller.MoveColumnRight(email, columnID);
            Columns.Move(columnID, columnID + 1);
        }

        /// <summary>
        /// if item is added or deleted from the list, we also handle the change in bussiness layer 
        /// </summary>
        /// <param name="sender">sender of the change,</param>
        /// <param name="e"></param>
        private void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ColumnModel c in e.OldItems)
                {

                    Controller.RemoveColumn(_creatorEmail, c.ColumnID);
                }

            }
        }

        public void MoveColumnLeft(string email, int columnID)
        {
            Controller.MoveColumnLeft(email, columnID);
            Columns.Move(columnID, columnID - 1);
        }

        /// <summary>
        /// add new task to the board
        /// </summary>
        /// <param name="email">task assignee</param>
        /// <param name="title">task's title</param>
        /// <param name="description">task's description</param>
        /// <param name="dueDate">task's dueDate</param>
        public void AddTask(string email, string title, string description, DateTime dueDate)
        {
            TaskModel task = Controller.AddTask(email, title, description, dueDate);
            Columns[0].tasks.Add(task);
        }

        /// <summary>
        /// delete task from board
        /// </summary>
        /// <param name="task">task to remove</param>
        public void deleteTask(TaskModel task)
        {
            Columns[task.ColumnID].RemoveTask(task);
        }

        /// <summary>
        /// advance task to the next column
        /// </summary>
        /// <param name="task">task to advance</param>
        public void AdvanceTask(TaskModel task)
        {
            Columns[task.ColumnID].tasks.Remove(task);
            Columns[task.ColumnID + 1].tasks.Add(task);
        }
    }
}
