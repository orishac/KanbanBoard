using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer.UserService
{
    class UserService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BusinessLayer.UserController userController;

        public UserService()
        {
            userController = new BusinessLayer.UserController();
        }

        /// <summary>        
        /// Loads the data. Intended be invoked only when the program starts
        /// </summary>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response LoadData()
        {
            try
            {
                userController.loadData();
                return new Response();

            } catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        ///<param name="nickname">the user nickname.</param>
        ///<returns cref="Response">The response of the action</returns>
        public Response Register(string email, string password, string nickname)
        {
            try
            {
                userController.register(email, password, nickname, email);
                return new Response();
            }
            catch (Exception e) {

                return new Response(e.Message);
            }
            
                 
        }

        public Response Register(string email, string password, string nickname, string emailHost)
        {
            try
            {
                userController.register(email, password, nickname, emailHost);
                return new Response();
            }
            catch (Exception e)
            {

                return new Response(e.Message);
            }


        }

        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        public Response<SerivceUser> Login(string email, string password)
        {
            try
            {
                userController.login(email, password);
                BusinessLayer.User user = userController.getUser(email);
                SerivceUser ServiceUser = new SerivceUser (user);
                return new Response<SerivceUser>(ServiceUser);
            }
            catch (Exception e)
            {
                return new Response<SerivceUser>(e.Message);
            }
             
        }

        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Logout(string email)
        {
            try
            {
                userController.logout(email);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }


        public BusinessLayer.UserController getController()
        {
            return userController;
        }

        /// <summary>
        /// remove all peristance data that belongs to user
        /// </summary>
        /// <returns> Response with error message</returns>
        public Response RemoveData()
        {
            try
            {
                userController.RemoveData();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
    }
}
