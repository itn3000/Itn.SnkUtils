using System;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using Org.BouncyCastle;
using bcpem = Org.BouncyCastle.Utilities.IO.Pem;
using bcssl = Org.BouncyCastle.OpenSsl;
using bccrypto = Org.BouncyCastle.Crypto;
using bcutil = Org.BouncyCastle.Utilities;
using bcx509 = Org.BouncyCastle.X509;
using bcsec = Org.BouncyCastle.Security;

namespace Itn.SnkUtils
{
    [Command(Description = "convert from openssl's RSA private key(PEM format)")]
    [HelpOption]
    class ConvertFromOpensslPemCommand
    {
        [Required]
        [Argument(1, "OUTPUT_PATH", "output file path")]
        public string OutputPath { get; }
        [Required]
        [Argument(0, "INPUT_PATH", "input file path(openssl's PEM format)")]
        public string InputPath { get; }
        public void OnExecute(IConsole console)
        {
            using (var istm = File.OpenRead(InputPath))
            using (var sreader = new StreamReader(istm))
            {
                var preader = new bcssl.PemReader(sreader, ConsolePassFinder.Instance);
                // var pem = preader.ReadPemObject();
                // foreach (var header in pem.Headers)
                // {
                //     if (header is bcpem.PemHeader pemheader)
                //     {
                //         console.Error.WriteLine($"{pemheader.Name}={pemheader.Value}");
                //     }
                //     else
                //     {
                //         console.Error.WriteLine($"{header}");
                //     }
                // }
                var pem = preader.ReadObject();
                if(pem is bccrypto.AsymmetricCipherKeyPair keyPair)
                {
                    var rsaPrivateKey = keyPair.Private as bccrypto.Parameters.RsaPrivateCrtKeyParameters;
                    var rsaparam = bcsec.DotNetUtilities.ToRSAParameters(rsaPrivateKey);
                    using(var rsa = new RSACryptoServiceProvider())
                    {
                        rsa.ImportParameters(rsaparam);
                        File.WriteAllBytes(OutputPath, rsa.ExportCspBlob(true));
                    }
                }
            }
        }
    }
}