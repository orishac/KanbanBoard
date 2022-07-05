using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DalColumn : DalObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ColumnController controller;
        public const string EmailColumnName = "Email";
        public const string ColumnColumnID = "ColumnID";
        public const string ColumnColumnName = "ColumnName";
        public const string ColumnColumnLimit = "ColumnLimit";

        private string _Email;
        public string Email
        {
            get => _Email;

        }

        private int _ColumnID;
        public virtual int ColumnID
        {
            get => _ColumnID;
            set
            {;
                controller.UpdateID(Email, ColumnName, ColumnColumnID, value);
                _ColumnID = value;
                log.Debug(string.Format("column {0} updated column name {1} to parameter {2}", ColumnName, ColumnColumnID, value));

            }
        }

        private string _ColumnName;
        public string ColumnName
        {
            get => _ColumnName;
            set
            {
                controller.UpdateName(Email, ColumnID, ColumnColumnName, value);
                _ColumnName = value;
                log.Debug(string.Format("column {0} updated {1} to parameter {2}", ColumnID, ColumnColumnName, value));
            }
        }


        private int _Limit;
        public int Limit
        {
            get => _Limit;
            set
            {
                controller.UpdateLimit(Email, ColumnID,ColumnColumnLimit, value);
                _Limit = value;
                log.Debug(string.Format("column {0} updated column name {1} to parameter {2}", ColumnID, ColumnColumnLimit, value));
            }
        }



        public DalColumn(string email, int columnId, string name, int limit)
        {
            _Email = email;
            _ColumnID = columnId;
            _ColumnName = name;
            _Limit = limit;
            controller = new ColumnController();
        }

        public DalColumn() 
        {
            controller = new ColumnController();
        }


        /// <summary>
        /// load columns of specific board from the DB
        /// </summary>
        /// <returns> list of the columns</returns>
        public List<DalColumn> GetColumns(string email)
        {
            return controller.Select(email);
        }

        /// <summary>
        /// saves to object by inserting it to the table
        /// </summary>
        public void save()
        {
           if (!controller.Insert(this))
            {
                log.Error("insert query did not succeed");
                throw new Exception("insert query did not succeed");
            }
        }

        /// <summary>
        /// delete all the records of the object from the table
        /// </summary>
        public void RemoveData()
        {
            if (!controller.DeleteTable())
            {
                log.Error("deleteTable query did not succeed");
                throw new Exception("deleteTable query did not succeed");
            }
        }

        /// <summary>
        /// delete record of the object from the DB
        /// </summary>
        public void Delete()
        {
            if (!controller.Delete(this))
            {
                log.Error("delete query did not succeed");
                throw new Exception("delete query did not succeed");
            }
        }


    }


}
