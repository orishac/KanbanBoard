using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Task
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly int ID;
        private string title;
        private string body;
        private readonly DateTime creationDate;
        private DateTime dueDate;
        private string emailAssignee;

        private const int MaxTitleLength = 50;
        private const int MaxDescriptionLength = 300;

        public Task(int ID, string title, string body,DateTime dueDate, string emailAssignee)
        {
            if (dueDate.CompareTo(DateTime.Now) <= 0)
            {
                log.Error("An attempt to create a task with illegal DueDate");
                throw new Exception("iilegal due date. due Date must be a future date");
            }
            if (title == null || title.Length > MaxTitleLength || title.Length == 0)
            {
                log.Error("An attempt to create a task with illegal title");
                throw new Exception("illegal Title. title must contain at least 1 character and no more than 50");
            }
            if(body !=null && body.Length > MaxDescriptionLength)
            {
                log.Error("An attemp to create a task with illegal description");
                throw new Exception("illegal description. descreption maximum length is 300 characters");
            }

            this.ID = ID;
            this.title = title;
            this.body = body;
            creationDate = DateTime.Now;
            this.dueDate = dueDate;
            this.emailAssignee = emailAssignee;

        }

        public Task(DataAccessLayer.DalTask dalTask)
        {
            this.ID = dalTask.ID;
            this.title = dalTask.Title;
            this.body = dalTask.Description;
            this.dueDate = dalTask.DueDate;
            this.creationDate = dalTask.CreationTime;
            this.emailAssignee = dalTask.EmailAssignee;
        }

        ///gets a new string represents a new title
        ///check if the new title length is biggee than 0 and smaller than 50, otherwise it throws an exception  (title cannot be empty and it cannot be more than 50 chars)
        ///if the new title is legal, changes this task's title to the given title
        public void SetTitle (string newTitle, String email, int columnID)
        {
            if (newTitle.Length == 0)
            {
                log.Error("An attempt to set illegal title");
                throw new Exception("new title is illegal. the title must contain at least one character");
            }
            else if (newTitle.Length > MaxTitleLength)
            {
                log.Error("An attempt to set illegal title");
                throw new Exception("new title is illegal. you have exceeded the maximum length for the title");
            }
            else
            {

                toDalObject(email, columnID).Title = newTitle;

                title = newTitle;
                log.Info("task " + this.ID + " title was edited");
            }
        }


        ///gets a new string represents a new body
        ///check if the new body length is less then 300, otherwise it throws an exception (there is a limit of 300 chars in the body)
        ///if the new body is legal, changes this task's body to the given body
        public void SetBody (string newBody,string email, int columnID)
        {
            if (newBody != null && newBody.Length > MaxDescriptionLength)
            {
                log.Error("An attempt to set illegal description");
                throw new Exception("new description is illegal. you have exceeded the maximum length for description");
            }
            else
            {

                toDalObject(email, columnID).Description = newBody;

                body = newBody;
                log.Info("task " + this.ID + " description was edited");
            }

        }

        ///gets a new DateTime represents new due date
        ///checks if the new due date is legal - it must be a future date compare to current due date
        ///if the new due date is legal, changes this task's due date to the given due date
        public void SetDueDate (DateTime newDate, string email, int columnID)
        {
            if (newDate.CompareTo(DateTime.Now) <= 0)
            {
                log.Error("An attempt to set illegal DueDate");
                throw new Exception("new due date is illegal. due Date must be a future date");
            }



            toDalObject(email, columnID).DueDate = newDate;
            log.Info("task " + this.ID + " dueDate was edited");

            dueDate = newDate;
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <param name="email">email creator of the board</param>
        public void AssignTask(string newEmailAssignee, string email,int columnID)
        {
            toDalObject(email, columnID).EmailAssignee = newEmailAssignee;
            log.Info("task " + this.ID + " email assignee was edited");

            this.emailAssignee = newEmailAssignee;
        }

        ///makes a new Task in Data access layer with the same ID, title, body, due date and creation date
        ///return the new task
        public DataAccessLayer.DalTask toDalObject(string email, int columnID)
        {
            return new DataAccessLayer.DalTask(email, columnID, ID, title, body,creationDate,dueDate, emailAssignee);
        }

        ///returns this task ID
        public int getId()
        {
            return ID;
        }

        ///returns this task title
        public string getTitle()
        {
            return title;
        }

        ///return this task creation date
        public DateTime getCreationDate()
        {
            return creationDate;
        }

        ///return this task description
        ///
        public string getDescription()
        {
            return body;
        }

        ///return this task due date
        public DateTime getDueDate()
        {
            return dueDate;
        }

        public string getEmailAssignee()
        {
            return emailAssignee;
        }

        /// <summary>
        /// saves the object to DB
        /// </summary>
        /// <param name="email">email of the board creator</param>
        /// <param name="columnID">Column id of the task</param>
        public void save(string email, int columnID)
        {
            toDalObject(email, columnID).save();
        }
    }
}
