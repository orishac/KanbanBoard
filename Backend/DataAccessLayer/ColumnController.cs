using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class ColumnController : DalController
    {
        private const string ColumnTableName = "Columns";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ColumnController() : base(ColumnTableName)
        {

        }

        /// <summary>
        /// converts the reader output to column dal object
        /// </summary>
        /// <param name="reader">reader of the DB, returns the records</param>
        /// <returns>the converted object</returns>
        protected override DalObject Convert(SQLiteDataReader reader)
        {
            DalColumn result = new DalColumn(reader.GetString(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3));
            return result;
        }

        /// <summary>
        /// selects from the DB all the columns from specified board
        /// </summary>
        /// <param name="email"> one of the keys of column </param>
        /// <returns></returns>
        public List<DalColumn> Select(string email)
        {
            List<DalColumn> columnList = new List<DalColumn>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select* from {_tableName} where [{DalColumn.EmailColumnName}] = @EmailVal";
                command.Parameters.Add(new SQLiteParameter("EmailVal", email));

                SQLiteDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        columnList.Add(new DalColumn(email, reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3)));
                    }
                }
                finally
                {
                    if (reader == null)
                        reader.Close();
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Debug("The columns from board " + email + " has been selected successfuly");
            return columnList;
        }


        /// <summary>
        ///  inserts new record of column to Tasks table
        /// </summary>
        /// <param name="column">object to insert</param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool Insert(DalColumn column)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {ColumnTableName} ({DalColumn.EmailColumnName},{DalColumn.ColumnColumnID},{DalColumn.ColumnColumnName}, {DalColumn.ColumnColumnLimit}) " +
                        $"VALUES (@emailVal,@idVal,@nameVal,@limitVal);";

                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", column.Email);
                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", column.ColumnID);
                    SQLiteParameter nameParam = new SQLiteParameter(@"nameVal", column.ColumnName);
                    SQLiteParameter limitParam = new SQLiteParameter(@"limitVal", column.Limit);

                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(idParam);
                    command.Parameters.Add(nameParam);
                    command.Parameters.Add(limitParam);
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
                log.Debug("column " + column.ColumnID + " has been inserted to Columns table successfuly");
                return res > 0;
            }
        }

        /// <summary>
        /// updates a record's Name in Columns table
        /// </summary>
        /// <param name="email">The email of the board contains the column</param>
        /// <param name="columnID">column ID,key of the column</param>
        /// <param name="columnName">column name to update</param>
        /// <param name="parameter">the new value of the attribute, string type value</param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool UpdateName (string email, int columnID, string columnName, string parameter)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"UPDATE {ColumnTableName} set [{columnName}] = @paramVal WHERE {DalColumn.EmailColumnName} = @emailParam and {DalColumn.ColumnColumnID} = @columnIDParam";

                    SQLiteParameter paramVal = new SQLiteParameter(@"paramVal", parameter);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailParam", email);
                    SQLiteParameter columnIDParam = new SQLiteParameter(@"columnIDParam", columnID);

                    command.Parameters.Add(paramVal);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(columnIDParam);

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
        /// updates a record's Limit in Columns table
        /// </summary>
        /// <param name="email">The email of the board contains the column</param>
        /// <param name="columnID">column ID,key of the column</param>
        /// <param name="columnName">column name to update</param>
        /// <param name="parameter">the new value of the attribute, string type value</param>
        /// <returns></returns>
        public bool UpdateLimit(string email, int columnID, string columnName,  int parameter)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"UPDATE {ColumnTableName} set [{columnName}] = @paramVal WHERE {DalColumn.EmailColumnName} = @emailParam and {DalColumn.ColumnColumnID} = @columnIDParam";

                    SQLiteParameter paramVal = new SQLiteParameter(@"paramVal", parameter);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailParam", email);
                    SQLiteParameter columnIDParam = new SQLiteParameter(@"columnIDParam", columnID);

                    command.Parameters.Add(paramVal);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(columnIDParam);

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
        /// updates a record's ID in Columns table
        /// </summary>
        /// <param name="email">The email of the board contains the column</param>
        /// <param name="columnID">column ID,key of the column</param>
        /// <param name="columnName">column name to update</param>
        /// <param name="parameter">the new value of the attribute, string type value</param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool UpdateID(string email, string columnName, string DBcolumnName, int parameter)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"UPDATE {ColumnTableName} set [{DBcolumnName}] = @paramVal WHERE {DalColumn.EmailColumnName} = @emailParam and {DalColumn.ColumnColumnName} = @columnNameParam";

                    SQLiteParameter paramVal = new SQLiteParameter(@"paramVal", parameter);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailParam", email);
                    SQLiteParameter columnIDParam = new SQLiteParameter(@"columnNameParam", columnName);

                    command.Parameters.Add(paramVal);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(columnIDParam);

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
        /// delete record of column from Columns table
        /// </summary>
        /// <param name="column">object to delete</param>
        /// <returns>boolean value indicates whether the method succeeded</returns>
        public bool Delete(DalColumn column)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where [{DalColumn.EmailColumnName}]= @emailVal and [{DalColumn.ColumnColumnID}] = @ColumnID"
                };
                command.Parameters.Add(new SQLiteParameter("emailVal", column.Email));
                command.Parameters.Add(new SQLiteParameter("ColumnID", column.ColumnID));
                try
                {
                    connection.Open();
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                } catch (Exception e)
                {
                    log.Error(e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            log.Debug(string.Format("column {0} has been deleted", column.ColumnID));
            return res > 0;
        }
    }
}
