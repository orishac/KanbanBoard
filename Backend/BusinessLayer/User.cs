using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class User
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string email;
        private string password;
        private string nickname;
        private bool loggedIn;
        private string belongingBoard;

        private const int MinPasswordLength = 5;
        private const int MaxPasswordLength = 25;

        //for tests
        public User(User u)
        {
            email = u.email;
            password = u.password;
            nickname = u.nickname;
            belongingBoard = u.belongingBoard;
            loggedIn = false;

        }

        public User(string email, string password, string nick, string belongingBoard)
        {
            this.email = email;
            if (!isLegitPassword(password))
            {
                log.Error("An attempt to create a user with illegal paswsord");
                throw new Exception("password is illegal");
            }
            this.password = password;
            nickname = nick;
            loggedIn = false;
            this.belongingBoard = belongingBoard;
        }

        public User(DataAccessLayer.DalUser dalUser)
        {
            email = dalUser.Email;
            password = dalUser.Password;
            nickname = dalUser.Nickname;
            belongingBoard = dalUser.UsersBoard;

        }
        ///checks if the password is correct
        ///if the password is correct - cahnges the loggedIn to true
        ///returns this user
        public virtual User login(string password)
        {
            if (!this.password.Equals(password))
            {
                log.Error("An attempt to login with the wrong password");
                throw new Exception("user name or password missmatch");
            }
            else
            {
                loggedIn = true;
                log.Debug("user" + this.email + " has logged-in");
                return this;
            }

        }

        ///changes the loggedIn field to false
        public void logout()
        {
            loggedIn = false;
            log.Debug("user" + this.email + "is now logged-off");
        }

        //only for tests
        public virtual void logout(bool isloggedin)
        {
            loggedIn = isloggedin;
            log.Debug("user" + this.email + "is now logged-off");
        }

        ///checks if the given old password is correct to verify that this is the user
        ///checks if the new password is legit by calling the isLegitPassword function
        ///changes the password field to the new password
        ///calls the save function to save the changes that was made
        public void changePassword(string newPassword, string OldPassword)
        {
            if (!this.password.Equals(OldPassword))
            {
                log.Error("An attempt to change password providing a wrong old password");
                throw new Exception("old passwords no match");
            }
            else if (!isLegitPassword(OldPassword))
            {
                log.Error("An attempt to change password to illegal one");
                throw new Exception("new password is illeagal");
            }
            else
            {
                toDalObject().Password = newPassword;

                this.password = newPassword;
                log.Info("user" + this.email + "has changed his password");
            }
        }

        ///checks if the new nickname is legit (not empty)
        ///changes the nickname field to the new nickname
        ///calls the save function to save the changes that was made
        public void changeNickname(string newNickname)
        {
            if (newNickname == null || newNickname.Length == 0)
            {
                log.Error("An attempt to change nickName to illegal one");
                throw new Exception("nickname cannot be null");
            }
            else
            {

                toDalObject().Nickname = newNickname;
                this.nickname = newNickname;
                log.Info("user" + this.email + "has changed his nickName");
            }
        }

        ///checks if the given password is legit 
        ///if the given password is null/its length is smaller than 5 or bigger than 25 return that it is not legit
        private bool isLegitPassword(String password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length==0|| password.Length < MinPasswordLength || password.Length > MaxPasswordLength)
            {
                return false;
            }
            else
            {
                bool hasNumber = password.Any(c => (c >= '0' && c <= '9'));
                bool hasCapitalLetter = password.Any(c => (c >= 'A' && c <= 'Z'));
                bool hasSmallLetter = password.Any(c => (c >= 'a' && c <= 'z'));
                return hasCapitalLetter && hasNumber && hasSmallLetter;
            }

        }

        ///return the email of this user
        public string getEmail()
        {
            return email;
        }

        ///makes a new Data Access Layer user using the toDalObject function
        ///saves the changes that was made calling the save function in the Data Access Layer user
        public void save(User u)
        {
            DataAccessLayer.DalUser user = u.toDalObject();
            user.save();
        }

       

        ///makes a new user in Data Access Layer represting this user
        private DataAccessLayer.DalUser toDalObject()
        {
            return new DataAccessLayer.DalUser(email, password, nickname, belongingBoard);
        }

        ///return the nickname of this user
        public string getNickName()
        {
            return nickname;
        }

        /// <summary>
        /// returns the email of the board's creator
        /// </summary>
        /// <returns></returns>
        public string GetBelongingBoard()
        {
            return belongingBoard;
        }

    }   
}
