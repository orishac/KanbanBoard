using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace GUI.Model
{
    public class ColumnModel : NotifiableModelObject
    {
        private int _columnID;
        public int ColumnID
        {
            get => _columnID;
            set
            {
                _columnID = value;
                
            }
        }

        private string _columnName;
        public string ColumnName
        {
            get => _columnName;
            set
            {
                _columnName = value;
                Controller.ChangeColumnName(UserEmail, _columnID, _columnName);
                RaisePropertyChanged("ColumnName");
            }
        }

        private string _userEmail;
        public string UserEmail
        {
            get => _userEmail;
            set
            {
 
            }
        }

        public ObservableCollection<TaskModel> tasks { get; private set; }



        public ColumnModel (BackendController controller , ObservableCollection<TaskModel> tasks, string name, string userEmail, int columnId): base (controller)
        {
            this.tasks = tasks;
            this.tasks.CollectionChanged += HandleChange;
            _columnName = name;
            _userEmail = userEmail;
            _columnID = columnId;
        }

        public ColumnModel(BackendController controller, ServiceColumn serviceColumn, string userEmail,string loggedInUser) : base(controller)
        {
            tasks = new ObservableCollection<TaskModel>();
            foreach(ServiceTask t in serviceColumn.Tasks)
            {
                tasks.Add(new TaskModel(controller,t, serviceColumn.ColumnID, loggedInUser));
            }
            this.tasks.CollectionChanged += HandleChange;
            _columnName = serviceColumn.Name;
            UserEmail = userEmail;
            _columnID = serviceColumn.ColumnID;

        }
        /// <summary>
        /// remove task from the board
        /// </summary>
        /// <param name="t">task to remove</param>
        public void RemoveTask(TaskModel t)
        {
            Controller.DeleteTask(t.EmailAsignee, t.ColumnID, t.ID);
            tasks.Remove(t);

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
                foreach (TaskModel y in e.OldItems)
                {
                   //Controller.DeleteTask(UserEmail, ColumnID, y.ID);
                }

            }

        }

    }
}
