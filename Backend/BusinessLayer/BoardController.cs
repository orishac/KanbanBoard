using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class BoardController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<string, Board> boards;
        private UserController userController;

        public BoardController(UserController userController)
        {
            boards = new Dictionary<string, Board>();
            this.userController = userController;
        }

        /// <summary>
        /// checks if the user is registered and if he is logged in
        /// </summary>
        /// <param name="email">Email of the user.</param>
        /// <returns></returns>
        private bool ValidateEmail(string email)
        {

            if (!boards.ContainsKey(email))
            {
                log.Error("An attempt to a board of a non registerd user");
                return false;
            }
            if (!userController.isLoggedin(email))
            {
                log.Error("An attempt to access the board of logged-off user");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// get email of user check if it loggedin and Registed and return the host email
        /// </summary>
        /// <param name="user"> user Identifing (currently the email)</param>
        /// <returns> the host Email </returns>
        private string GetHostEmail(string user)
        {
            if (!userController.isLoggedin(user))
                throw new Exception("trying to access not loggedIn User");
            return userController.GetHostEmail(user);
        }

        /// <summary>
        /// if the host is not logged-in, in some functions we would like to check whether one of the co-users in logged-in
        /// </summary>
        /// <param name="email">email of the host</param>
        /// <returns>boolean value indicates whether the access to baord is allowed or not</returns>
        private bool ValidateUser(string email)
        {
            foreach (string co_user in boards[email].GetUsersList())
            {
                if (userController.isLoggedin(co_user))
                {
                    return true;
                }
            }
            log.Error("An attempt to access the board of logged-off user");
            return false;

        }

        /// <summary>
        /// register when there is already exsiting board. adding the new user to co_users list
        /// </summary>
        /// <param name="email">Baord's creator email</param>
        /// <param name="emailHost">new user email</param>
        public void register(string email, string emailHost)
        {
            if (!boards.ContainsKey(emailHost))
            {
                log.Error("An attempt to register when the email Host is not registered");
                throw new Exception("There isn't an exsiting email-host user with the current email");
            }
            Board hostBoard = getBoard(emailHost);
            hostBoard.addNewUser(email);

        }

        ///gets a string email, and if there's no board in the boards list that associated with this email, makes a new board
        ///and add it to the boards list
        public Board register(string email)
        {
            if (boards.ContainsKey(email) || boards.ContainsKey(email.ToLower()))
            {
                log.Error("An attempt to register an exsiting user");
                throw new Exception("There is already an exsiting user with the current email");
            }

            Board newBoard = new Board(email);
            boards.Add(email, newBoard);
            log.Debug("New board has been created for the new user " + email);
            newBoard.save();

            return boards[email];
        }

        /// <summary>
        /// add new column to the requsted board.
        /// </summary>
        /// <param name="email">the requsted board</param>
        /// <param name="columnID">columnID of the new column - determins its location in the board</param>
        /// <param name="name">name of the new column</param>
        /// <returns>the added column</returns>
        public Column addColumn(string email, int columnID, string name)
        {
            if (!ValidateEmail(email))
                throw new Exception("User is not registered of not logged in");

            return boards[email].AddColumn(email,columnID, name);

        }

        /// <summary>
        /// removes requsted column from the requsted board
        /// </summary>
        /// <param name="email">the id of the column</param>
        /// <param name="columnID">the id of the column we need to remove</param>
        public void removeColumn(string email, int columnID)
        {
            if (!ValidateEmail(email))
                throw new Exception("User is not registered of not logged in");

            boards[email].RemoveColumn(email,columnID);
        }

        ///gets a string email, searches in the boards list for the board associated with this email, and return it
        public Board getBoard(string email)
        {
            string host = GetHostEmail(email);
            return boards[host];
        }

        ///recevies email, and title, body and duetime of a new task
        ///searches for the board associated with this email in the boards list
        ///if found, calls for the addTask function in Boards.
        public Task AddTask(string email, string title, string body, DateTime dueDate)
        {

            string hostEmail = GetHostEmail(email);
            return boards[hostEmail].AddTask(email, title, body, dueDate);

        }

        ///recevies email, collum Id and Task Id
        ///searches for the board associated with this email in the boards list
        ///if found, calls for the advanceTask function in Boards.
        public void advanceTask(int columnId, string email, int taskId)
        {
            string hostEmail = GetHostEmail(email);
            boards[hostEmail].advanceTask(email, columnId, taskId);

        }

        ///recevies email, collum Id and Task Id and new title
        ///searches for the board associated with this email in the boards list
        ///if found, calls for the editTaskTitle function in Boards.
        public void editTitle(string email, int columnId, int taskId, string newTitle)
        {
            string hostEmail = GetHostEmail(email);
            boards[hostEmail].editTaskTitle(email, columnId, taskId, newTitle);

        }

        ///recevies email, collum Id, Task Id and new DueDate
        ///searches for the board associated with this email in the boards list
        ///if found, calls for the editTaskDueDate function in Boards.
        public void editDueDate(string email, int columnId, int taskId, DateTime newDueDate)
        {
            string hostEmail = GetHostEmail(email);
            boards[hostEmail].editTaskDueDate(email, columnId, taskId, newDueDate);

        }

        ///recevies email, collum Id, Task Id and new description
        ///searches for the board associated with this email in the boards list
        ///if found, calls for the editTaskDescription function in Boards.
        public void editDescription(string email, int columnId, int taskId, string newDescription)
        {
            string hostEmail = GetHostEmail(email);
            boards[hostEmail].editTaskDescription(email, columnId, taskId, newDescription);

        }

        ///recevies email, collum Id, and new limit
        ///searches for the board associated with this email in the boards list
        ///if found, calls for the SetLimit function in Boards.
        public void LimitColumnTasks(string email, int columnOrdinal, int limit)
        {
            if (!ValidateEmail(email))
                throw new Exception("User is not registered of not logged in");

            boards[email].SetLimit(email, limit, columnOrdinal);

        }

        ///recevies email and collum Id
        ///searches for the board associated with this email in the boards list
        ///if found, calls for the getColumnByID function in Boards.
        public Column getColumnByID(string email, int coloumsId)
        {
            string host = GetHostEmail(email);
            return boards[host].getColumnByID(coloumsId);


        }

        ///recevies email and collum name
        ///searches for the board associated with this email in the boards list
        ///if found, calls for the getColumnByName function in Boards.
        public Column getColumnByName(string email, string coloumsName)
        {
            string host = GetHostEmail(email);
            return boards[host].getColumnByName(coloumsName);

        }

        ///makes a new board in Data Access Layer
        ///calls for the load function in the new board that was created and makes a new list of Boards in Data Access Layer
        ///using a loop, add to this (the BoardController) list, every board that was loaded to the list of Boards in Data Access Layer
        ///calls for the loadData function in userController to load the user lists
        public void loadData()
        {
            DataAccessLayer.DalBoard board = new DataAccessLayer.DalBoard();
            List<DataAccessLayer.DalBoard> dalBoards = board.LoadData();
            foreach (DataAccessLayer.DalBoard dalBoard in dalBoards)
            {

                boards.Add(dalBoard.EmailCreator, new Board(dalBoard));
            }

            log.Debug("data has been loaded successfully in board controller");

        }

        /// <summary>
        /// moves the requsted column left in the requsted board.
        /// </summary>
        /// <param name="email">key of the requsted board</param>
        /// <param name="columnOrdinal">id of the requsted column to remove</param>
        /// <returns>the column we moved</returns>
        public Column MoveColumnRight(string email, int columnOrdinal)
        {
            if (!ValidateEmail(email))
                throw new Exception("User is not registered of not logged in");

            return boards[email].MoveRight(email, columnOrdinal);
        }

        /// <summary>
        /// moves the requsted column right in the requsted board.
        /// </summary>
        /// <param name="email">key of the requsted board</param>
        /// <param name="columnOrdinal">id of the requsted column to remove</param>
        /// <returns>the column we moved</returns>
        public Column MoveColumnLeft(string email, int columnOrdinal)
        {
            if (!ValidateEmail(email))
                throw new Exception("User is not registered of not logged in");

            return boards[email].MoveLeft(email, columnOrdinal);
        }

        /// <summary>
        /// delete all percitance data of board
        /// </summary>
        public void RemoveData()
        {
            new DataAccessLayer.DalBoard().RemoveData();
            new DataAccessLayer.DalColumn().RemoveData();
            new DataAccessLayer.DalTask().RemoveData();
            boards.Clear();
            log.Debug("data has been removed successfully in board controller");
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        public void AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            string hostEmail = GetHostEmail(email);
            boards[hostEmail].AssignTask(email, columnOrdinal, taskId, emailAssignee);
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>      
        public void DeleteTask(string email, int columnOrdinal, int taskId)
        {
            string hostEmail = GetHostEmail(email);
            boards[hostEmail].DeleteTask(email, columnOrdinal, taskId);
        }

        /// <summary>
        /// Change the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="newName">The new name.</param>
        public void ChangeColumnName(string email, int columnOrdinal, string newName)
        {
            if (!ValidateEmail(email))
                throw new Exception("User is not registered of not logged in");
            boards[email].ChangeColumnName(email, columnOrdinal, newName);
        }


    }
}
