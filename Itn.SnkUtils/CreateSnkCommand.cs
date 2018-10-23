using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;

namespace Itn.SnkUtils
{
    [Command(Description = "create snk file")]
    [HelpOption]
    public class CreateSnkCommand
    {
        [Required]
        [Argument(0, "OUTPUT_PATH", "path to output file(required)")]
        public string OutputPath { get; }
        [Option("-b|--keybits <BITNUMBER>", "number of key bits(default: 2048)", CommandOptionType.SingleValue)]
        public int KeyBits { get; } = 2048;
        public void OnExecute()
        {
            const int OffsetOfAlgId = 1 + 1 + 2;
            using (var rsa = new RSACryptoServiceProvider(KeyBits))
            {
                var bytes = rsa.ExportCspBlob(true);
                // set ALG_ID to 0x00002400(little endian)
                bytes[OffsetOfAlgId] = 0;
                bytes[OffsetOfAlgId + 1] = 0x24;
                bytes[OffsetOfAlgId + 2] = 0;
                bytes[OffsetOfAlgId + 3] = 0;
                File.WriteAllBytes(OutputPath, rsa.ExportCspBlob(true));
            }
        }
    }
}