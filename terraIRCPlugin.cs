using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Terraria
{
    class TerraIRC : Plugin
    {
        // Settings
        public Dictionary<string, string> settings;
        public string settingsPath = "terrairc.txt";

        // IRC fields
        public string server = "irc.pwncraft.net";
        public int port = 6667;
        public string authtype = "none";
        public string authpass = "none";
        public string nickname = "Pwnaria";
        public string username = "terra.pwncraft.net";
        public string channel = "#pwncraft";
        private Thread irc;
        public override void Initialize()
        {
            pluginName = "TerraIRC";
            pluginDescription = "Terraria <-> IRC link bot";
            pluginAuthor = "PwnCraft";
            pluginVersion = "v1.0";
            this.registerHook(Hook.PLAYER_CHAT);
            this.registerHook(Hook.PLAYER_DEATH);
            this.registerHook(Hook.PLAYER_JOIN);
            //loadSettings();
            this.irc = new Thread(new ThreadStart(startIRC));
            this.irc.Start();
            Console.WriteLine("Connecting to " + server + ":" + port);
            Console.WriteLine("[TerraIRC] " + pluginVersion + " loaded!");
        }
        public void startIRC()
        {
            IRC.connect(server, port, authtype, authpass, nickname, username, channel);
        }
        public override void onPlayerChat(ChatEvent ev)
        {
            base.onPlayerChat(ev);
            string p = ev.getPlayer().name;
            string text = ev.getChat();
            IRC.send("PRIVMSG #pwncraft :(" + p + ") " + text);
        }
        public override void onPlayerDeath(PlayerEvent ev)
        {
            base.onPlayerDeath(ev);
            string p = ev.getPlayer().name;
            IRC.send("PRIVMSG #pwncraft :" + p + " was slain..");
        }
        public override void Unload()
        {
            irc.Suspend();
            Console.WriteLine("[TerraIRC] Unloaded :(");
        }
        public override void onPlayerJoin(PlayerEvent ev)
        {
            base.onPlayerJoin(ev);
            string p = ev.getPlayer().name;
            IRC.send("PRIVMSG #pwncraft :[" + p +" connected]");
        }
        public void loadSettings()
        {
            this.settings = new Dictionary<string, string>();
            if (!File.Exists(settingsPath))
            {
                TextWriter writer = new StreamWriter(settingsPath);
                writer.WriteLine("server=irc.pwncraft.net");
                writer.WriteLine("port=6667");
                writer.WriteLine("# use either nickserv, x or none. refer to your ircops for more information");
                writer.WriteLine("authtype=x");
                writer.WriteLine("authpass=mypassword");
                writer.WriteLine("nickname=TerraIRC");
                writer.WriteLine("username=Terraria IRC Bot");
                writer.WriteLine("channel=#pwncraft");
                writer.Close();
            }

            foreach (string str2 in File.ReadAllLines(settingsPath))
            {
                this.settings.Add(str2.Split(new char[] { '=' })[0], str2.Split(new char[] { '=' })[1]);
            }

            Console.WriteLine(settings.Keys);
            // IRC fields
            try
            {
                server = this.settings["server"];
                port = int.Parse(this.settings["port"]);
                authtype = this.settings["authtype"];
                authpass = this.settings["authpass"];
                nickname = this.settings["nickname"];
                username = this.settings["username"];
                channel = this.settings["channel"];
            }
            catch (NullReferenceException exception)
            {
                Console.WriteLine("[TerraIRC] Error: " + exception.ToString());
            }
        }

    }
    public static class IRC
    {

        public static StreamWriter writer;
        public static void send(string text)
        {
            writer.WriteLine(text);
            writer.Flush();
            Console.WriteLine("OUT: " + text);
        }
        public static void connect(string server, int port, string authtype, string authpass, string nick, string user, string channel)
        {
            char meme;
            meme = Convert.ToChar(1);
            NetworkStream stream;
            TcpClient irc;
            string inputLine;
            StreamReader reader;
            string nickname;
            try
            {
                Console.WriteLine("CONNETING!");
                irc = new TcpClient(server, port);
                stream = irc.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                string[] users = user.Split(' ');
                send("USER " + users[0] + " 0 * :" + user);
                send("NICK " + nick);
                
                while (true)
                {
                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        string[] arr = inputLine.Split(' ');
                        //Console.WriteLine(inputLine);
                        if (arr[1] == "376")
                        {
                            send("JOIN " + channel);
                        }
                        else if (inputLine.EndsWith("JOIN :" + channel))
                        {
                            //get nick
                            nickname = inputLine.Substring(1, inputLine.IndexOf("!") - 1);
                            foreach (Player p in Main.player)
                            {
                                p.sendMessage("[IRC]<" + nickname + "> has joined IRC");
                            }
                            
                        }
                        else if (inputLine.StartsWith("PING"))
                        {
                            send("PONG " + arr[1]);
                        }
                        else if ((arr[1] == "PRIVMSG") && (arr[2].ToLower() == channel.ToLower())) 
                        {
                            Regex text = new Regex(@":(.*) PRIVMSG (.*) :(.*)");
                            Match match = text.Match(inputLine);
                            if (match.Success)
                            {
                                //get nick
                                nickname = inputLine.Substring(1, inputLine.IndexOf("!") - 1);
                                if (nickname.ToLower() != "pwnsurvival")
                                {
                                    foreach (Player p in Main.player)
                                    {
                                        if (p.name.Length > 0)
                                        {
                                            string message = "[IRC]<" + nickname + "> " + match.Groups[3].Value;
                                            p.sendMessage(message.Replace(meme, '*'));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Close all streams
                    writer.Close();
                    reader.Close();
                    irc.Close();
                }
            }
            catch (Exception e)
            {
                // Show the exception, sleep for a while and try to establish a new connection to irc server
                Console.WriteLine(e.ToString());

                /*string[] argv = { };
                connect(argv[0], int.Parse(argv[1]), argv[2], argv[3], argv[4], argv[5], argv[6]);*/

            }
        }
    }
    
}