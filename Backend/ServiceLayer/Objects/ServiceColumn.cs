using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct ServiceColumn
    {
        public readonly IReadOnlyCollection<ServiceTask> Tasks;
        public readonly string Name;
        public readonly int Limit;
        public readonly int ColumnID;
        internal ServiceColumn(IReadOnlyCollection<ServiceTask> tasks, string name, int limit, int columnID)
        {
            this.Tasks = tasks;
            this.Name = name;
            this.Limit = limit;
            this.ColumnID = columnID;
        }
        
        public override string ToString()
        {
            string s = "Column Name: " + Name + ", Column Limit: " + Limit + " ,Column Tasks: ";
            foreach (ServiceTask t in Tasks)
            {
                s += "\n" +t.ToString() ;
            }
            return s;
        }
    }
}
