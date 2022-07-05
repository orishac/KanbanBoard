using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Column
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        internal List<Task> taskList;
        private string columnName; 
        private int columnID;
        private int limit;
        private const int DefaultLimit = 100;
        private const int MaxNameLegth = 15;

   

        public Column (string name, int ID)
        {
            taskList = new List<Task>();

            if (name == null ||name.Length == 0 || name.Length > 15)
            {
                log.Error("An attempt to create a column with illegal name");
                throw new Exception("illegal name. name must contain at least 1 character and no more than 15");
            }
            columnName = name;
            columnID = ID;
            limit = DefaultLimit;
        }

        public Column (DataAccessLayer.DalColumn dalColumn,string email)
        {
            columnName = dalColumn.ColumnName;
            columnID = dalColumn.ColumnID;
            limit = dalColumn.Limit;
            taskList = new List<Task>();
            LoadTask(email) ;
        }

        /// <summary>
        /// check if the given new name is legit and then change the name and save it
        /// </summary>
        /// <param name="email">the email of the board's creaton user for saving ther column</param>
        /// <param name="newName"> the new column name, cannot bew null empty and <15 chars </param>
        public void SetColumnName(string email, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) || newName.Length > MaxNameLegth)
                throw new Exception("column name must contain at least 1 character and no more then 15 chars");
            toDalObject(email).ColumnName = newName;
            columnName = newName;
        }

        /// <summary>
        /// set the ID of the column
        /// </summary>
        /// <param name="newColumnID">the new value</param>
        /// <param name="email">the email of the board creator</param>
        public void SetColumnId(int newColumnID, string email)
        {
            toDalObject(email).ColumnID = newColumnID; 

            foreach (Task t in taskList)
            {
                t.toDalObject(email, columnID).ColumnID = newColumnID;
            }

            columnID = newColumnID;
        }

        public int getColumnId()
        {
            return columnID;
        }

        ///if the given limit number is <= 0, throws an exception (limit must be a positive number)
        ///if the given limit number is legal, set the new limit as the given limit
        public void setLimit(int newLimit,string email)
        {
            if (newLimit <= 0)
            {
                log.Error("An attempt to set illegal limit number to a column");
                throw new Exception("Limit must be a positive number");
            }
            else if (newLimit <= taskList.Count)
            {
                log.Error("An attempt to set lower limit than number of tasks in the column");
                throw new Exception("There are more tasks than the limit");
            }
            else
            {
                toDalObject(email).Limit = newLimit;
                this.limit = newLimit;

                log.Info("column " + this.columnID + " now will contain maximum " + limit + " tasks");
            }
            

        }

        ///return the limit number of this column
        public virtual int getLimit()
        {
            return limit;
        }

        ///return the name of this column
        public string getColumnName()
        {
            return columnName;
        }

        ///checks if the tasks list is at its limit number of tasks
        ///if not, adds the tasks to tasks list
        public void AddTask(Task task, string emailCreator)
        {
            if (taskList.Count == limit)
            {
                log.Error("An attempt to exceed the maximum limit of tasks in the column");
                throw new Exception("You have reached the limit for the tasks in the column ");
            }
            taskList.Add(task);
            log.Debug("A new task has been added");
            task.save(emailCreator, columnID);
        }

        ///using a loop, searches for the task that have the same ID as the given ID
        ///if the correct task is found, calls the SetBody function in Tasks calss
        public void editTaskDescription(string emailAssignee, int id, string newBody,string emailCreator)
        {
            Task toEdit = findTask(id);
            if (toEdit != null)
            {
                if (toEdit.getEmailAssignee().Equals(emailAssignee))
                    toEdit.SetBody(newBody, emailCreator, columnID);

                else
                {
                    log.Error("An attempt of user who is not task asignee to edit task");
                    throw new Exception(string.Format("only {0} can edit task", toEdit.getEmailAssignee()));
                }
            }
            else
            {
                log.Error("At attempt to edit a Task that does not exist");
                throw new Exception("there is no Task with the requsted TaskID");
            }
        }

        ///using a loop, searches for the task that have the same ID as the given ID
        ///if the correct task is found, calls the SetTitle function in Tasks calss
        public void editTaskTitle(string emailAssignee, int id, string newTitle,string emailCreator)
        {
            Task toEdit = findTask(id);
            if (toEdit != null)
            {
                if (toEdit.getEmailAssignee().Equals(emailAssignee))
                    toEdit.SetTitle(newTitle, emailCreator, columnID);
                else
                {
                    log.Error("An attempt of user who is not task asignee to edit task");
                    throw new Exception(string.Format("only {0} can edit task", toEdit.getEmailAssignee()));
                }
            }
            else
            {
                log.Error("At attempt to edit a Task that does not exist");
                throw new Exception("there is no Task with the requsted TaskID");
            }
        }

        ///using a loop, searches for the task that have the same ID as the given ID
        ///if the correct task is found, calls the SetDueDate function in Tasks calss
        public void editTaskDueDate(string emailAssignee, int id, DateTime newDate, string emailCreator)
        {
            Task toEdit = findTask(id);
            if (toEdit != null)
            {
                if(toEdit.getEmailAssignee().Equals(emailAssignee))
                   toEdit.SetDueDate(newDate, emailCreator, columnID);
                else
                {
                    log.Error("An attempt of user who is not task asignee to edit task");
                    throw new Exception(string.Format("only {0} can edit task", toEdit.getEmailAssignee()));
                }
            }
            else
            {
                log.Error("At attempt to edit a Task that does not exist");
                throw new Exception("there is no Task with the requsted TaskID");
            }
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <param name="email">email creator of the board</param>
        public void AssignTask(int taskId, string newEmailAssignee, string email, string emailCreator)
        {
            Task toAssign = findTask(taskId);
            if (toAssign != null)
            {
                if (toAssign.getEmailAssignee() == email)
                    toAssign.AssignTask(newEmailAssignee, emailCreator, columnID);

                else
                {
                    log.Error("An attempt of user who is not task asignee to asign task");
                    throw new Exception(string.Format("only {0} can assign task to {1}", toAssign.getEmailAssignee(), newEmailAssignee));
                }
            }
            else
            {
                log.Error("At attempt to edit a Task that does not exist");
                throw new Exception("there is no Task with the requsted TaskID");
            }
        }


        ///using the remove function in "List" class, removes the given task
        public void DeleteTask (string emailassignee, int taskId, string emailCreator)
        {
            Task toRemove = findTask(taskId);
            if (toRemove != null)
            {
                if (toRemove.getEmailAssignee().Equals(emailassignee))
                {
                    taskList.Remove(toRemove);
                    toRemove.toDalObject(emailCreator, columnID).Delete();
                }
                else
                {
                    log.Error("An attempt of user who is not task asignee to edit task");
                    throw new Exception(string.Format("only {0} can edit task", toRemove.getEmailAssignee()));
                }
            }
            else
            {
                log.Error("At attempt to delete a Task that does not exist");
                throw new Exception("there is no Task with the requsted TaskID");
            }
        }

        ///Makes a new List of tasks of type Data Access Layer
        ///using a loop, goes over every task in the tasks list, call toDalObject function in Task class in Data access layer
        ///and add each "new" task to the new Data Access Layer tasks list
        ///return the new list of tasks 
        public DataAccessLayer.DalColumn toDalObject(string email)
        {
            return new DataAccessLayer.DalColumn(email, columnID, columnName, limit);
        }

        ///using a loop, goes over every task in the task list
        ///if it finds a task with the same ID as the given ID, return it;
        ///otherwise, throws an exception that this task does not exist
        public Task findTask(int taskId)
        {
            Task toReturn =  taskList.Find(t => t.getId()== taskId);

            if (toReturn == null)
            {
                log.Error("An attempt to access a task that does not exist");
                throw new Exception("the task requested does not exist");
            }

            return toReturn;
        }

        /// <summary>
        /// returns the tasks as a read-only collection
        /// </summary>
        /// <returns> read-only collection of the column tasks</returns>
        public IReadOnlyCollection<Task> getTasksCollection()
        {

            IReadOnlyCollection<Task> Tasks = taskList;

            return Tasks;
        }

        /// <summary>
        /// build the tasklist from dalobject
        /// </summary>
        /// <param name="email"> the email of the user's board that the column's is belong to </param>
        private void LoadTask(string email)
        {
            DataAccessLayer.DalTask dalTask = new DataAccessLayer.DalTask();
            List<DataAccessLayer.DalTask> dalTasks = dalTask.GetTasks(email, this.columnID);
            foreach( DataAccessLayer.DalTask task in dalTasks)
            {
                this.taskList.Add(new Task(task));
            }
            
        }

        /// <summary>
        /// saves to object to the DB
        /// </summary>
        /// <param name="email">email of the board creator</param>
        public void save(string email)
        {
            toDalObject(email).save();
        }

    }
}
