using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TerraIRC
{
    public class KillEvent : Event
    {
        public User killer;
        public User killee;
        public string reason;
        public KillEvent(User killer, User killee, string reason)
        {
            this.killer = killer;
            this.killee = killee;
            this.reason = reason;
        }
    }
}
