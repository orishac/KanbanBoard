using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI.ViewModel;

namespace GUI.Model
{
    public class NotifiableModelObject : NotifiableObjects
    {
        public BackendController Controller { get; private set; }
        protected NotifiableModelObject(BackendController controller)
        {
            this.Controller = controller;
        }
    }
}
