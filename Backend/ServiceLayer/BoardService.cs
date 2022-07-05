using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer.BoardService
{
    class BoardService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BusinessLayer.BoardController boardController;


        public BoardService(BusinessLayer.UserController userController)
        {
            boardController = new BusinessLayer.BoardController(userController);
        }

        /// <summary>        
        /// Loads the data. Intended be invoked only when the program starts
        /// </summary>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response LoadData()
        {
            try
            {
                boardController.loadData();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Returns the board of a user. The user must be logged in
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>A response object with a value set to the board, instead the response should contain a error message in case of an error</returns>
        public Response<ServiceBoard> GetBoard(string email)
        {
            try
            {
                BusinessLayer.Board board = boardController.getBoard(email);
                ServiceBoard SeviceBoard = new ServiceBoard(board.getColumnsNamesCollection(), email);
                return new Response<ServiceBoard>(SeviceBoard);
            }
            catch (Exception e)
            {
                return new Response<ServiceBoard>(e.Message);
            }

        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response LimitColumnTasks(string email, int columnOrdinal, int limit)
        {
            try
            {
                boardController.LimitColumnTasks(email, columnOrdinal, limit);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A response object with a value set to the Task, instead the response should contain a error message in case of an error</returns>
        public Response<ServiceTask> AddTask(string email, string title, string description, DateTime dueDate)
        {
            try
            {
                BusinessLayer.Task task = boardController.AddTask(email, title, description, dueDate);
                ServiceTask ServiceTask = new ServiceTask(task);
                return new Response<ServiceTask>(ServiceTask);

            }
            catch (Exception e)
            {
                return new Response<ServiceTask>(e.Message);
            }
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDueDate(string email, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                boardController.editDueDate(email, columnOrdinal, taskId, dueDate);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskTitle(string email, int columnOrdinal, int taskId, string title)
        {
            try
            {
                boardController.editTitle(email, columnOrdinal, taskId, title);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDescription(string email, int columnOrdinal, int taskId, string description)
        {
            try
            {
                boardController.editDescription(email, columnOrdinal, taskId, description);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AdvanceTask(string email, int columnOrdinal, int taskId)
        {
            try
            {
                boardController.advanceTask(columnOrdinal, email, taskId);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }


        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<ServiceColumn> GetColumn(string email, string columnName)
        {
            try
            {
                BusinessLayer.Column buisnessColumn = boardController.getColumnByName(email, columnName);

                IReadOnlyCollection<ServiceTask> tasks = convertCollection(buisnessColumn.getTasksCollection());

                ServiceColumn serviceColumn = new ServiceColumn(tasks, buisnessColumn.getColumnName(), buisnessColumn.getLimit(), buisnessColumn.getColumnId());
                return new Response<ServiceColumn>(serviceColumn);

            }
            catch (Exception e)
            {
                return new Response<ServiceColumn>(e.Message);
            }
        }

        /// <summary>
        /// Returns a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>

        public Response<ServiceColumn> GetColumn(string email, int columnOrdinal)
        {
            try
            {
                BusinessLayer.Column buisnessColumn = boardController.getColumnByID(email, columnOrdinal);

                IReadOnlyCollection<ServiceTask> tasks = convertCollection(buisnessColumn.getTasksCollection());

                ServiceColumn serviceColumn = new ServiceColumn(tasks, buisnessColumn.getColumnName(), buisnessColumn.getLimit(), buisnessColumn.getColumnId());
                return new Response<ServiceColumn>(serviceColumn);
            }
            catch (Exception e)
            {
                return new Response<ServiceColumn>(e.Message);
            }
        }

        /// <summary>
        /// register in board contoller, created new board
        /// </summary>
        /// <param name="email">email of the new user, sent as parameter to be the key of the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Register(string email)
        {
            try
            {
                boardController.register(email);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// private method. converts collection of Buissness layer tasks to Service layer tasks.
        /// </summary>
        /// <param name="collection">collection of Buissness layer tasks</param>
        /// <returns>collection of Service layer tasks</returns>
        private List<ServiceTask> convertCollection(IReadOnlyCollection<BusinessLayer.Task> collection)
        {
            List<ServiceTask> serviceTasks = new List<ServiceTask>();

            foreach (BusinessLayer.Task t in collection)
            {
                serviceTasks.Add(new ServiceTask(t));
            }

            return serviceTasks;
        }

        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveColumn(string email, int columnOrdinal)
        {
            try
            {
                boardController.removeColumn(email, columnOrdinal);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Location to place to column</param>
        /// <param name="Name">new Column name</param>
        /// <returns>A response object with a value set to the new Column, the response should contain a error message in case of an error</returns>
        public Response<ServiceColumn> AddColumn(string email, int columnOrdinal, string Name)
        {
            try
            {
                BusinessLayer.Column Bcolumn = boardController.addColumn(email, columnOrdinal, Name);
                ServiceColumn Scolumn = new ServiceColumn(convertCollection(Bcolumn.getTasksCollection()), Bcolumn.getColumnName(), Bcolumn.getLimit() ,Bcolumn.getColumnId());
                return new Response<ServiceColumn>(Scolumn);
            }
            catch (Exception e)
            {
                return new Response<ServiceColumn>(e.Message);
            }
        }

        /// <summary>
		/// Registers a new user and joins the user to an existing board.
		/// </summary>
		/// <param name="email">The email address of the user to register</param>
		/// <param name="password">The password of the user to register</param>
		/// <param name="nickname">The nickname of the user to register</param>
		/// <param name="emailHost">The email address of the host user which owns the board</param>
		/// <returns>A response object. The response should contain a error message in case of an error<returns>
        public Response Register(string email,string emailHost)
        {
            try
            {
                boardController.register(email,emailHost);
                return new Response();
            }
            catch(Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        /// <returns>A response object with a value set to the moved Column, the response should contain a error message in case of an error</returns>
        public Response<ServiceColumn> MoveColumnRight(string email, int columnOrdinal)
        {
            try
            {
                BusinessLayer.Column Bcolumn = boardController.MoveColumnRight(email, columnOrdinal);
                ServiceColumn serviceColumn = new ServiceColumn(convertCollection(Bcolumn.getTasksCollection()), Bcolumn.getColumnName(), Bcolumn.getLimit(), Bcolumn.getColumnId());
                return new Response<ServiceColumn>(serviceColumn);
            }
            catch (Exception e)
            {
                return new Response<ServiceColumn>(e.Message);
            }
        }

        /// <summary>
        /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the column</param>
        /// <returns>A response object with a value set to the moved Column, the response should contain a error message in case of an error</returns>
        public Response<ServiceColumn> MoveColumnLeft(string email, int columnOrdinal)
        {
            try
            {
                BusinessLayer.Column Bcolumn = boardController.MoveColumnLeft(email, columnOrdinal);
                ServiceColumn serviceColumn = new ServiceColumn(convertCollection(Bcolumn.getTasksCollection()), Bcolumn.getColumnName(), Bcolumn.getLimit(), Bcolumn.getColumnId());
                return new Response<ServiceColumn>(serviceColumn);
            }
            catch (Exception e)
            {
                return new Response<ServiceColumn>(e.Message);
            }
        }

        /// <summary>
        /// remove the persistance data of the boards
        /// </summary>
        public Response RemoveData()
        {
            try
            {
                boardController.RemoveData();
                return new Response();
            }
            catch(Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
		/// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
        {
            try
            {
                boardController.AssignTask(email, columnOrdinal, taskId, emailAssignee);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        		
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response DeleteTask(string email, int columnOrdinal, int taskId)
        {
            try
            {
                boardController.DeleteTask(email, columnOrdinal, taskId);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Change the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="newName">The new name.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response ChangeColumnName(string email, int columnOrdinal, string newName)
        {
            try
            {
                boardController.ChangeColumnName(email, columnOrdinal, newName);
                return new Response();
            }
            catch(Exception e)
            {
                return new Response(e.Message);
            }
            
        }
    }
}
