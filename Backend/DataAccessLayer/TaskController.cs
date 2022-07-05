using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class TaskController : DalController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string TaskTableName = "Tasks";

        public TaskController() : base(TaskTableName)
        {

        }

        /// <summary>
        /// selects from the DB all the tasks from specified columnID
        /// <param name="email">The email of the board contains the task</param>
        /// <param name="columnID">The column ID that contains the task</param>
        /// </summary>
        /// <returns> list of the relvant tasks</returns>
        public List<DalTask> SelectColumnTask(string email, int columnID)
        {
            List<DalTask> taskList = new List<DalTask>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(connection)
                {
                    CommandText = $"select* from {_tableName} where [{DalTask.EmailColumnName}] = @EmailVal AND [{DalTask.TaskColumnIDName}] = @ColumnID"
                };
                command.Parameters.Add(new SQLiteParameter("emailVal", email));
                command.Parameters.Add(new SQLiteParameter("ColumnID", columnID));
                SQLiteDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(2);
                        string title = reader.GetString(3);
                        string desc = reader.IsDBNull(4) ? null : reader.GetString(4);
                        DateTime creationDate = reader.GetDateTime(5);
                        DateTime dueDate = reader.GetDateTime(6);
                        string assignee = reader.GetString(7);
                        taskList.Add(new DalTask(email, columnID,id, title, desc, creationDate, dueDate ,assignee));
                    }
                }
                catch(Exception e)
                {
                    log.Fatal(e.Message);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    command.Dispose();
                    connection.Close();
                }

            }
            log.Debug("The tasks from " + columnID + " has been selected successfuly");
            return taskList;
        }

        /// <summary>
        /// converts the reader output to task dal object
        /// </summary>
        /// <param name="reader">reader of the DB, returns the records</param>
        /// <returns>the converted object</returns>
        protected override DalObject Convert(SQLiteDataReader reader)
        {
            DalTask result = new DalTask(reader.GetString(0), (int)reader.GetValue(1), (int)reader.GetValue(2), reader.GetString(3), reader.GetString(4), reader.GetDateTime(5), reader.GetDateTime(6), reader.GetString(7));

            return result;

        }

        /// <summary>
        /// inserts new record of task to Tasks table
        /// </summary>
        /// <param name="task"> object to insert</param>
        /// <returns>boolean value indicates whether the method succeeded </returns>
        public bool Insert(DalTask task)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;

                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {TaskTableName} ({DalTask.EmailColumnName} , {DalTask.TaskColumnIDName} , {DalTask.TaskIDColumnName} , {DalTask.TaskTitleColumnName} , {DalTask.TaskDescriptionColumnName} , {DalTask.TaskCreationTimeColumnName} , {DalTask.TaskDueDateColumnName}, {DalTask.EmailAssigneeColumnName})" +
                         $"VALUES (@emailVal,@columnIdVal,@idVal,@titleVal,@descriptionVal,@creationTimeVal,@dueDateVal, @Assignee)";

                    command.Parameters.Add(new SQLiteParameter(@"emailVal", task.Email));
                    command.Parameters.Add(new SQLiteParameter(@"columnIdVal", task.ColumnID));
                    command.Parameters.Add(new SQLiteParameter(@"idVal", task.ID));
                    command.Parameters.Add(new SQLiteParameter(@"titleVal", task.Title));
                    command.Parameters.Add(new SQLiteParameter(@"descriptionVal", task.Description));
                    command.Parameters.Add(new SQLiteParameter(@"creationTimeVal", task.CreationTime));
                    command.Parameters.Add(new SQLiteParameter(@"dueDateVal", task.DueDate));
                    command.Parameters.Add(new SQLiteParameter(@"Assignee", task.EmailAssignee));
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }
                
                return res > 0;
                
            }
           
        }

        /// <summary>
        /// updates a record in Tasks table
        /// </summary>
        /// <param name="email">The email of the board contains the task</param>
        /// <param name="columnID">The columnID contains the task</param>
        /// <param name="TaskID"> task id to update</param>
        /// <param name="columnName">column name to update</param>
        /// <param name="parameter"> the new value of the attribute, string type value </param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool Update(string email, int columnID, int TaskID, string columnName, string parameter)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"UPDATE {TaskTableName} set [{columnName}] = @paramVal WHERE {DalTask.EmailColumnName} = @emailParam and {DalTask.TaskColumnIDName} = @columnIDParam and {DalTask.TaskIDColumnName} = @idParam";

                    SQLiteParameter paramVal = new SQLiteParameter(@"paramVal", parameter);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailParam", email);
                    SQLiteParameter columnIDParam = new SQLiteParameter(@"columnIDParam", columnID);
                    SQLiteParameter IDparam = new SQLiteParameter(@"idParam", TaskID);

                    command.Parameters.Add(paramVal);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(columnIDParam);
                    command.Parameters.Add(IDparam);

                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }
               
                return res > 0;
            }
        }

        /// <summary>
        /// updates a record in Tasks table
        /// </summary>
        /// <param name="email">The email of the board contains the task</param>
        /// <param name="columnID">The columnID contains the task</param>
        /// <param name="TaskID"> task id to update</param>
        /// <param name="columnName">column name to update</param>
        /// <param name="parameter"> the new value of the attribute, int type value </param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool Update(string email, int columnID, int TaskID, string columnName, int parameter)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"UPDATE {TaskTableName} set [{columnName}] = @paramVal WHERE {DalTask.EmailColumnName} = @emailParam and {DalTask.TaskColumnIDName} = @columnIDParam and {DalTask.TaskIDColumnName} = @idParam";

                command.Parameters.Add(new SQLiteParameter(@"paramVal", parameter));
                command.Parameters.Add(new SQLiteParameter(@"emailParam", email));
                command.Parameters.Add(new SQLiteParameter(@"columnIDParam", columnID));
                command.Parameters.Add(new SQLiteParameter(@"idParam", TaskID));
                int res = -1;

                try
                {
                    connection.Open();
                    

                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }
                
                return res > 0;
            }
        }

        public bool Update(string email, int columnID, int TaskID, string columnName, DateTime parameter)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"UPDATE {TaskTableName} set [{columnName}] = @paramVal WHERE {DalTask.EmailColumnName} = @emailParam and {DalTask.TaskColumnIDName} = @columnIDParam and {DalTask.TaskIDColumnName} = @idParam";

                command.Parameters.Add(new SQLiteParameter(@"paramVal", parameter));
                command.Parameters.Add(new SQLiteParameter(@"emailParam", email));
                command.Parameters.Add(new SQLiteParameter(@"columnIDParam", columnID));
                command.Parameters.Add(new SQLiteParameter(@"idParam", TaskID));
                int res = -1;

                try
                {
                    connection.Open();


                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

                return res > 0;
            }
        }

        /// <summary>
        /// delete record of task from Tasks table
        /// </summary>
        /// <param name="toDelete"> object to delete</param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        /// 
        public bool Delete(DalTask toDelete)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand(null, connection);
                int res = -1;

                try
                {
                    connection.Open();
                    command.CommandText = $"delete from {_tableName} where {DalTask.EmailColumnName} =@emailParam and {DalTask.TaskColumnIDName} =@columnIdParam and {DalTask.TaskIDColumnName} =@idParam";

                    SQLiteParameter emailParam = new SQLiteParameter(@"emailParam", toDelete.Email);
                    SQLiteParameter columnIDParam = new SQLiteParameter(@"columnIDParam", toDelete.ColumnID);
                    SQLiteParameter IDparam = new SQLiteParameter(@"idParam", toDelete.ID);

                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(columnIDParam);
                    command.Parameters.Add(IDparam);

                    res = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                
                return res > 0;
            }

        }

    }

}

