using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class UserController : DalController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserController() : base("Users")
        {

        }

        /// <summary>
        /// converts the reader output to user dal object
        /// </summary>
        /// <param name="reader">reader of the DB, returns the records</param>
        /// <returns>the converted object</returns>
        protected override DalObject  Convert(SQLiteDataReader reader)
        {
            return new DalUser(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.IsDBNull(3) ? null : reader.GetString(3));
        }

        /// <summary>
        /// returns all the records of users from Users table
        /// </summary>
        /// <returns> list of users </returns>
        public List<DalUser> selectUsers()
        {
            List<DalUser> users = base.Select().Cast<DalUser>().ToList();
            log.Debug("all users has been selected from the DB successfuly");
            return users;
        }

        /// <summary>
        /// inserts new record of user to Users table
        /// </summary>
        /// <param name="user">object to insert</param>
        /// /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool Insert(DalUser user)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({DalUser.EmailColumnName},{DalUser.PasswordColumnName},{DalUser.NicknameColumnName},{DalUser.BoardsIdentifingEmail})" +
                        $"VALUES (@EmailVal, @PasswordVal, @NicknameVal, @BoardEmailVal);";
                   
                    command.Parameters.Add(new SQLiteParameter("EmailVal", user.Email));
                    command.Parameters.Add(new SQLiteParameter("PasswordVal", user.Password));
                    command.Parameters.Add(new SQLiteParameter("NicknameVal", user.Nickname));
                    command.Parameters.Add(new SQLiteParameter("BoardEmailVal", user.UsersBoard));
                    command.Prepare();

                    res = command.ExecuteNonQuery();
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
               
                return res > 0;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email">email of the user - primary key</param>
        /// <param name="parameter">the new value of the attribute, string type value</param>
        /// <param name="ColumnName">column name to update</param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool Update(string email, string parameter, string ColumnName)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"update {_tableName} set [{ColumnName}] = @ParameterVal where {DalUser.EmailColumnName} = @EmailVal";
                command.Parameters.Add(new SQLiteParameter("ParameterVal", parameter));
                command.Parameters.Add(new SQLiteParameter("EmailVal",email ));
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    log.Error(e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            
            return res > 0;
        }


        public List<String> SelectCoUsers(string BoardsEmail)
        {
            List<string> CoUsers = new List<string>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} where [{DalUser.BoardsIdentifingEmail}] = @emailPar";
                command.Parameters.Add(new SQLiteParameter("emailPar", BoardsEmail));

                SQLiteDataReader reader = null;

                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        CoUsers.Add(reader.GetString(0));
                    }

                }
                catch(Exception e)
                {
                    log.Error(e.Message);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    command.Dispose();
                    connection.Close();
                }
            }
            

            return CoUsers;
        }
    }

}
