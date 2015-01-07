using CommandHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unturned
{
    public abstract class Module
    {

        internal virtual void Load() {return;}
        internal virtual void Create() {return;}
        internal virtual void Save() {return;}

        internal virtual IEnumerable<Command> GetCommands() { return null; }
        internal virtual String GetHelp() {return null;}

    }
}
