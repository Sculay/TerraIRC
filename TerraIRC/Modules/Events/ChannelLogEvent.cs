using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraIRC
{
    public class ChannelLogEvent : Event
    {
        public string message;
        public ChannelLogEvent(string message)
        {
            this.message = message;
        }
    }
}
