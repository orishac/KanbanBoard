using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;



namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public abstract class DalController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly string _connectionString;
        protected readonly string _tableName;

        public DalController(string tableName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "database.db"));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = tableName;
            CreateDB(path);

        }

        /// <summary>
        /// select all records from requseted table
        /// </summary>
        /// <returns> list of the selected object from the Database</returns>
        public List<DalObject> Select()
        {
            List<DalObject> result = new List<DalObject>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(connection)
                {
                    CommandText = $"select* from {_tableName}"
                };
                SQLiteDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        result.Add(Convert(reader));
                    }
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    command.Dispose();
                    connection.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// delete all the records from a requsted table in the DB
        /// </summary>
        /// <returns> boolean value indicates whether the method succeeded</returns>
        public bool DeleteTable()
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} "
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

                return res > -1;
            }
        }

        protected abstract DalObject Convert(SQLiteDataReader reader);

        private void CreateDB(string path)
        {
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile("database");
                createUserTable();
                createBoardTable();
                createColumnTable();
                createTaskTable();
            }
        }

        private void createUserTable()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"create table Users ({DalUser.EmailColumnName} text not null, {DalUser.PasswordColumnName} text not null, {DalUser.NicknameColumnName} text not null," +
                    $"{DalUser.BoardsIdentifingEmail} text not null, primary key ({DalUser.EmailColumnName}))";
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
        }

        private void createBoardTable()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"create table Boards ({DalBoard.EmailColumnName} text not null, {DalBoard.IdGeneratorColumnName} integer not null," +
                    $"primary key ({DalBoard.EmailColumnName}))";
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
        }

        private void createColumnTable()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"create table Columns ({DalColumn.EmailColumnName} text not null, {DalColumn.ColumnColumnID} integer not null," +
                    $"{DalColumn.ColumnColumnName} text not null, {DalColumn.ColumnColumnLimit} integer not null," +
                    $"primary key ({DalColumn.EmailColumnName}, {DalColumn.ColumnColumnID}))";
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
        }

        private void createTaskTable()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"create table Tasks ({DalTask.EmailColumnName} text not null, {DalTask.TaskColumnIDName} integer not null," +
                    $"{DalTask.TaskIDColumnName} integer not null, {DalTask.TaskTitleColumnName} text not null, {DalTask.TaskDescriptionColumnName} text," +
                    $"{DalTask.TaskCreationTimeColumnName} text not null, {DalTask.TaskDueDateColumnName} text not null, {DalTask.EmailAssigneeColumnName} text not null, " +
                    $"primary key ({DalTask.EmailColumnName}, {DalTask.TaskColumnIDName}, {DalTask.TaskIDColumnName}))";
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
        }

    }
}

