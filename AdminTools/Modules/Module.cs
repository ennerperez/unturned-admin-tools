using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unturned
{
    public abstract class Module
    {

        internal abstract void GetCommands();
        internal abstract void Load();
        internal abstract String GetHelp();

    }
}
