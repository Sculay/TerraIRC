using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraIRC
{
    public abstract class ProtocolPlugin
    {
        public bool hasBurst = false;
        public string server;
        public string thisserver;
        public int port;
        public string sendpass;
        public string SID;
        public abstract void Connect();
        public virtual void Send(string message) { }

        public void Init(string server, string thisserver, int port, string sendpass, string SID)
        {
            this.server = server;
            this.port = port;
            this.thisserver = thisserver;
            this.sendpass = sendpass;
            this.SID = SID;
        }
        public static int getTimeStamp()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            int timestamp = (int)t.TotalSeconds;
            return timestamp;
        }
        public User getUserFromUID(string UID)
        {
            if (Program.Users.ContainsKey(UID))
            {
                User user;
                Program.Users.TryGetValue(UID, out user);
                return user;
            }
            return null;
        }
        public Channel getChannelFromName(string channel)
        {
            if (Program.Channels.ContainsKey(channel.ToLower()))
            {
                Channel chan;
                Program.Channels.TryGetValue(channel.ToLower(), out chan);
                return chan;
            }
            return null;
        }
        public static int UIDCount = 100000;
        //yea, math.
        public static String generateUID()
        {
            //Initiate objects & vars    
            /*Random random = new Random();
            String randomString = "";
            int randNumber;
            int length = 6;
            //Loop ‘length’ times to generate a random number or character
            for (int i = 0; i < length; i++)
            {
                if (random.Next(1, 3) == 1)
                    randNumber = random.Next(97, 123); //char {a-z}
                else
                    randNumber = random.Next(48, 58); //int {0-9}

                //append random char or digit to random string
                randomString = randomString + (char)randNumber;
            }
            //return the random string
            return randomString.ToUpper();*/
            UIDCount++;
            return UIDCount.ToString();
        }
        public virtual void introduceUser(string nickname, string username, string modes, string hostname, string gecos, string UID)
        {
        }
        public virtual void msgUser(Client sender, User sendee, string message)
        {
        }
        public virtual void noticeUser(Client sender, User sendee, string message)
        {
        }
        public virtual void joinUser(User joinee, Channel channel)
        {
        }
        public virtual void killUser(User killee, string reason = null)
        {
        }
        public virtual void killUser(Client killer, User killee, string reason = null)
        {
        }
        public virtual void kickUser(Client kicker, User kickee, Channel channel, string reason = null)
        {
        }
        public virtual void chanMode(Client sender, Channel channel, User dest, string modes)
        {
        }
        public virtual void chanMode(Client sender, Channel channel, string modes)
        {
        }
        public virtual void msgChannel(Client sender, Channel channel, string message)
        {
        }
        public virtual void noticeChannel(Client sender, Channel channel, string message)
        {
        }
        public virtual void serverchanMode(Channel channel, string modes)
        {
        }
        public virtual void serverchanMode(Channel channel, User dest, string modes)
        {
        }
    }
}
