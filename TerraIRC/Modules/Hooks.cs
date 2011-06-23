using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraIRC
{
    public enum Hooks
    {
        USER_CHANNEL_PRIVMSG,
        USER_CHANNEL_NOTICE,
        USER_MESSAGE_CLIENT,
        USER_NOTICE_CLIENT,
        USER_NICKCHANGE,
        USER_IDENTIFY,
        CHANNEL_LOG,
        USER_LOGOUT,
        USER_CONNECT,
        CLIENT_KILLED,
        CLIENT_INTRO,
        SERVER_BURST
    }
}
