using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TerraIRC
{
    public class UserNickChangeEvent : UserEvent
    {
        public User user;
        public string oldnick;
        public string newnick;
        public UserNickChangeEvent(User user, string oldnick, string newnick) : base(user)
        {
            this.user = user;
            this.oldnick = oldnick;
            this.newnick = newnick;
        }
    }
}
