using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct SerivceUser
    {
        public readonly string Email;
        public readonly string Nickname;
        public readonly string BelongingBoard;
        internal SerivceUser(string email, string nickname, string BelongingBoard)
        {
            this.Email = email;
            this.Nickname = nickname;
            this.BelongingBoard = BelongingBoard;
        }
        internal SerivceUser(BusinessLayer.User user)
        {
            this.Email = user.getEmail();
            this.Nickname = user.getNickName();
            this.BelongingBoard = user.GetBelongingBoard();
        }

    }
}
