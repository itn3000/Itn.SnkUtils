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
    [Command(Description = "convert from PKCS8 PEM formatted RSA private key. if password needed, read from stdin")]
    [HelpOption]
    class ConvertFromPkcs8PemCommand
    {
        [Required]
        [Argument(1, "OUTPUT_PATH", "output file path")]
        public string OutputPath { get; }
        [Required]
        [Argument(0, "INPUT_PATH", "input file path(PKCS8 PEM format)")]
        public string InputPath { get; }
        public ConvertFromPkcs8PemCommand(bcssl.IPasswordFinder passwordFinder)
        {
            m_Finder = passwordFinder;
        }
        bcssl.IPasswordFinder m_Finder;
        public void OnExecute(IConsole console)
        {
            using (var istm = File.OpenRead(InputPath))
            using (var sreader = new StreamReader(istm))
            {
                var preader = new bcssl.PemReader(sreader, m_Finder);
                var pem = preader.ReadObject();
                if (pem is bccrypto.AsymmetricCipherKeyPair keyPair)
                {
                    var rsaPrivateKey = keyPair.Private as bccrypto.Parameters.RsaPrivateCrtKeyParameters;
                    // 'ToRSA()' uses unsupported API in Linux.
                    var rsaparam = bcsec.DotNetUtilities.ToRSAParameters(rsaPrivateKey);
                    var cspparam = new CspParameters();
                    cspparam.KeyNumber = (int)KeyNumber.Signature;
                    using (var rsa = new RSACryptoServiceProvider(cspparam))
                    {
                        rsa.ImportParameters(rsaparam);
                        File.WriteAllBytes(OutputPath, rsa.ExportCspBlob(true));
                    }
                }
            }
        }
    }
}