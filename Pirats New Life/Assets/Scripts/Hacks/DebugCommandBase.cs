using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Hacks
{
    public class DebugCommandBase
    {
        private string _comandId;
        private string _comandDescription;
        private string _comandFormat;

        public string comandId { get { return _comandId; } }
        public string comandDescription { get { return _comandDescription; } }
        public string comandFormat { get { return _comandFormat; } }

        public DebugCommandBase(string id, string description, string format)
        {
            _comandId = id;
            _comandDescription = description;
            _comandFormat = format;
        }
    }

    public class DebugCommand : DebugCommandBase
    {
        private Action _command;

        public DebugCommand(string id, string description, string format, Action command) : base (id, description, format)
        {
            _command = command;
        }

        public void Invoke()
        {
            _command.Invoke();
        }
    }
    public class DebugCommand<T1> : DebugCommandBase
    {
        private Action<T1> _command;

        public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
        {
            _command = command;
        }

        public void Invoke(T1 value)
        {
            _command.Invoke(value);
        }
    }
}

