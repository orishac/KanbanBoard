using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI.Model;

namespace GUI.ViewModel
{
    class AddTaskViewModel : NotifiableObjects
    {
        private BackendController controller;
        private UserModel user;

        public AddTaskViewModel(BackendController controller, UserModel user)
        {
            this.controller = controller;
            this.user = user;
        }

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

        /// <summary>
        /// try to add new task to the board
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        public bool AddTask()
        {
            Message = "";
            try
            {
                user.Board.AddTask(user.Email, Title, Description, DueDate);
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }
        }
    }
}
