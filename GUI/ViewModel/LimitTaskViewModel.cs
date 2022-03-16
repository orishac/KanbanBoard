using GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModel
{
    class LimitTaskViewModel : NotifiableObjects
    {
        private BackendController controller;
        private UserModel user;

        public LimitTaskViewModel(BackendController controller, UserModel user)
        {
            this.controller = controller;
            this.user = user;
        }

        private int _columnID;
        public int ColumnID
        {
            get => _columnID;
            set
            {
                _columnID = value;
                RaisePropertyChanged("ColumnName");
            }

        }

        private int _columnLimit;
        public int ColumnLimit
        {
            get => _columnLimit;
            set
            {
                _columnLimit = value;
                RaisePropertyChanged("ColumnLimit");
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

        /// <summary>
        /// try to limit column's tasks in the board
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        public bool LimitTasks()
        {
            Message = "";
            try
            {
                controller.LimitColumnTasks(user.Email, ColumnID, ColumnLimit);
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
