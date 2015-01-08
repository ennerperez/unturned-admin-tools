using CommandHandler;
using System;
using System.Collections.Generic;

namespace Unturned
{
    public abstract class Module
    {

        internal virtual void Create() { return; }
        internal virtual void Save() { return; }

        internal virtual IEnumerable<Command> GetCommands() { return null; }
        internal virtual String GetHelp() { return null; }

        internal virtual void Load() { return; }
        internal virtual void Refresh() { return; }
        internal virtual void Print() { return; }
        internal virtual void Clear() { return; }

    }
}
