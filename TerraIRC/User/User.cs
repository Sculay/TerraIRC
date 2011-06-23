using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraIRC
{
    public class User
    {
        public string nickname;
        public string username;
        public string hostname;
        public string gecos;
        public string UID;
        public string modes = "";
        public bool isIntroduced = false;
        public bool loggedIn = false;
        public User(string nickname, string username, string modes, string hostname, string gecos, string UID, bool introduced = false)
        {
            this.nickname = nickname;
            this.username = username;
            this.hostname = hostname;
            this.gecos = gecos;
            this.modes = modes;
            this.UID = UID;
            Console.WriteLine("=== " + UID); 
            TerraIRC.Users.Add(UID, this);
            this.isIntroduced = introduced;
        }
        
        public void joinChannel(Channel chan)
        {
            Protocol.protocolPlugin.joinUser(this, chan);
        }
        public void kill(string reason)
        {
            //server kill client
        }
        
    }
}