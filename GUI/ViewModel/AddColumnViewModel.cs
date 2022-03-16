using GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModel
{
    class AddColumnViewModel : NotifiableObjects
    {
        private BackendController controller;
        private UserModel user;

        public AddColumnViewModel(BackendController controller, UserModel user)
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

        private string _columnName;
        public string ColumnName
        {
            get => _columnName;
            set
            {
                _columnName = value;
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
        /// try to add new column to the board
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        public bool AddColumn()
        {
            Message = "";
            try
            {
                user.Board.AddColumn(user.Email, ColumnID, ColumnName, user.Email);
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
