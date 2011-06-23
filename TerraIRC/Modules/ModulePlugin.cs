using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace TerraIRC
{
    public abstract class ModulePlugin
    {
        public ArrayList moduleHooks = new ArrayList();
        public User moduleClient;
        public void registerClient(User client)
        {
            if (moduleClient == null)
            {
                this.moduleClient = client;
            }
        }
        public void registerHook(Hooks hook)
        {
            if (!moduleHooks.Contains(hook))
            {
                moduleHooks.Add(hook);
                Console.WriteLine(hook + " added");
            }
        }
        public string generateUID()
        {
            return TerraIRC.serverLinking_SID + ProtocolPlugin.generateUID();
        }
        public abstract void Initialize();
        public virtual void onUserMessageChannel(ChannelMessageEvent ev) { }
        public virtual void onUserMessageClient(UserMessageEvent ev) { }
        public virtual void onUserPartChannel() { }
        public virtual void onClientKilled(KillEvent ev) { }
        public virtual void onClientKill(KillEvent ev) { }
        public virtual void onUserConnect(UserEvent ev) { }
        public virtual void onUserNickChange(UserNickChangeEvent ev) { }
        public virtual void onUserIdentify(UserEvent ev) { }
        public virtual void onChannelLog(ChannelLogEvent ev) { }
        public virtual void onClientIntroduce(ClientIntroduceEvent ev) { }
        public virtual void onServerBurst() { }
    }
}
