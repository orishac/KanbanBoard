using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Board 
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private  string emailCreator;
        private List<string> co_users;
        private List<Column> columns;
        private int idGenerator;

        public Board(string email)
        {
            this.emailCreator = email;
            idGenerator = 1;
            columns = new List<Column>
            {
                new Column("backlog", 0),
                new Column("in progress", 1),
                new Column("done", 2)
            };

            columns[0].save(emailCreator);
            columns[1].save(emailCreator);
            columns[2].save(emailCreator);

            co_users = new List<string>();
        }

        public Board(DataAccessLayer.DalBoard dalBoard)
        {
            this.emailCreator = dalBoard.EmailCreator;
            this.idGenerator = dalBoard.IDGenerator;
            columns = new List<Column>();
            BuildColumns();
            DataAccessLayer.DalUser u = new DataAccessLayer.DalUser();
            co_users = u.GetBoardCoUsers(emailCreator);

        }

        /// <summary>
        /// adds new user to the board users.
        /// </summary>
        /// <param name="email">email of the new user</param>
        public void addNewUser(string email)
        {
            co_users.Add(email);
        }

        /// <summary>
        /// checks if the column is not a read-only column
        /// </summary>
        /// <param name="columnId">The column ID</param>
        /// <returns>boolean value indicates whether the column can be edited or not</returns>
        private bool CanEditColumn(int columnId)
        {
            if (columnId == columns.Count - 1)
            {
                log.Error("An attempt to edit read-only column");
                return false;
            }
            return true;

        }

        /// <summary>
        /// checks if the column exists
        /// </summary>
        /// <param name="columnId">The column ID</param>
        /// <returns>boolean value indicates whether the column exists or not</returns>
        private bool ValidateColumnOrdinal(int columnId)
        {
            if (columnId > columns.Count - 1 || columnId < 0)
            {
                log.Error("An attempt to delete a column that does not exist");
                return false;
            }
            return true;
        }

        /// <summary>
        /// build the columns list from DataAccessLayser
        /// </summary>
        private void BuildColumns()
        {
            List<DataAccessLayer.DalColumn> dalColumns = new DataAccessLayer.DalColumn().GetColumns(emailCreator);
            foreach (DataAccessLayer.DalColumn dalColumn in dalColumns)
            {
                this.columns.Add(new Column(dalColumn, emailCreator));
            }
        }

        /// <summary>
        /// removes requsted column from board if possible, and transfers all of its tasks to the adjacent column
        /// </summary>
        /// <param name="id">the id of the column</param>
        public void RemoveColumn(string email,int id)
        {
            if (!emailCreator.Equals(email))
                throw new Exception("not the email of the board creator");

            if (columns.Count == 2)
            {
                log.Error("An attempt to remove column when there are only 2 columns left");
                throw new Exception("Can't remove column, board must contain at least 2 columns");
            }
            if (!ValidateColumnOrdinal(id))
            {
                throw new Exception("the requested column to remove does not exist");
            }
            else if (id != 0)
            {
                if (columns[id - 1].taskList.Count + columns[id].taskList.Count > columns[id - 1].getLimit())
                {
                    log.Error("An attempt to delete a column when the adjacent column will exceed its maximum limit");
                    throw new Exception("the requested column cannot be deleted,the adjacent column will exceed its maximum limit");
                }
                columns[id - 1].taskList.AddRange(columns[id].taskList);
                foreach (Task t in columns[id].taskList)
                {
                    t.toDalObject(emailCreator, id).ID = id - 1;
                }
            }
            else
            {
                if (columns[id + 1].taskList.Count + columns[id].taskList.Count > columns[id + 1].getLimit())
                {
                    log.Error("An attempt to delete a column when the adjacent column will exceed its maximum limit");
                    throw new Exception("the requested column to remove cannot be deleted,the adjacent column will exceed its maximum limit ");
                }
                columns[id + 1].taskList.AddRange(columns[id].taskList);
                foreach (Task t in columns[id].taskList)
                {
                    t.toDalObject(emailCreator, id).ID = id + 1;
                }
            }

            columns[id].toDalObject(emailCreator).Delete();
            columns.RemoveAt(id);


            for (int i = id; i < columns.Count; i++)
            {
                columns[i].SetColumnId(i - 1, emailCreator);
            }


            log.Debug("column " + id + " has been removed");
        }

        /// <summary>
        /// add new column to the board.
        /// </summary>
        /// <param name="columnId">columnID of the new column - determins its location in the board</param>
        /// <param name="columnName">name of the new column</param>
        /// <returns>the added column</returns>
        public Column AddColumn(string email,int columnId, string columnName)
        {
            if (!emailCreator.Equals(email))
                throw new Exception("not the email of the board creator");

            if (string.IsNullOrWhiteSpace(columnName) || columnName.Length == 0 || columnName.Length > 15)
            {
                log.Error("An attempt to create column with illegal name");
                throw new Exception("Column name must contain at least one letter");
            }

            foreach (Column c in columns)
            {
                if (c.getColumnName().Equals(columnName))
                {
                    log.Error("An attempt to create new column with exsiting name");
                    throw new Exception("Column name requested already exists");
                }
            }

            if (columnId > columns.Count)
            {
                log.Error("An attempt to create column with illegal id");
                throw new Exception("Column id is illegal");
            }

            Column toAdd = new Column(columnName, columnId);

            columns.Insert(columnId, toAdd);

            for (int i = columns.Count - 1; i > columnId; i--)
            {
                columns[i].SetColumnId(i, emailCreator);
            }


            toAdd.save(emailCreator);
            log.Debug("column " + columnId + " has been added");
            return toAdd;
        }

        ///sets a new limit for a specific column using the column ID
        ///searches for the column with the given column ID and calls for the setLimit function in Column with the given new limit;
        public void SetLimit(string email, int limit, int columnId)
        {
            if (!emailCreator.Equals(email))
                throw new Exception("not the email of the board creator");

            if (!ValidateColumnOrdinal(columnId))
                throw new Exception("the requested column to remove does not exist");

            columns[columnId].setLimit(limit, emailCreator);
        }

        ///gets a title, description and dueDate for a new Task
        ///makes a new Task and gives it a generetedID and adds it to column[0] by default;
        ///calls for the save function in order to save the changes that was made
        ///return the new Task that was created
        public Task AddTask(string taskCreator, string title, string description, DateTime dueDate)
        {
            Task task = new Task(idGenerator, title, description, dueDate, taskCreator);
            idGenerator++;
            columns[0].AddTask(task, emailCreator);
            toDalObject().IDGenerator = idGenerator;
            return task;
        }

        ///if the given column ID is smaller than 1 this function throws an exception (the columns ID is >=0)
        ///throws an exception if the user tries to edit a task that in column[2] (this column's tasks cannot be edited)
        ///searches in the columns list the column with the given ID and calls for the editTaskDescription in Column
        ///calls for the save function in order to save the changes that was made
        public void editTaskDescription(string emailAssignee, int columnId, int taskId, string newBody)
        {

            if (!ValidateColumnOrdinal(columnId))
            {
                throw new Exception("the requested column does not exist");
            }
            else if (!CanEditColumn(columnId))
            {
                throw new Exception("tasks in 'done' column cannot be edited");
            }
            else
            {
                columns[columnId].editTaskDescription(emailAssignee, taskId, newBody, emailCreator);
            }
        }

        ///if the given column ID is smaller than 1 this function throws an exception (the columns ID is >=0)
        ///throws an exception if the user tries to edit a task that in column[2] (this column's tasks cannot be edited)
        ///searches in the columns list the column with the given ID and calls for the editTaskTitle in Column
        ///calls for the save function in order to save the changes that was made
        public void editTaskTitle(string emailAssignee, int columnId, int taskId, string newTitle)
        {
            if (!ValidateColumnOrdinal(columnId))
            {
                throw new Exception("the requested column does not exist");
            }
            else if (!CanEditColumn(columnId))
            {
                throw new Exception("tasks in 'done' column cannot be edited");
            }
            else
            {
                columns[columnId].editTaskTitle(emailAssignee, taskId, newTitle, emailCreator);
            }
        }

        ///if the given column ID is smaller than 1 this function throws an exception (the columns ID is >=0)
        ///throws an exception if the user tries to edit a task that in column[2] (this column's tasks cannot be edited)
        ///searches in the columns list the column with the given ID and calls for the editTaskDueDate in Column
        ///calls for the save function in order to save the changes that was made
        public void editTaskDueDate(string emailAssignee, int columnId, int taskId, DateTime newDate)
        {
            if (!ValidateColumnOrdinal(columnId))
            {
                throw new Exception("the requested column does not exist");
            }
            else if (!CanEditColumn(columnId))
            {
                throw new Exception("tasks in 'done' column cannot be edited");
            }
            else
            {
                columns[columnId].editTaskDueDate(emailAssignee, taskId, newDate, emailCreator);
            }

        }

        /// <summary>
        /// deleting an existed task in the board
        /// </summary>
        /// <param name="emailAssignee">the email of the task's assignee - only he allowed to preform action on the task </param>
        /// <param name="columnOrdinal">column id of the task</param>
        /// <param name="taskId">the id of the task</param>
        public void DeleteTask(string emailAssignee, int columnOrdinal, int taskId)
        {
            if (!ValidateColumnOrdinal(columnOrdinal))
            {
                throw new Exception("the requested column does not exist");
            }
            else if (!CanEditColumn(columnOrdinal))
            {
                throw new Exception("tasks in 'done' column cannot be edited");
            }
            else
                columns[columnOrdinal].DeleteTask(emailAssignee, taskId, emailCreator);

        }

        ///if the given column ID is smaller than 1 this function throws an exception (the columns ID is >=0)
        ///throws an exception if the user tries to advance a task that in column[2] (there is not another column after this column)
        ///searches in the columns list the column with the given ID and calls for the editTaskDescription in Column
        ///makes a copy of the task that the user wants to advance, delete the original task from the current column and add the copy to the next column
        ///calls for the save function in order to save the changes that was made
        public void advanceTask(string emailAssignee, int columnId, int taskId)
        {

            if (!ValidateColumnOrdinal(columnId))
            {
                throw new Exception("the requested column does not exist");
            }
            else if (!CanEditColumn(columnId))
            {
                throw new Exception("tasks in 'done' column cannot be edited");
            }
            else
            {

                Task toAdvance = columns[columnId].findTask(taskId);
                columns[columnId].DeleteTask(emailAssignee, taskId, emailCreator);
                columns[columnId + 1].AddTask(toAdvance, emailCreator);

                log.Info("task " + taskId + "  was advanced to column " + (columnId + 1));
            }
        }

        ///if the given column ID is smaller than 1 this function throws an exception (the columns ID is >=0)
        ///returns the column with the guven ID
        public Column getColumnByID(int columnId)
        {
            if (!ValidateColumnOrdinal(columnId))
                throw new Exception("the requested column to remove does not exist");

            return columns[columnId];

        }

        ///using a loop, goes over every column in the columns list
        ///if it finds a column that has the same name as the given mame, return the column
        public Column getColumnByName(string columnName)
        {
            foreach (Column column in columns)
            {
                if (column.getColumnName().Equals(columnName))
                {
                    return column;
                }
            }
            log.Error("An attempt to access a column that does not exist");
            throw new Exception("the requested column does not exist");
        }

        ///makes a new list of Data Access Layer columns
        ///using a loop that goes over every column in this.columns list, adds a Dal Object column to the list of Data Access Layer columns,
        ///using toDalObject function in DataAccessLayer.column class
        ///makes a new Data Access Layer Board using the new list of DataAccessLayer.Column that was made, the user's email and the idGenerator;
        public DataAccessLayer.DalBoard toDalObject()
        {
            return new DataAccessLayer.DalBoard(emailCreator, idGenerator);
        }

        ///makes a new Data Access Layer board using the toDalObject function
        ///saves the changes that was made calling the save function in the Data Access Layer board
        public void save()
        {
            DataAccessLayer.DalBoard dalBoard = this.toDalObject();
            dalBoard.save();
        }

        /// <summary>
        /// aid method - board object in service layer contains the names of the columns
        /// </summary>
        /// <returns>list of the column names in columns list</returns>
        public IReadOnlyCollection<string> getColumnsNamesCollection()
        {
            List<string> columnsNames = new List<string>();
            foreach (Column c in columns)
            {
                columnsNames.Add(c.getColumnName());
            }
            IReadOnlyCollection<string> cNames = columnsNames;

            return cNames;
        }

        /// <summary>
        /// moves the requsted column left in the board, changes its location in the list.
        /// </summary>
        /// <param name="ColumnID"> id of the requsted column to move</param>
        /// <returns>the column that we moved</returns>
        public Column MoveLeft(string email, int ColumnID)
        {
            if (!emailCreator.Equals(email))
                throw new Exception("not the email of the board creator");

            if (ColumnID == 0)
            {
                log.Error("Am attempt to shift left the first column");
                throw new Exception("Can't shift first Column left");
            }

            Column column = columns[ColumnID];
            Column predeccesor = columns[ColumnID - 1];


            columns[ColumnID] = predeccesor;

            columns[ColumnID - 1] = column;



             columns[ColumnID].SetColumnId(-1, emailCreator); // demi value update, beacuse the column id is unique
             columns[ColumnID - 1].SetColumnId(ColumnID - 1, emailCreator);
             columns[ColumnID].SetColumnId(ColumnID, emailCreator);

            log.Debug("column " + ColumnID + " has moved left");
            return column;

        }

        /// <summary>
        /// moves the requsted column right in the board, changes its location in the list.
        /// </summary>
        /// <param name="ColumnID"> id of the requsted column to move</param>
        /// <returns>the column that we moved</returns>
        public Column MoveRight(string email, int ColumnID)
        {
            if (!emailCreator.Equals(email))
                throw new Exception("not the email of the board creator");

            if (ColumnID >= columns.Count - 1)
            {
                log.Error("Am attempt to shift right the last column");
                throw new Exception("Can't shift last Column right");
            }
            Column column = columns[ColumnID];
            Column successor = columns[ColumnID + 1];

            columns[ColumnID] = successor;
            columns[ColumnID + 1] = column;

            columns[ColumnID].SetColumnId(-1, emailCreator); // demi value update, beacuse the column id is unique

            column.SetColumnId(ColumnID + 1, emailCreator);
            successor.SetColumnId(ColumnID, emailCreator);
            log.Debug("column " + ColumnID + " has moved right");
            return column;

        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="ColumnID">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        public void AssignTask(string email, int ColumnID, int taskId, string emailAssignee)
        {
            if (!co_users.Contains(emailAssignee))
            {
                log.Error("trying to set task assignee to user that doen't belong the board");
                throw new Exception("the email of the new assignee is not belonging to this board");
           
            }
                
            if (!ValidateColumnOrdinal(ColumnID))
            {
                throw new Exception("the requested column does not exist");
                
            }
            else if (!CanEditColumn(ColumnID))
            {
                throw new Exception("tasks in 'done' column cannot be edited");
            }

            columns[ColumnID].AssignTask(taskId, emailAssignee, email, emailCreator);
        }

        /// <summary>
        /// check if there is already column with the exact name given else try to change it's name
        /// </summary>
        /// <param name="email"> the email that belong to the creator's of the board</param>
        /// <param name="columnOrdinal"> the column that the user want to change the name of</param>
        /// <param name="newName">the new name of the column</param>
        public void ChangeColumnName(string email, int columnOrdinal, string newName)
        {
            if (!ValidateColumnOrdinal(columnOrdinal))
                throw new Exception("the requested column does not exist");
            if (!emailCreator.Equals(email))
                throw new Exception("not the email of the board creator");
            if (columns.Any(c => c.getColumnName().Equals(newName)))
                throw new Exception("column name suppose to be unique");

            columns[columnOrdinal].SetColumnName(email, newName);

        }

        /// <summary>
        /// returns the board's users
        /// </summary>
        /// <returns>list of the board's users (their emails)</returns>
        public List<string> GetUsersList()
        {
            return co_users;
        }

        public List<Column> GetColumns()
        {
            return columns;
        }

        public string GetEmailCreator()
        {
            return emailCreator;
        }
    }
}
