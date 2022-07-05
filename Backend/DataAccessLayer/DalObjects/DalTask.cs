using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DalTask : DalObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TaskController controller;
        public const string EmailColumnName = "Email";
        public const string TaskColumnIDName = "ColumnID";
        public const string TaskIDColumnName = "TaskID";
        public const string TaskTitleColumnName = "Title";
        public const string TaskDescriptionColumnName = "Description";
        public const string TaskDueDateColumnName = "DueDate";
        public const string TaskCreationTimeColumnName = "CreationTime";
        public const string EmailAssigneeColumnName = "EmailAssignee"; 


        private string _Email;
        public string Email
        {
            get => _Email;
        }

        private int _ColumnID;
        public int ColumnID { 
            get  => _ColumnID; 
            set {
                controller.Update(Email, ColumnID, ID, TaskColumnIDName, value);
                _ColumnID = value;
                log.Debug(string.Format("task {0} updated column name {1} to parameter {2}", ID, TaskColumnIDName, value));
            } 
        }

        private int _ID;
        public int ID { 
            get => _ID; 
            set { 
                controller.Update(Email, ColumnID, ID, TaskIDColumnName, value);
                _ID = value;
            }
        }

        private string _Title;
        public string Title { 
            get => _Title; 
            set { 
                controller.Update(Email, ColumnID, ID, TaskTitleColumnName, value);
                log.Debug(string.Format("task {0} updated column name {1} to parameter {2}", ID, TaskTitleColumnName, value));
                _Title = value;
            } 
        }

        private string _Description;
        public string Description {
            get => _Description;
            set
            {
                controller.Update(Email, ColumnID, ID, TaskDescriptionColumnName, value);
                _Description = value;
                log.Debug(string.Format("task {0} updated column name {1} to parameter {2}", ID, TaskDescriptionColumnName, value));
            }
        }

        private DateTime _DueDate;
        public DateTime DueDate {
            get => _DueDate;
            set
            {
                controller.Update(Email, ColumnID, ID, TaskDueDateColumnName, value);
                log.Debug(string.Format("task {0} updated column name {1} to parameter {2}", ID, TaskDueDateColumnName, value.ToString()));
                _DueDate = value;
            }
        }

        private DateTime _CreationTime;
        public DateTime CreationTime { 
            get => _CreationTime; 
            set {
                controller.Update(Email, ColumnID, ID, TaskCreationTimeColumnName, value);
                log.Debug(string.Format("task {0} updated column name {1} to parameter {2}", ID, TaskCreationTimeColumnName, value.ToString()));
                _CreationTime = value;
            }
        }

        private string _EmailAssignee;

        public string EmailAssignee
        {
            get => _EmailAssignee;
            set
            {
                controller.Update(Email, ColumnID, ID, EmailAssigneeColumnName, value);
                log.Debug(string.Format("task {0} updated column name {1} to parameter {2}", ID, EmailAssigneeColumnName, value));
                _EmailAssignee = value;
            }
        }


        public DalTask(string email, int columnId, int id, string title, string description, DateTime creationTime, DateTime dueDate, string emailAssignee)
        {
            _Email = email;
            _ColumnID = columnId;
            _ID = id;
            _Title = title;
            _Description = description;
            _DueDate = dueDate;
            _CreationTime = creationTime;
            _EmailAssignee = emailAssignee;
            controller = new TaskController();
        }

        public DalTask()
        {
            controller = new TaskController();
        }

        /// <summary>
        /// load tasks of specific column from the DB
        /// </summary>
        /// <returns> list of the tasks</returns>
        public List<DalTask> GetTasks(string email, int columnID)
        {
            return controller.SelectColumnTask(email, columnID);
        }

        /// <summary>
        /// saves to object by inserting it to the table
        /// </summary>
        public void save()
        {
            if (!controller.Insert(this))
            {
                log.Error("insert query did not succeed");
                throw new Exception("insert query did not succeed");
            }
            log.Debug("task " + ID + " has been inserted to Tasks table successfuly");
        }

        /// <summary>
        /// delete all the records of the object from the table
        /// </summary>
        public void RemoveData()
        {
            if (!controller.DeleteTable())
            {
                log.Error("deleteTable query did not succeed");
                throw new Exception("deleteTable query did not succeed");
            }
            log.Debug("table 'Tasks' has been deleted successfuly from the DB");
        }

        /// <summary>
        /// delete record of the object from the DB
        /// </summary>
        public void Delete()
        {
            if (!controller.Delete(this))
            {
                log.Error("delete query did not succeed");
                throw new Exception("delete query did not succeed");
            }
            log.Debug(string.Format("task {0} has been deleted", ID));
        }

    }
}
