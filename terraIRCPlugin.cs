using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;

namespace TerraIRC
{
    public class TerraIRC : TerrariaPlugin
    {
	
	    // Settings
        public Dictionary<string, string> settings;
        public string settingsPath = "terrairc.txt";

        // IRC fields
        public string server;
        public int port;
        public string authtype;
        public string authpass;
        public string nickname;
        public string username;
        public string channel;
        private Thread irc;
	
	public override string Name
        {
            get { return "TerraIRC"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 2); }
        }

        public override Version APIVersion
        {
            get { return new Version(1, 2); }
        }

        public override string Author
        {
            get { return "PwnCraft"; }
        }

        public override string Description
        {
            get { return "Terraria <-> IRC link bot"; }
        }

        public TerraIRC(Main game)
            : base(game)
        {
        }
		
		public override void Initialize()
        {
            ServerHooks.Chat += OnChat;
            ServerHooks.Join += OnJoin;
			ServerHooks.Leave += OnLeave;
			loadSettings();
            this.irc = new Thread(new ThreadStart(startIRC));
            this.irc.Start();
            Console.WriteLine("Connecting to " + server + ":" + port);
            Console.WriteLine("[TerraIRC] " + Version + " loaded!");
        }

        public override void DeInitialize()
        {
			ServerHooks.Chat -= OnChat;
            ServerHooks.Join -= OnJoin;
			ServerHooks.Leave -= OnLeave;
			irc.Suspend();
            IRC.send("QUIT :Unloaded!");
            Console.WriteLine("[TerraIRC] Unloaded :(");
        }
		
        public void startIRC()
        {
            IRC.connect(server, port, authtype, authpass, nickname, username, channel);
        }
		
        private void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
        {
			string p = Main.player[ply].name;
            IRC.send("PRIVMSG " + channel + " :(" + p + ") " + msg);
			e.Handled = true;
        }
		
        //public override void onPlayerDeath(PlayerEvent ev)
        //{
        //    base.onPlayerDeath(ev);
        //    string p = ev.getPlayer().name;
        //    IRC.send("PRIVMSG " + channel + " :" + p + " was slain..");
        //}
		
		private void OnJoin(int ply, HandledEventArgs handler)
        {				
            string p = Main.player[ply].name;
            IRC.send("PRIVMSG " + channel + " :[" + p +" connected]");
			handler.Handled = true;
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
                writer.WriteLine("# this doesn't work in the current version 1.0");
                writer.WriteLine("authtype=x");
                writer.WriteLine("authpass=mypassword");
                writer.WriteLine("nickname=TerraIRC");
                writer.WriteLine("username=Terraria IRC Bot");
                writer.WriteLine("channel=#pwncraft");
                writer.Close();
            }

            foreach (string str2 in File.ReadAllLines(settingsPath))
            {
                if (!str2.StartsWith("#"))
                {
                    settings.Add(str2.Split(new char[] { '=' })[0], str2.Split(new char[] { '=' })[1]);
                }
            }
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
            Console.WriteLine("[TerraIRC] IRC Raw: " + text);
        }
        public static void command(string cmd)
        {
            string[] cmdarray = cmd.Split(' ');
            Player p;
            int pna = 0;
            foreach (Player pl in Main.player)
            {
                if (pl.name.Length < 1)
                {

                    pna = pl.whoAmi;
                    break;
                }
            }
            p = Main.player[pna];
            p.name = "Console";
            CommandEvent commandevent = new CommandEvent(cmdarray, p);
            commandevent = null;
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
                                if (p.name.Length > 0)
                                {
                                    p.sendMessage("[IRC]<" + nickname + "> has joined IRC");
                                }
                            }
                            
                        }
                        else if (inputLine.EndsWith("PART :" + channel))
                        {
                            //get nick
                            nickname = inputLine.Substring(1, inputLine.IndexOf("!") - 1);
                            foreach (Player p in Main.player)
                            {
                                if (p.name.Length > 0)
                                {
                                    p.sendMessage("[IRC]<" + nickname + "> has left IRC");
                                }
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
                    // Close all streams
                    writer.Close();
                    reader.Close();
                    irc.Close();
                }
            }
            catch (EndOfStreamException e)
            {
                // Show the exception, sleep for a while and try to establish a new connection to irc server
                Console.WriteLine(e.ToString());

                /*string[] argv = { };
                connect(argv[0], int.Parse(argv[1]), argv[2], argv[3], argv[4], argv[5], argv[6]);*/

            }
        }
    }
    
}