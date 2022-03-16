using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GUI.Model;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace GUI
{
    public class BackendController
    {
        public IService Service { get; private set; }

        public BackendController()
        {
            this.Service = new Service();
            Service.LoadData();
        }



        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        public void Logout(string email)
        {
            Response response = Service.Logout(email);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>the logged in user</returns>
        public UserModel Login (string email, string password)
        {
            Response<SerivceUser> user = Service.Login(email, password);
            if (user.ErrorOccured)
            {
                throw new Exception(user.ErrorMessage);
            }
            return new UserModel(this, user.Value);
        }


        /// <summary>
        /// Change the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="newName">The new name.</param>
        public void ChangeColumnName(string userEmail,int columnID ,string columnName)
        {
            Response response = Service.ChangeColumnName(userEmail, columnID, columnName);
            // TO-DO : what if error occured
            if (response.ErrorOccured)
            {
               
            }
        }

        /// <summary>
        /// Returns the board of a user. The user must be logged in
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>object of the board</returns>
        public BoardModel GetBoard(string host, string loggedInUser)
        {
            Response<ServiceBoard> board= Service.GetBoard(loggedInUser);
            ServiceBoard ServiceBoard= board.Value;
            BoardModel GuiBoard = new BoardModel(this, ServiceBoard, loggedInUser);
            return GuiBoard;
            
        }

        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Location to place to column</param>
        /// <param name="Name">new Column name</param>
        public ColumnModel AddColumn(string email, int columnID, string columnName, string loggedInUser)
        {
            Response <ServiceColumn> response = Service.AddColumn(email, columnID, columnName);
            if (response.ErrorOccured)
                throw new Exception(response.ErrorMessage);
            return new ColumnModel(this, response.Value, email, loggedInUser);
        }

        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnName">Column name</param>
        /// <returns>observable collection of columnModel objects</returns>
        public ObservableCollection<ColumnModel> GetColumnsByNames (string email, IReadOnlyCollection<string> columnNames, string loggedInUser)
        {
            ObservableCollection<ColumnModel> columns = new ObservableCollection<ColumnModel>();
            foreach (string name in columnNames)
            {
                Response<ServiceColumn> c = Service.GetColumn(email, name);
                ServiceColumn serviceColumn = c.Value;
                columns.Add(new ColumnModel(this, serviceColumn, email, loggedInUser));
            }
            return columns;
        }

  
        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        ///<param name="nickname">the user nickname.</param>
        public void Register(string email, string password, string nickname)
        {
            Response r = Service.Register(email, password, nickname);
            if (r.ErrorOccured)
            {
                throw new Exception(r.ErrorMessage);
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>      
        public void DeleteTask(string userEmail,int ColumnID, int ID)
        {
            Response response = Service.DeleteTask(userEmail, ColumnID, ID);
            if (response.ErrorOccured)
                throw new Exception(response.ErrorMessage);
        }

        /// <summary>
		/// Registers a new user and joins the user to an existing board.
		/// </summary>
		/// <param name="email">The email address of the user to register</param>
		/// <param name="password">The password of the user to register</param>
		/// <param name="nickname">The nickname of the user to register</param>
		/// <param name="emailHost">The email address of the host user which owns the board</param>
        public void Register(string email, string password, string nickname, string emailHost)
        {
            Response r = Service.Register(email, password, nickname, emailHost);
            if (r.ErrorOccured)
            {
                throw new Exception(r.ErrorMessage);
            }
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        public void UpdateTaskTitle(string taskAssignee, string newTitle, int taskID, int columnID)
        {
            Response r = Service.UpdateTaskTitle(taskAssignee, columnID, taskID, newTitle);
            if (r.ErrorOccured)
            {
                throw new Exception(r.ErrorMessage);
            }
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        public void UpdateTaskDescription(string taskAssignee, string newDescription, int taskID, int columnID)
        {
            Response r = Service.UpdateTaskDescription(taskAssignee, columnID, taskID, newDescription);
            if (r.ErrorOccured)
            {
                throw new Exception(r.ErrorMessage);
            }
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        public void UpdateTaskDueDate(string taskAssignee, DateTime newDateTime, int taskID, int columnID)
        {
            Response r = Service.UpdateTaskDueDate(taskAssignee, columnID, taskID, newDateTime);
            if (r.ErrorOccured)
            {
                throw new Exception(r.ErrorMessage);
            }
        }

        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnName">Column name</param>
        public void AdvanceTask(string assigneeEmail, int coloumnID, int taskId)
        {
            Response response = Service.AdvanceTask(assigneeEmail, coloumnID, taskId);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>the object of taskModel that was added</returns>
        public TaskModel AddTask (string email,string title, string description, DateTime dueDate)
        {
            Response<ServiceTask> response = Service.AddTask(email, title, description, dueDate);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            return new TaskModel(this, response.Value, 0, email);
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
		/// <param name="emailAssignee">Email of the user to assign to task to</param>
        public void AssignTask(string email, int columnId, int taskId, string newAssignee)
        {
            Response response = Service.AssignTask(email, columnId, taskId, newAssignee);
            if (response.ErrorOccured)
                throw new Exception(response.ErrorMessage);
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        public void LimitColumnTasks(string email, int columnID, int limit)
        {
            Response response =  Service.LimitColumnTasks(email, columnID, limit);
            if (response.ErrorOccured)
                throw new Exception(response.ErrorMessage);
        }

        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        public void RemoveColumn(string email, int columnID)
        {
            Response response = Service.RemoveColumn(email, columnID);
            if (response.ErrorOccured)
                throw new Exception(response.ErrorMessage);
        }

        /// <summary>
        /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the columns</param>
        public void MoveColumnRight(string email, int columnOrdinal)
        {
            Response response = Service.MoveColumnRight(email, columnOrdinal);
            if (response.ErrorOccured)
                throw new Exception(response.ErrorMessage);
        }

        /// <summary>
        /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the columns</param>
        public void MoveColumnLeft(string email, int columnOrdinal)
        {
            Response response = Service.MoveColumnLeft(email, columnOrdinal);
            if (response.ErrorOccured)
                throw new Exception(response.ErrorMessage);
        }


    }
}
