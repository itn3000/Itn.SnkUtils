using System;
using Org.BouncyCastle;
using bcssl = Org.BouncyCastle.OpenSsl;
using System.IO;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Itn.SnkUtils
{
    class MyPasswordFinder : bcssl.IPasswordFinder
    {
        public char[] GetPassword()
        {
            var str = McMaster.Extensions.CommandLineUtils.Prompt.GetPassword("input password:");
            return str.ToCharArray();
        }
    }
    [Command()]
    [Subcommand("create", typeof(CreateSnkCommand))]
    [Subcommand("convert-from", typeof(ConvertFromPkcs8PemCommand))]
    [Subcommand("convert-to", typeof(ConvertToPkcs8PemCommand))]
    [HelpOption]
    class RootApp
    {

    }
    class Program
    {
        static CommandLineApplication<RootApp> CreateApp()
        {
            var services = new ServiceCollection();
            services.AddTransient<Org.BouncyCastle.OpenSsl.IPasswordFinder, ConsolePassFinder>();
            services.AddTransient<IConsole, PhysicalConsole>();
            var app = new CommandLineApplication<RootApp>();
            app.Conventions.UseDefaultConventions();
            app.Conventions.UseConstructorInjection(services.BuildServiceProvider());
            return app;
        }
        static void Main(string[] args)
        {
            try
            {
                var rc = CreateApp().Execute(args);
                Environment.ExitCode = rc;
            }
            catch (Exception e)
            {
                Console.WriteLine($"failed to execute app:{e}");
                Environment.ExitCode = 1;
            }
        }
    }
}
