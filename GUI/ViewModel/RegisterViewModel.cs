using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModel
{
    public class RegisterViewModel : NotifiableObjects
    {
        public BackendController controller { get; private set; }
        private string _email;

        public RegisterViewModel(BackendController controller)
        {
            this.controller = controller;
        }

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

        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                RaisePropertyChanged("Nickname");
            }
        }

        private string _emailHost;
        public string EmailHost
        {
            get => _emailHost;
            set
            {
                _emailHost = value;
                RaisePropertyChanged("EmailHost");
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
        /// try to register to the board
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        public bool Register()
        {
            Message = "";
            if (string.IsNullOrEmpty(EmailHost))
            {
                try
                {
                    controller.Register(Email, Password, Nickname);
                    return true;
                }
                catch (Exception e)
                {
                    Message = e.Message;
                    return false;
                }
            }
            else
            {
                try
                {
                    controller.Register(Email, Password, Nickname, EmailHost);
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
}

