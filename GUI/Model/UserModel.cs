using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace GUI.Model
{
    public class UserModel : NotifiableModelObject
    {
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                
            }
        }

        private string _nickname;

        public string Nickname
        {
            get => _nickname;
            set
            {

            }
        }

        private string _belongingBoard;
        public string BelongingBoard
        {
            get => _belongingBoard;
            set
            {

            }
        }

        public BoardModel Board { get; set; }
 

        public UserModel(BackendController controller, SerivceUser user): base(controller)
        {
            _email = user.Email;
            _nickname = user.Nickname;
            _belongingBoard = user.BelongingBoard;
            Board = controller.GetBoard(BelongingBoard, Email);
        }
    }
}

