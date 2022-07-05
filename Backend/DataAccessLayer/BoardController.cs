using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class BoardController : DalController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string BoardsTableName = "Boards";
        public BoardController() : base(BoardsTableName)
        {
            
        }

        /// <summary>
        /// converts the reader output to board dal object
        /// </summary>
        /// <param name="reader">reader of the DB, returns the records</param>
        /// <returns>the converted object</returns>
        protected override DalObject Convert(SQLiteDataReader reader)
        {
            return new DalBoard(reader.GetString(0), reader.GetInt32(1));
        }

        /// <summary>
        /// returns all the records of boards from Boards table
        /// </summary>
        /// <returns> list of boards </returns>
        public List<DalBoard> SelectBoard()
        {
            List<DalBoard> result = Select().Cast<DalBoard>().ToList();
            log.Debug("all boards has been selected from the DB successfuly");
            return result;
        }

        /// <summary>
        /// inserts new record of board to Boards table
        /// </summary>
        /// <param name="board">object to insert</param>
        /// /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool Insert (DalBoard board)
        {
            using( var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                int res = -1;
                command.CommandText = $"INSERT INTO {BoardsTableName} ({DalBoard.EmailColumnName} , {DalBoard.IdGeneratorColumnName}) VALUES (@emailVal,@IDgeneratorVal)";
                command.Parameters.Add(new SQLiteParameter(@"emailVal", board.EmailCreator));
                command.Parameters.Add(new SQLiteParameter(@"IDgeneratorVal", board.IDGenerator));
                try
                {
                    connection.Open();
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

 
        /// </summary>
        /// <param name="email">email of the board - primary key</param>
        /// <param name="parameter">the new value of the attribute, int type value</param>
        /// <param name="ColoumnName">column name to update</param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool Update(string email, int parameter, string ColumnName)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"update {_tableName} set [{ColumnName}] = @ParameterVal where {DalUser.EmailColumnName} = @EmailVal";
                command.Parameters.Add(new SQLiteParameter("ParameterVal", parameter));
                command.Parameters.Add(new SQLiteParameter("EmailVal", email));
                try
                {
                    connection.Open();
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

    }
}
