using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI.Model;

namespace GUI.ViewModel
{
    public class LoginViewModel : NotifiableObjects
    {
        public LoginViewModel()
        {
            this.Controller = new BackendController();
        }

        public BackendController Controller { get; private set; }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                RaisePropertyChanged("Email");
            }

        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
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
        /// try to login to the board
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        public UserModel Login()
        {
            Message = "";
            try
            {
                UserModel u = Controller.Login(Email, Password);
                return u;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return null;
            }
        }
    }
}
