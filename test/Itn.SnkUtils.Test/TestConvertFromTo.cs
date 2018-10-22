using System;
using Xunit;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Itn.SnkUtils.Test
{
    public class TestConvertFromTo
    {
        [Fact]
        public void TestConvertFromNoEncrypted()
        {
            var app = CreateApplication();
            var tempName = Path.GetTempFileName();
            var inputTempName = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(inputTempName, Encoding.UTF8.GetBytes(NoEncrypted));
                var rc = app.Execute("convert-from", inputTempName, tempName);
                Assert.Equal(0, rc);
                using (var rsa = new RSACryptoServiceProvider())
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
                if (File.Exists(inputTempName))
                {
                    File.Delete(inputTempName);
                }
            }
        }
        [Fact]
        public void ConvertFromEncrypted()
        {
            var app = CreateApplication();
            var tempName = Path.GetTempFileName();
            var inputTempName = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(inputTempName, Encoding.UTF8.GetBytes(Encrypted));
                var rc = app.Execute("convert-from", inputTempName, tempName);
                Assert.Equal(0, rc);
                using (var rsa = new RSACryptoServiceProvider())
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
                if (File.Exists(inputTempName))
                {
                    File.Delete(inputTempName);
                }
            }
        }
        [Fact]
        public void ConvertToPkcs8()
        {
            const int keySize = 4096;
            var app = CreateApplication();
            var tempName = Path.GetTempFileName();
            var inputTempName = Path.GetTempFileName();
            try
            {
                using (var rsa = new RSACryptoServiceProvider(keySize))
                {
                    File.WriteAllBytes(inputTempName, rsa.ExportCspBlob(true));
                }
                var rc = app.Execute("convert-to", inputTempName, tempName);
                Assert.Equal(0, rc);
                using (var fstm = File.OpenRead(tempName))
                using (var sr = new StreamReader(fstm))
                {
                    var preader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                    var pemObject = preader.ReadObject();
                    var keyPair = pemObject as Org.BouncyCastle.Crypto.AsymmetricKeyParameter;
                    Assert.NotNull(keyPair);
                    var rsaPrivate = keyPair as Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters;
                    Assert.NotNull(rsaPrivate);
                    var dotnetRsa = Org.BouncyCastle.Security.DotNetUtilities.ToRSA(rsaPrivate);
                    Assert.Equal(keySize, dotnetRsa.KeySize);
                }
            }
            finally
            {
                if (File.Exists(tempName))
                {
                    File.Delete(tempName);
                }
                if (File.Exists(inputTempName))
                {
                    File.Delete(inputTempName);
                }
            }
        }
        // RSA 2048 bits AES128 encrypted, passphrase = "abcde"
        const string Encrypted = @"-----BEGIN RSA PRIVATE KEY-----
Proc-Type: 4,ENCRYPTED
DEK-Info: AES-128-CBC,59EC21F9A54246245B8166DACD8719DF

H5JpDUqJyYP3hfU6LBUaB3wA8w7Ctcje7pZa785ldtbh7ak6lAnHRAbAuYMi4Nsr
hagiO3UFpfA7VRjwjJT0cMTh4OwgllmQpo2MwjXXC19UBPOGhmPUU30pPn19cEo8
pnfUrdsV3sWsCCTVL4D9zEs77KuSwQrXx/fi3Dglba7hEWfGjTA9Nkdw+HXoLttO
Y56nOz0Rz1DE3hdUMUMAtZDalMWJ5AHjbtVIxtb3zBWV+het7sZS+Z0XpjrykYox
CRGd30bLrRPC3L6KD3KAwVJBj0DT9IyI1/jAoRyI+yBxrVXH52zWDVngIVp+Fwb2
IsBpBBXmsWFWllsk8rALlA6gIv6lxiXq+mTfyNEvtX4U8UU8bLCSr2G1KH/mc+l3
25U7gb9voQotyjlqumZJYVKZtQfZpvxu395X4xL5k5koHpo1fmay52tGzLEU/66m
1zwKFiK2suBLIZXcz87v0aeDDv7mPqjK+mG4H4XLZtmzVnBAwScmuhIX9+6gPdon
7+8X/k+jtcZHjxhdcHtsGdCTacNO7WtQ0kUEC/VjL/2ezXnZN1c+/TFBM99jJXTV
wbihwbDbV3LOBgtzDhYs3yj9PjFg0lYDWXgqBixzHVzcV3xCrgZNdnkMl/Dd174N
3lBnwd0DIiaNfzrRUs558F2WTpMGxTBhTLaFlJBxZjNn9aGXl6m29FBQRRS+SH4v
1uA5AWx3jzTKdlMGdUbBZY2I09jPWKqH3+Dhog7/fi3P80Pi8sq49VhUxwbPJCKJ
RaWH1yfmymahZvpDm5J7K/szr2zYXOYZcTckOKgoVuZNf3kQSCE8bGDgukKP+zsT
hTl3Vqk4/3KQYMOlHPtHEXI2zXy+mywdNNA4L+NbL8L2PqOhH6EZmBRlNymAdlNI
94l81EIYD/kNPto0i/qarNV1P8pZn0ReTjheJtDC4SCJKiGK0vTJBh2ZTRtWsl8z
kkv0LgZNMKC8iwlSHMGCoip+G0kdwmjZhtXmnHVpE5NZ3sjJVSLoVJSa9su+YCrL
Pn7XNFfYkaxdhis+u7rlO5BZAqmUlOvfeIstvPv+lCJKyoDI40dCdE4xcsPuV3F1
urLOo8FlYdtn3uIEvlDeKGumdf9juuCBEklv+UVy77Uhkw/JQuyMeyehC9jVuyGP
xwfG+TMR5sLRWkM+hDPxiRNrLtnuO4oQN7wSo1RqxSgzD1igyA7w1w1MTKWKG6ug
WXgdnLZ62p+Z7YVhq4BZClIdYnTkoVDKWAL417uq03p0lU94p9vEkrp60HT6gWM5
Hij0Qi2HGpEiSU/KcsIlPf2ey8KoPKON64FSY5mP069Aiaf5XDl851wT7gKq0HHz
YkuvzRy6wsuPXTueB3AU9cnKKyPZpFj3ZDA2vfVx2c8JMVQliphywj/62kf8ZtAt
YktgurVoBGxlJtPpDdcCb+ByuGo9swOJyE0Gqp41t1goqbMocD/uOf/Qdoa4c5FU
uI0BTKJMFxE0vX3tz7CTk7Lp9Aev/tWhvTQJE2EYBiXPCiuxK9wt4nvqfkp4dAtZ
7mEsaue/TrhhJoQdYjHGBqVLLlpYyVa3gHwugiEhZlxQcf8d/FlxysVAPwDKR4GD
-----END RSA PRIVATE KEY-----
";
        // RSA private key 2048 bits no encrypted
        const string NoEncrypted = @"-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEA2wOaPOGJcmk44m0vHDyPAk3qyfxwZ/K9OptrTuTPgsotipT1
iXthAOEnBRHS0A5Nx+/sSONJTVQ50lZHemqIB55uRYhK05yjxTxGYFxsEVf3V9M9
ZY7nRorRNG/EYCYMtAu5dXaNj7cwaXD3YJ7VrrmRF0HRs89ikNSMjH7Hz4y5B/M9
g6LYCuo/fmVcuydfy/YJfTxBY+dBNbx2NM8h9TZ8qn/MX6zrjQkMNrw7vHKEDo81
Iyw9MU35cMXZ+BmfEzono+RZivZIpMof0pbiRGFGBalRzYLTOFMUqBWEVKD7Zb4f
uVqym56JZlJrqrWN9n4xQEhEWDr6MpeQL3TkdQIDAQABAoIBABHOKSvp6hKzGzzN
Q5c4FShKzVu+eiT5RwkMuZygXX50kV9C3PNgZHJ9234+BLWFdC1dBAio8bynrEu9
V/JL1uNHrg9ZFTw+79GJJO+Qk/iU/jbVpZgsFgFWJ8vrijuG3GGG0n0KNlgIvJbV
6EaioxyhWXpaj4HeylSb8xE4/WWSFLPOEw7+ltiySJJlwTdMRKLl8ggAvCSsS0pt
5ckfXs9vI5L5k6qospmOOacFuRHEfpkg6Qoz9dlnGCfLT5T6xC53IXkPqhmOAuJp
Uw59nv1cFMxepOA8C1pJWsVuLd6NDeZ1YNsN32LavxfVu34updjz+P+OfiSWrDz1
zvpQoAECgYEA9FYyrxQU0JS1D/8zNAFiwRxVlny8Fa54IsBgdnjo0MCO3C2udqnW
Lz1P1Iu7++wLnZIKhqSK/Z45lkGwLAHDUcgj+UBizHZA6nt/Ol9aQQfN6/8xaH6K
nWend9eWjaduuWmyxiAc7nMLr7ExHFAUWlyvp2AmwbxKEsWFU3ZnF/UCgYEA5Xf2
Clkua9IFdf+pJQX/Pdx5xiCn8o9b5si2tA6t0OD1IMU4B9xzsjWLtLmjOU3UDEUw
yBlL6Zlga7Gb3BMNmPU1tCUuX0fThVkjbSWPAle4JhyY6Jf1WN/RztfA601YGRIu
XKcj5S48p50RAYUxiVZzuug6OKjHWcxM29ohSoECgYEAyu5YlB7Klt4bb5jWTwgj
nB3LW/xnPl60erBqrZsISnDTdj0enIG8WWnDxtTzoW1PzGQCCAfbRH9IdupBxpsE
PQVME3UNWaGRku5VIgcV79LqjWpa/92xhMAVUCynZyk7hhUqnPEI9mZou1ggYNKF
xPMcFJiAOAIJR4PzkvlQ8I0CgYEAlMQJBvx6U/x8/dsZ1Z7lsz1U1VYmi+LVOfG2
QVWi6mBtiylRAgeyGj8Mq8YORABpEOtqjM+Zn4CIOkmTOkI/9oJCpt/UCIGQaEs5
5HENHf+wq0Su/VxG+69fXjISKHqkOtzGodraMqKfQCtb8xhe3SLnNK2J0WHanCUP
TxlDrAECgYAv1dlS9oo1ZTWQSQhPAsapvp9O3B2MyCcFzCP6hZ2LBmyxqtmIxguw
z+Je/zlDjJewcG/3XK77mTp71bZzxVCnp7Uxywn4jq+nAU0fW6cmg7m7hLQrtLC6
mvLB83uR7IYoi+KVMIjFj/yNeVwfWPddNE5tQjpCosPKDAuw8e4YVQ==
-----END RSA PRIVATE KEY-----
";
        class StaticPassFinder : Org.BouncyCastle.OpenSsl.IPasswordFinder
        {
            string m_Password;
            public StaticPassFinder(string pass)
            {
                m_Password = pass;
            }
            public char[] GetPassword()
            {
                return m_Password.ToCharArray();
            }
        }
        CommandLineApplication<RootApp> CreateApplication()
        {
            var services = new ServiceCollection();
            services.AddTransient<IConsole, PhysicalConsole>()
                .AddTransient<Org.BouncyCastle.OpenSsl.IPasswordFinder, StaticPassFinder>(provider => new StaticPassFinder("abcde"));
            var app = new CommandLineApplication<RootApp>();
            app.Conventions.UseDefaultConventions();
            app.Conventions.UseConstructorInjection(services.BuildServiceProvider());
            return app;
        }
    }
}
