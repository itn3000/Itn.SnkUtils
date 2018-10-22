using bcssl = Org.BouncyCastle.OpenSsl;
using System;
using McMaster.Extensions.CommandLineUtils;

namespace Itn.SnkUtils
{
    class ConsolePassFinder : bcssl.IPasswordFinder
    {
        IConsole m_Console;
        public char[] GetPassword()
        {
            if (m_Console.IsInputRedirected)
            {
                return m_Console.In.ReadLine().Trim().ToCharArray();
            }
            else
            {
                return Prompt.GetPassword("input password: ").Trim().ToCharArray();
            }
        }
        public ConsolePassFinder(IConsole console)
        {
            m_Console = console;
        }
    }
}