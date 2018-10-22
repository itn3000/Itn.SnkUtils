using bcssl = Org.BouncyCastle.OpenSsl;
using System;
using McMaster.Extensions.CommandLineUtils;

namespace Itn.SnkUtils
{
    class ConsolePassFinder : bcssl.IPasswordFinder
    {
        public char[] GetPassword()
        {
            return Prompt.GetPassword("input password: ").Trim().ToCharArray();
        }
        public static ConsolePassFinder Instance = new ConsolePassFinder();
    }
}