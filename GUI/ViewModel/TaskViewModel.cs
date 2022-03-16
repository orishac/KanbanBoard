using GUI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModel
{
    public class TaskViewModel : NotifiableObjects
    {
        public TaskModel task { get; set; }

        private string _title;
        public string Title 
        { 
            get => _title; 
            set
            {
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
                _description = value;
                RaisePropertyChanged("Description");
            }
        }
        private DateTime _dueDate;
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                RaisePropertyChanged("DueDate");
            }
        }
        public string CreationTime
        {
            get => task.CreationTime.ToString();
        }
        private string _taskAssignee;
        public string TaskAssingee
        {
            get => _taskAssignee;
            set
            {
                _taskAssignee = value;
                RaisePropertyChanged("TaskAssignee");
            }
        }
        
        public BoardModel board;
        private BackendController controller;
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }



        public TaskViewModel(TaskModel t, BackendController controller, BoardModel board)
        {
            this.task = t;
            _title = t.Title;
            _description = t.Description;
            _dueDate = t.DueDate;
            _taskAssignee = t.EmailAsignee;
            this.controller = controller;
            this.board = board;
        }

        /// <summary>
        /// try to delete task from the board
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        public bool DeleteTask()
        {
            Message = "";
            try
            {
                board.deleteTask(task);
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        /// <summary>
        /// try to advance task to the next column in the board
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        public bool AdvanceTask()
        {
            Message = "";
            try
            {
                controller.AdvanceTask(task.EmailAsignee, task.ColumnID, task.ID);
                board.AdvanceTask(task);
                task.ColumnID++;
                return true;
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
            
        }

        public bool Save()
        {
            Message = "";
            try
            {
                if(!task.Title.Equals(Title) )
                    task.Title = Title;
                if(!task.Description.Equals(Description) )
                    task.Description = Description;
                if(!task.DueDate.Equals(DueDate))
                    task.DueDate = DueDate;
                if (!task.EmailAsignee.Equals(TaskAssingee))
                    task.EmailAsignee = TaskAssingee;
                return true;
                
            }
            catch(Exception e)
            {
                Message = e.Message;
                return false;
            }
        }
        

    }
}
