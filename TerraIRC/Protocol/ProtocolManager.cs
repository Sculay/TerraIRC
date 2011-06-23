using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;


namespace TerraIRC
{
    public class Protocol
    {
        public static ProtocolPlugin protocolPlugin;
        public static void loadPlugins()
        {
            try
            {
                Console.WriteLine("Loading protocol modules..");
                foreach (string str in Directory.GetFiles(@"serverplugins\protocols\"))
                {
                    FileInfo info = new FileInfo(str);
                    if (info.Name.ToLower() == TerraIRC.serverLinking_Protocol.Split('.')[0])
                    {
                        Console.WriteLine("Loading Protocol: " + info.Name);
                        Assembly assembly = Assembly.LoadFrom(info.FullName);
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (!type.IsAbstract && (type.BaseType == typeof(ProtocolPlugin)))
                            {
                                ProtocolPlugin plugin = (ProtocolPlugin)Activator.CreateInstance(type);
                                protocolPlugin = plugin;
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine("Protocol plugin loaded!");
            }
            catch (ReflectionTypeLoadException exception)
            {
                Console.WriteLine(exception.ToString());
                foreach (Exception exception2 in exception.LoaderExceptions)
                {
                    Console.WriteLine(exception2.ToString());
                }
                Console.WriteLine("Problem loading protocol plugins");
            }
        }

    }
}
