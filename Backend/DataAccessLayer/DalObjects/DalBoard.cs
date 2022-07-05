
using System;
using System.Collections.Generic;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
     public class DalBoard : DalObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string EmailColumnName = "Email";
        public const string IdGeneratorColumnName = "IDgenerator";

        private string _EmailCreator;
        public virtual string EmailCreator
        {
            get => _EmailCreator;
        }

        private int _IDGenerator;
        public int IDGenerator
        {
            get => _IDGenerator;
            set
            {
                _controller.Update(_EmailCreator, value, IdGeneratorColumnName);
                _IDGenerator = value;
                log.Debug(string.Format("board {0} updated column name {1} to parameter {2}", _EmailCreator, IdGeneratorColumnName, value));
            }
        }

        private BoardController _controller;

        public DalBoard()
        {
            _controller = new BoardController();
        }

        public DalBoard (string emailCreator, int idGenerator)
        {
            _controller = new BoardController();
            _EmailCreator = emailCreator;
            _IDGenerator = idGenerator;
        }

        /// <summary>
        /// load all the boards
        /// </summary>
        /// <returns> list of the boards</returns>
        public List<DalBoard> LoadData()
        {
            return _controller.SelectBoard();
        }

        /// <summary>
        /// saves to object by inserting it to the table
        /// </summary>
        public void save()
        {
            if (!_controller.Insert(this))
            {
                log.Error("insert query did not succeed");
                throw new Exception("insert query did not succeed");
            }
            log.Debug(string.Format("board {0} has been added successuly to the table", EmailCreator));
        }

        /// <summary>
        /// delete all the records of the object from the table
        /// </summary>
        public void RemoveData()
        {
            if (!_controller.DeleteTable())
            {
                log.Error("deleteTable query did not succeed");
                throw new Exception("deleteTable query did not succeed");
            }
            log.Debug("table 'Boards' has been deleted successfuly from the DB");
        }
    }
}
