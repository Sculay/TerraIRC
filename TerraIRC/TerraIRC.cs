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
using Microsoft.Xna.Framework.Content;
using System.Collections;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;
using System.ComponentModel;

namespace TerraIRC
{
    [APIVersion(1, 5)]
    public class TerraIRC : TerrariaPlugin
    {
        // Settings
        public Dictionary<string, string> settings;
        public string settingsPath = "terrairc.txt";

        // IRC fields
        public static string server;
        public static int port;
        public static string authtype;
        public static string authpass;
        public static string nickname;
        public static string username;
        public static string channel;
        public static bool enableServerLinking;
        private Thread irc;
        public bool sendCommandsIRC;
        public static bool rawConsole;
        public static string commandPrefix;
        public static bool enableFingering;
        public static string whitelistPath;
        public static string serverLinking_server;
        public static string serverLinking_SID;
        public static string serverLinking_Pass;
        public static string serverLinking_Desc;
        public static string serverLinking_Protocol;

        public static Dictionary<string, User> Users;
        public static ArrayList Clients;
        public static Dictionary<string, Channel> Channels;

        public override string Name
        {
            get { return "TerraIRC"; }
        }

        public override Version Version
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
            NetHooks.GreetPlayer += OnGreetPlayer;
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
            NetHooks.GreetPlayer -= OnGreetPlayer;
            ServerHooks.Leave -= OnLeave;
            IRC.send("QUIT :Unloaded!");
            irc.Abort();
            Console.WriteLine("[TerraIRC] Unloaded :(");
        }

        public void startIRC()
        {
            IRC.connect(server, port, authtype, authpass, nickname, username, channel);
        }

        private void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
        {
            string p = Main.player[ply].name;
            if (text.StartsWith("/"))
            {
                return;
            }
            else
            {
                IRC.send("PRIVMSG " + channel + " :(" + p + ") " + text);
            }
        }

        //public override void onPlayerDeath(PlayerEvent ev)
        //{
        //    base.onPlayerDeath(ev);
        //    string p = ev.getPlayer().name;
        //    IRC.send("PRIVMSG " + channel + " :" + p + " was slain..");
        //}

        private void OnGreetPlayer(int who, HandledEventArgs e)
        {
            string p = Main.player[who].name;
            IRC.send("PRIVMSG " + channel + " :[" + p + " connected]");
        }

        private void OnLeave(int who)
        {
            string p = Main.player[who].name;
            IRC.send("PRIVMSG " + channel + " :[" + p + " disconnected]");
        }

        protected string getSetting(string setting, string settingValue)
        {
            try
            {
                string ret = settings[setting];
                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("[TerraIRC] Missing setting: " + setting);
                TextWriter t = new StreamWriter(settingsPath, true);
                t.WriteLine(setting + "=" + settingValue);
                t.Close();
                this.settings.Add(setting, settingValue);
                return settingValue;
            }
        }

        public static void loadSettings()
        {
            this.settings = new Dictionary<string, string>();
            if (!File.Exists(settingsPath))
            {
                TextWriter writer = new StreamWriter(settingsPath);
                writer.WriteLine("#server linking allows you to connect terraria server clients directly to a TS6 server, join irc.pwncraft.net #pwncraft for help on setting up");
                writer.WriteLine("serverLinking=false");
                writer.WriteLine("#only charybdis is supported now, it should work on ratbox and other charybdis based ircds");
                writer.WriteLine("serverLinking-Protocol=charybdis");
                writer.WriteLine("serverLinking-server=localhost");
                writer.WriteLine("serverLinking-SID=30X");
                writer.WriteLine("serverLinking-Pass=asdf");
                writer.WriteLine("serverLinking-Desc=TerraIRC - Terraria <-> IRC Bridge");
                writer.WriteLine("server=irc.pwncraft.net");
                writer.WriteLine("port=6667");
                writer.WriteLine("# use either nickserv, x or none. refer to your ircops for more information");
                writer.WriteLine("# this doesn't work in the current version 1.0");
                writer.WriteLine("authtype=x");
                writer.WriteLine("authpass=mypassword");
                writer.WriteLine("nickname=TerraIRC");
                writer.WriteLine("username=Terraria IRC Bot");
                writer.WriteLine("channel=#pwncraft");
                writer.WriteLine("raw-console=true");
                writer.WriteLine("irc-prefix=!");
                writer.WriteLine("fingering=true");
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
                server = getSetting("server", "irc.pwncraft.net");
                port = int.Parse(getSetting("port", "6667"));
                authtype = getSetting("authtype", "x");
                authpass = getSetting("authpass", "mypassword");
                nickname = getSetting("nickname", "TerraIRC");
                username = getSetting("username", "Terraria IRC Bot");
                channel = getSetting("channel", "#pwncraft");
                rawConsole = bool.Parse(getSetting("raw-console", "true"));
                commandPrefix = getSetting("irc-prefix", "!");
                enableFingering = bool.Parse(getSetting("fingering", "true"));
                enableServerLinking = bool.Parse(getSetting("serverLinking", "false"));
                if (enableServerLinking)
                {
                    serverLinking_server = getSetting("serverLinking-server", "localhost");
                    serverLinking_Pass = getSetting("serverLinking-pass", "asdafs");
                    serverLinking_SID = getSetting("serverLinking-SID", "30X");
                    serverLinking_Desc = getSetting("serverLinking-Desc", "TerraIRC - Terraria <-> IRC Bridge");
                    serverLinking_Protocol = getSetting("serverLinking-Protocol", "charybdis").ToLower();
                    Users = new Dictionary<string, User>();
                    Channels = new Dictionary<string, Channel>();
                    Clients = new ArrayList();
                }
                
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
            if (TerraIRC.rawConsole == true)
            {
                Console.WriteLine("[TerraIRC] IRC Raw: " + text);
            }
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
                                    int ply = p.whoAmi;
                                    string message = "[IRC] <" + nickname + "> has joined IRC";
                                    // pre 1.05
                                    // NetMessage.SendData(0x19, ply, -1, message, 255, 0f, 255f, 0f);
                                    NetMessage.SendData((int)PacketTypes.ChatText, ply, -1, message, 255, 0f, 255f, 0f);
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
                                    int ply = p.whoAmi;
                                    string message = "[IRC] <" + nickname + "> has left IRC";
                                    // pre 1.05
                                    // NetMessage.SendData(0x19, ply, -1, message, 255, 0f, 255f, 0f);
                                    NetMessage.SendData((int)PacketTypes.ChatText, ply, -1, message, 255, 0f, 255f, 0f);
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

                                if (match.Groups[3].Value.StartsWith(TerraIRC.commandPrefix))
                                {
                                    string command = match.Groups[3].Value.Remove(0, 1).ToLower();
                                    string[] commandArray = match.Groups[3].Value.Split(' ');
                                    Console.WriteLine(match.Groups[3].Value);
                                    switch (command)
                                    {
                                        case "players":
                                            IRCCommands.Players();
                                                break;

                                        case "finger":
                                            IRCCommands.Finger();
                                                break;

                                        case "terrairc":
                                                IRCCommands.reloadSettings();
                                                break;
                                    }
                                }
                                else
                                {
                                    if (nickname != TerraIRC.nickname)
                                    {
                                        foreach (Player p in Main.player)
                                        {
                                            if (p.name.Length > 0)
                                            {
                                                string ircMsg = "[IRC] <" + nickname + "> " + match.Groups[3].Value;
                                                int ply = p.whoAmi;
                                                string message = ircMsg.Replace(meme, '*');
                                                // pre 1.05
                                                // NetMessage.SendData(0x19, ply, -1, message, 255, 0f, 255f, 0f);
                                                NetMessage.SendData((int)PacketTypes.ChatText, ply, -1, message, 255, 0f, 255f, 0f);
                                            }
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