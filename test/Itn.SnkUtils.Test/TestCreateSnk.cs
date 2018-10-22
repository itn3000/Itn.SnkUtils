using System;
using Xunit;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using McMaster.Extensions.CommandLineUtils;

namespace Itn.SnkUtils.Test
{
    public class TestCreateSnk
    {
        [Fact]
        public void TestSuccess()
        {
            var app = new CommandLineApplication<RootApp>();
            app.Conventions.UseDefaultConventions();
            var tempName = Path.GetTempFileName();
            try
            {
                var rc = app.Execute("create", tempName);
                Assert.Equal(0, rc);
                using(var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportCspBlob(File.ReadAllBytes(tempName));
                    Assert.Equal(2048, rsa.KeySize);
                }
            }
            finally
            {
                if (File.Exists(tempName))
                {
                    File.Delete(tempName);
                }
            }
        }
        [Theory]
        [InlineData(1024)]
        [InlineData(2048)]
        [InlineData(4096)]
        public void TestSuccessBits(int keybits)
        {
            var app = new CommandLineApplication<RootApp>();
            app.Conventions.UseDefaultConventions();
            var tempName = Path.GetTempFileName();
            try
            {
                var rc = app.Execute("create", tempName, "-b", keybits.ToString());
                Assert.Equal(0, rc);
                using(var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportCspBlob(File.ReadAllBytes(tempName));
                    Assert.Equal(keybits, rsa.KeySize);
                }
            }
            finally
            {
                if (File.Exists(tempName))
                {
                    File.Delete(tempName);
                }
            }
        }
    }
}
