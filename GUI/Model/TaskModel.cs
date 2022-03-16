using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace GUI.Model
{
    public class TaskModel : NotifiableModelObject
    {
        private int _id;
        public int ID
        {
            get => _id;
            set { }
        }
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                Controller.UpdateTaskTitle(loggedInUser, value, ID, ColumnID);
                _title = value;
                RaisePropertyChanged("Title");
            }
        }
        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                Controller.UpdateTaskDescription(loggedInUser, value, ID, ColumnID);
                _description = value;
                RaisePropertyChanged("Description");
            }
        }
        private DateTime _creationTime;
        public DateTime CreationTime
        {
            get => _creationTime;
            set
            {

            }
        }
        private DateTime _dueDate;
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                Controller.UpdateTaskDueDate(loggedInUser, value, ID, ColumnID);
                _dueDate = value;
                RaisePropertyChanged("DueDate");
            }
        }
        private string _emailAsignee;
        public string EmailAsignee
        {
            get => _emailAsignee;
            set
            {
                Controller.AssignTask(loggedInUser, ColumnID, ID, value);
                _emailAsignee = value;
                RaisePropertyChanged("EmailAsignee");
            }
        }
        private int _columnID;
        public int ColumnID
        {
            get => _columnID;
            set
            {
                _columnID = value;
            }
        }

        public SolidColorBrush TaskBackgroundColor
        {
            get
            {
                if (IsTaskOverDue())
                {
                    return new SolidColorBrush(Colors.Crimson);
                }
                else if (IsAlmostDueDate())
                {
                    return new SolidColorBrush(Colors.OrangeRed);
                }
                return new SolidColorBrush();
            }
        }

        public string loggedInUser;

        public SolidColorBrush TaskBorderColor
        {
            get
            {
                if (EmailAsignee.Equals(loggedInUser))
                {
                    return new SolidColorBrush(Colors.DodgerBlue);
                }
                return new SolidColorBrush();
            }

        }


        public TaskModel(string title, string description, DateTime creationTime, DateTime dueDate, string emailAsignee, BackendController controller, int taskId, int columnId) : base(controller)
        {
            _id = taskId;
            _columnID = columnId;
            _title = title;
            _description = description;
            _creationTime = creationTime;
            _dueDate = dueDate;
            _emailAsignee = emailAsignee;
        }

        public TaskModel(BackendController controller, ServiceTask t, int columnId, string loggedInUser) : base(controller)
        {
            _id = t.Id;
            _title = t.Title;
            _description = t.Description;
            _creationTime = t.CreationTime;
            _dueDate = t.DueDate;
            _emailAsignee = t.emailAssignee;
            _columnID = columnId;
            this.loggedInUser = loggedInUser;
        }

        /// <summary>
        /// checks if the dueDate of the task arrived or passed
        /// </summary>
        /// <returns>a boolean value, true for overdue, false for not overdue</returns>
        private bool IsTaskOverDue()
        {
            if (DateTime.Now.CompareTo(DueDate) > 0)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// checks if 75% of time has passed from creation time of the test
        /// </summary>
        /// <returns>a boolean value, true for almostoverdue, false for not almost overdue</returns>
        private bool IsAlmostDueDate()
        {
            double TotalPeriod = (DueDate.Subtract(CreationTime)).TotalDays;
            double TotalPassed = (DateTime.Now.Subtract(CreationTime) ).TotalDays;
            bool AlmostAtDue = (!(IsTaskOverDue()) && (TotalPeriod * 0.75) <= TotalPassed);

            return AlmostAtDue;
        }
    }
}
