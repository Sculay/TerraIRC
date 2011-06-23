using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraIRC
{
    public class Event
    {
        private bool state = true;
        public void setState(bool a)
        {
            this.state = a;
        }
        public bool getState()
        {
            return this.state;
        }
    }
}
