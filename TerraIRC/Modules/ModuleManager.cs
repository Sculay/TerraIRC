using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace TerraIRC
{
    public class Module
    {
        public static ArrayList moduleList;
        public static void loadPlugins()
        {
            moduleList = new ArrayList();
            try
            {
                Console.WriteLine("Loading modules..");
                foreach (string str in Directory.GetFiles(@"modules\"))
                {
                    FileInfo info = new FileInfo(str);
                    if (info.Name.EndsWith(".dll"))
                    {
                        Console.WriteLine("Loading module " + info.Name);
                        Assembly assembly = Assembly.LoadFrom(info.FullName);
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (!type.IsAbstract && (type.BaseType == typeof(ModulePlugin)))
                            {
                                ModulePlugin plugin = (ModulePlugin)Activator.CreateInstance(type);
                                moduleList.Add(plugin);
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine("Modules loaded!");
            }
            catch (ReflectionTypeLoadException exception)
            {
                Console.WriteLine(exception.ToString());
                foreach (Exception exception2 in exception.LoaderExceptions)
                {
                    Console.WriteLine(exception2.ToString());
                }
                Console.WriteLine("Problem loading modules!");
            }
        }
        public static void InitModules()
        {
            foreach (ModulePlugin plugin in moduleList)
            {
                plugin.Initialize();
            }
        }
        public static void callHook(Hooks hook, Client client, Event ev = null)
        {
            foreach (ModulePlugin module in moduleList)
            {
                try
                {
                    if (module.moduleHooks.Contains(hook))
                    {
                        Console.WriteLine("hook " + hook + " : " + module.ToString());
                        switch (hook)
                        {
                            case Hooks.USER_CHANNEL_PRIVMSG:
                                {
                                    ChannelMessageEvent me = (ChannelMessageEvent)ev;
                                    if (me.channel.containsUser(me.user))
                                    {
                                        module.onUserMessageChannel(me);
                                    }
                                    break;
                                }
                            case Hooks.USER_MESSAGE_CLIENT:
                                {
                                    Console.WriteLine("message called");
                                    UserMessageEvent me = (UserMessageEvent)ev;
                                    if (me.sendee == client)
                                    {
                                        module.onUserMessageClient(me);
                                    }
                                    break;
                                }
                            case Hooks.CLIENT_KILLED:
                                {
                                    KillEvent me = (KillEvent)ev;
                                    if (me.killee == client)
                                    {
                                        module.onClientKilled(me);
                                    }
                                    break;
                                }
                            case Hooks.USER_CONNECT:
                                {
                                    UserEvent me = (UserEvent)ev;
                                    if (me.user != client)
                                    {
                                        module.onUserConnect(me);
                                    }
                                    break;
                                }
                            case Hooks.USER_NICKCHANGE:
                                {
                                    UserNickChangeEvent me = (UserNickChangeEvent)ev;
                                    if (me.user != client)
                                    {
                                        module.onUserNickChange(me);
                                    }
                                    break;
                                }
                            case Hooks.USER_IDENTIFY:
                                {
                                    UserEvent me = (UserEvent)ev;
                                    module.onUserIdentify(me);
                                    break;
                                }
                            case Hooks.CHANNEL_LOG:
                                {
                                    ChannelLogEvent me = (ChannelLogEvent)ev;
                                    module.onChannelLog(me);
                                    break;
                                }
                            case Hooks.CLIENT_INTRO:
                                {
                                    ClientIntroduceEvent me = (ClientIntroduceEvent)ev;
                                    Console.WriteLine("Client " + me.client.nickname + " introduced!");
                                    module.onClientIntroduce(me);
                                    break;
                                }
                            case Hooks.SERVER_BURST:
                                {
                                    module.onServerBurst();
                                    break;
                                }
                            
                        }
                    }
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

    }
}
