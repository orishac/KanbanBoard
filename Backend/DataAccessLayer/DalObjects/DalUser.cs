using System;
using System.Collections.Generic;
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DalUser : DalObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string EmailColumnName = "Email";
        public const string PasswordColumnName = "Password";
        public const string NicknameColumnName = "Nickname";
        public const string BoardsIdentifingEmail = "BoardsEmail";

        private UserController _controller;


        private string _email;
        private string _password;
        private string _nickname;
        private string _UsersBoard;

        public string Email { get => _email; }
        public string Password
        {
            get => _password;
            set
            {
                _controller.Update(_email, value, PasswordColumnName);
                _password = value;
                log.Debug(string.Format("user {0} updated column name {1} to parameter {2}", Email, PasswordColumnName, value));
            } }
        public string Nickname
        {
            get => _nickname;
            set
            {
                _controller.Update(_email,value,NicknameColumnName);
                _nickname = value;
                log.Debug(string.Format("user {0} updated column name {1} to parameter {2}", Email, NicknameColumnName, value));
            }
        }
        public string UsersBoard { get => _UsersBoard; set
            {
                _controller.Update(_email, value, BoardsIdentifingEmail);
            } }

        //only for tests
        public DalUser(string email)
        {
            _email = email;
            _password = "Aa123";
            _nickname = "test";
            _UsersBoard = "test@test.com";
        }

        public DalUser()
        {
            _controller = new UserController();
        }

        public DalUser(string email, string password, string nickname, string boardIdentifing)
        {
            _controller = new UserController();
            _email = email;
            _password = password;
            _nickname = nickname;
            _UsersBoard = boardIdentifing;
        }

        /// <summary>
        /// load all the users
        /// </summary>
        /// <returns> list of the users</returns>
        public List<DalUser> LoadData()
        {
            return _controller.selectUsers();
        }

        /// <summary>
        /// saves to object by inserting it to the table
        /// </summary>
        public void save()
        {
            if (!_controller.Insert(this))
            {
                log.Error("insert query did not succeed");
                throw new Exception("insert query did not succeed");
            }
            log.Debug(string.Format("user {0} has been added successuly to the table",Email));
        }

        //only for tests
        public virtual void save (DalUser dalUser)
        {
            
        }

        /// <summary>
        /// delete all the records of the object from the table
        /// </summary>
        public void RemoveData()
        {
            if (!_controller.DeleteTable())
            {
                log.Error("deleteTable query did not succeed");
                throw new Exception("deleteTable query did not succeed");
            }

            log.Debug("table 'Users' has been deleted successfuly from the DB");
        }

        public List<String> GetBoardCoUsers(string boardIdentifier)
        {
            return _controller.SelectCoUsers(boardIdentifier);
        }
    }
}

