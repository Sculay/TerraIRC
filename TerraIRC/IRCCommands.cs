using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using System.IO;

namespace TerraIRC
{
    class IRCCommands
    {
        public static void Finger()
        {
            if (TerraIRC.enableFingering)
            {
                IRC.send("PRIVMSG " + TerraIRC.channel + " :....................../´¯/)");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :....................,/¯../");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :.................../..../");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :............./´¯/'...'/´¯¯`·¸");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :........../'/.../..../......./¨¯\\");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :........('(...´...´.... ¯~/'...')");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :.........\\.................'...../");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :..........''...\\.......... _.·´");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :............\\..............(");
                IRC.send("PRIVMSG " + TerraIRC.channel + " :..............\\.............\\...");
            }
        }

        public static void Players()
        {
            string str = "";
            int pCount = 0;
            foreach (Player pActive in Main.player)
            {
                if (pActive.active)
                {
                    pCount++;
                    if (str == "")
                    {
                        str = str + pActive.name;
                    }
                    else
                    {
                        str = str + ", " + pActive.name;
                    }
                }
            }
            if (pCount == 0)
            {
                IRC.send("PRIVMSG " + TerraIRC.channel + " :No one is Terrariaing right now.");
            }
            else
            {
                IRC.send("PRIVMSG " + TerraIRC.channel + " :Current players: " + str);
            }
        }

        public static void Whitelist(string player)
        {
            string savePath = Path.Combine("tshock", "whitelist.txt");
            TextWriter tw = new StreamWriter(savePath, true);
            tw.WriteLine(player);
            tw.Close();
            IRC.send("PRIVMSG " + TerraIRC.channel + " :\"" + player + "\" added to the whitelist!");
        }

        public static void reloadSettings()
        {
            TerraIRC.loadSettings();
        }
    }
}
