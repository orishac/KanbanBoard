using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct ServiceTask
    {
        public readonly int Id;
        public readonly DateTime CreationTime;
        public readonly DateTime DueDate;
        public readonly string Title;
        public readonly string Description;
        public readonly string emailAssignee;
        internal ServiceTask(int id, DateTime creationTime, DateTime dueDate, string title, string description, string emailAssignee)
        {
            this.Id = id;
            this.CreationTime = creationTime;
            this.DueDate = dueDate;
            this.Title = title;
            this.Description = description;
            this.emailAssignee = emailAssignee;
        }

        internal ServiceTask(BusinessLayer.Task businessTask)
        {
            this.Id = businessTask.getId();
            this.CreationTime = businessTask.getCreationDate();
            this.DueDate = businessTask.getDueDate();
            this.Title = businessTask.getTitle();
            this.Description = businessTask.getDescription();
            this.emailAssignee = businessTask.getEmailAssignee();
        }

        public override string ToString()
        {

            string s = string.Format("Task ID: {0}, Task Title: {1}, Task Description: " +
                "{2}, Task dueDate : {3}, Task CreationTime : {4}, assignee's email : {5}", Id, Title, Description, DueDate.ToString(), CreationTime.ToString(), emailAssignee);
            return s;
        }
    }
}