using System;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using bcssl = Org.BouncyCastle.OpenSsl;
using bcsec = Org.BouncyCastle.Security;
using bcasn1 = Org.BouncyCastle.Asn1;
using bccrypto = Org.BouncyCastle.Crypto;

namespace Itn.SnkUtils
{
    [Command(Description = "convert snk to PKCS8 PEM RSA keypair included private key(no encryption)")]
    [HelpOption]
    class ConvertToPkcs8PemCommand
    {
        [Required]
        [Argument(1, Name = "OUTPUT_PATH", Description = "output file path")]
        public string OutputPath { get; }
        [Required]
        [Argument(0, Name = "INPUT_PATH", Description = "input file path")]
        public string InputPath { get; }
        public void OnExecute(IConsole console)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportCspBlob(File.ReadAllBytes(InputPath));
                using (var ostm = File.Create(OutputPath))
                using (var tw = new StreamWriter(ostm, new UTF8Encoding(false)))
                {
                    var pwriter = new bcssl.PemWriter(tw);
                    var keyPair = bcsec.DotNetUtilities.GetRsaKeyPair(rsa);
                    var pkcs8gen = new bcssl.Pkcs8Generator(keyPair.Private, bcssl.Pkcs8Generator.PbeSha1_3DES);
                    pkcs8gen.SecureRandom = new bcsec.SecureRandom();
                    pwriter.WriteObject(pkcs8gen);
                }
            }
        }
    }
}