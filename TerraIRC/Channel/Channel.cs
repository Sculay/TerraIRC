using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace TerraIRC
{
    public class Channel
    {
        public string name;
        public bool registered = false;
        public Dictionary<string, User> Users;
        public Channel(string name)
        {
            this.name = name;
            Users = new Dictionary<string, User>();
            TerraIRC.Channels.Add(name.ToLower(), this);
        }
        public void addToChannel(User user)
        {
            Users.Add(user.UID, user);
        }
        public void removeFromChannel(User user)
        {
            if (Users.ContainsValue(user))
            {
                Users.Remove(user.UID);
                return;
            }
            Console.WriteLine("Attempted to remove non-existant user from channel " + this.name);
        }
        public bool containsUser(User user)
        {
            if (Users.ContainsValue(user))
            {
                return true;
            }
            return false;
        }

    }
}
