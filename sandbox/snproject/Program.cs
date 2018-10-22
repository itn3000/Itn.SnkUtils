using System;
using System.Reflection;

namespace snproject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"asm name is {typeof(Program).Assembly.FullName}");
        }
    }
}
