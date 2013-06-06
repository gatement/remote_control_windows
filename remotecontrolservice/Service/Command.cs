using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace remotecontrolservice
{
    public class Command
    {
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }
    }
}
