using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TerraIRC
{
    public class ClientIntroduceEvent : Event
    {
        public Client client;
        public ClientIntroduceEvent(Client client)
        {
            this.client = client;
        }
    }
}
