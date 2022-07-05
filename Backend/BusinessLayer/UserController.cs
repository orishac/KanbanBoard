using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;


namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class UserController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<string, User> users;
        private User currentUser;
        
        //only for tests
        public UserController(Dictionary<string, User> users)
        {
            this.currentUser = null;
            this.users = users;
        }

        public UserController()
        {
            this.currentUser = null;
            users = new Dictionary<string, User>();

        }

        ///checks if the email/password/nickname is valid (not null)
        ///checks if there is already a user that registered through this email
        ///makes a new user with the given email,password and nickname and adds it to the list of users
        public void register(string email, string password, string nickname, string belongingBoard)
        {
            if (!IsValidEmail(email) || !IsValidEmail(belongingBoard) || string.IsNullOrWhiteSpace(nickname) || nickname.Length == 0)
            {
                log.Error("An attempt to register new user with illegal parameters");
                throw new Exception("email or nickname cannot be null");
            }
            else if (users.ContainsKey(email))
            {
                log.Error("An attempt to register an exsiting user");
                throw new Exception("there is an exsiting user with " + email);
            }
            else
            {
                User user = new User(email, password, nickname, belongingBoard);
                users.Add(email, user);
                log.Debug("New user " + email + " has been registered");
                user.save(user); 
            }
        }


        //only for test purposes
        public void DemiRegister(string email, string password, string nickname, string belongingBoard)
        {
            if (!IsValidEmail(email) || !IsValidEmail(belongingBoard) || string.IsNullOrWhiteSpace(nickname) || nickname.Length == 0)
            {
                log.Error("An attempt to register new user with illegal parameters");
                throw new Exception("email or nickname or password are illegal");
            }
            else if (users.ContainsKey(email))
            {
                log.Error("An attempt to register an exsiting user");
                throw new Exception("there is an exsiting user with " + email);
            }
            else
            {
                User user = new User(email, password, nickname, belongingBoard);
                users.Add(email, user);
            }
        }


        ///checks if there is another user loggen in the system
        ///checks if user registered to the system
        ///calls the login function in the user class assigned to this email
        public void login(string email, string password)
        {
            if (currentUser != null)
            {
                log.Warn("An attempt to login when another user is already logged in");
                throw new Exception("only one user can be logged in");
            }
            else if (!users.ContainsKey(email))
            {
                log.Warn("An attempt of user to login when not registered");
                throw new Exception("the User is not registered");
            }
            else
            {
                User user = users[email];
                currentUser = user.login(password);

            }
        }

        ///checks if this user registered to the system
        ///checks if this user is logged in (only if the user is already logged in logout is possible)
        ///calls for the logout function in the user assinged to this user;
        ///
        public void logout(string email)
        {
            if (!users.ContainsKey(email))
            {
                log.Error("An attempt of user to logout when not registered");
                throw new Exception("user is not registered");
            }
            User user = users[email];
            if (!isLoggedin(email))
            {
                log.Error("An attempt to logout when the user is not logged-in");
                throw new Exception("user is not logged in");
            }
            user.logout();
            currentUser = null;
        }

        ///checks if this user registered to the system
        ///checks if this user is logged in (can't get user that is logged out)
        ///return the user assigned to given email
        public User getUser(string email)
        {
            if (!users.ContainsKey(email))
            {
                log.Error("An attempt to access to user that does not exist");
                throw new Exception("user is not registered");
            }
            else if (!isLoggedin(email))
            {
                log.Error("An attempt to access a user that is not logged-in");
                throw new Exception("user is not logged In");
            }
            return users[email];

        }

        ///checks if there is a user logged in the system
        ///checks the email of the current logged in user and if it matches the given email, return that the user loggen in
        public bool isLoggedin(string email)
        {
            if (currentUser == null)
            {
                return false;
            }
            return currentUser.getEmail().Equals(email);
        }

        ///checks if this user registered to the system
        ///checks if this user is logged in (can't change nickname to a user that is logged out)
        ///if the user is logged in - calls the changeNickname function in the user assigned to the given email with the new nickname
        public void changeNickname(string email, string newNickname)
        {
            if (!users.ContainsKey(email))
            {
                log.Error("An attempt to change nickName to a non-registered user");
                throw new Exception("user is not registered");
            }
            else if (!isLoggedin(email))
            {
                log.Error("An attempt to change nickName to a logged-off user");
                throw new Exception("user is not logged In");
            }
            else
            {               
                users[email].changeNickname(newNickname);

            }
        }

        ///checks if this user registered to the system
        ///checks if this user is logged in (can't change password to a user that is logged out)
        ///if the user is logged in - calls the changePassword function in the user assigned to the given email with the old and new password
        public void changePassword(string email, string oldPassword, string newPassword)
        {
            if (!users.ContainsKey(email))
            {
                log.Error("An attempt to change nickName to a non-registered user");
                throw new Exception("user is not registered");
            }
            else if (!isLoggedin(email))
            {
                log.Error("An attempt to change nickName to a logged-off user");
                throw new Exception("user is not logged In");
            }
            users[email].changePassword(oldPassword, newPassword);
        }

        ///makes a new user in Data Access Layer
        ///calls for the load function in the new user that was created and makes a new list of users in Data Access Layer
        ///using a loop, add to this (the UserController) list every user that was loaded to the list of users in Data Access Layer
        public void loadData()
        {
            DataAccessLayer.DalUser user = new DataAccessLayer.DalUser();
            List<DataAccessLayer.DalUser> dalUsers = user.LoadData();
            foreach (DataAccessLayer.DalUser daluser in dalUsers)
            {

                users.Add(daluser.Email, new User(daluser));
            }

            log.Debug("data has been loaded successfully in user controller");
        }

        /// <summary>
        /// remove the persistance data of the users
        /// </summary>
        public void RemoveData()
        {
            DataAccessLayer.DalUser dalUser = new DataAccessLayer.DalUser();
            dalUser.RemoveData();
            users.Clear();

            log.Debug("data has been removed successfully in user controller");
        }

        ///checks is the user's email is a valid email adress
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public string getCurrentUser()
        {
            if (currentUser != null)
            {
                return currentUser.getEmail();
            }
            else
                return null;
        }

        public string GetHostEmail( string email)
        {
            if (!isLoggedin(email))
                throw new Exception("trying to access user that's not logged in");
            if (!users.ContainsKey(email))
                throw new Exception("user is not registed");
            return users[email].GetBelongingBoard();
        }

        public void SetCurrentUser(User u)
        {
            currentUser = u;
        }
    }


}
