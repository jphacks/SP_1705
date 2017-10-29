using System;
using System.Collections.Generic;
using System.Text;

namespace DMessenger
{
    public sealed class Account
    {
        public string Name { get; set; }
        public byte[] PublicKey { get; }
        public DateTime UpdateTime { get; set; }
    }
}
