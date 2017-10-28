using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace DMessenger
{
    public static class MessageEncoder
    {
        public static XElement Decode(Stream data)
        {
            var version = data.ReadByte();
            if (version < 0)
                throw new FormatException("データがありません");

            var buf = new byte[2];

            data.Read(buf, 0, 2);
            var binSize = BitConverter.ToUInt16(buf, 0); // メッセージデータ

            data.Read(buf, 0, 2);
            var pkeySize = BitConverter.ToUInt16(buf, 0); // 公開鍵

            data.Read(buf, 0, 2);
            var sigSize = BitConverter.ToUInt16(buf, 0); // 電子署名

            var gzbin = new byte[binSize];
            data.Read(gzbin, 0, binSize);

            var pkey = new byte[pkeySize];
            data.Read(pkey, 0, pkeySize);

            var sig = new byte[sigSize];
            data.Read(sig, 0, sigSize);

            var key = new RSAParameters()
            {
                Modulus = pkey,
                Exponent = new byte[] { 1, 0, 1 }
            };

            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(key);
                if (!rsa.VerifyData(gzbin, sig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                    throw new InvalidDataException("署名が正しくありません");
            }

            using (var ms1 = new MemoryStream(gzbin))
            using (var gz = new GZipStream(ms1, CompressionMode.Decompress))
            {
                return XElement.Load(gz);
            }
        }

        public static byte[] Encode(XElement message, RSA key)
        {
            var txt = message.ToString(SaveOptions.DisableFormatting);
            var bin = Encoding.UTF8.GetBytes(txt);

            using (var outms = new MemoryStream())
            {
                var gzdat = Compress(bin);
                var pkey = key.ExportParameters(false).Modulus;
                var sig = key.SignData(gzdat, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                outms.WriteByte(0); // バージョン番号
                outms.Write(BitConverter.GetBytes(checked((ushort)gzdat.Length)), 0, 2); // データサイズ
                outms.Write(BitConverter.GetBytes(checked((ushort)pkey.Length)), 0, 2); // 公開鍵サイズ
                outms.Write(BitConverter.GetBytes(checked((ushort)sig.Length)), 0, 2); // 署名サイズ
                outms.Write(gzdat, 0, gzdat.Length); // データ
                outms.Write(pkey, 0, pkey.Length); // 公開鍵
                outms.Write(sig, 0, sig.Length); // 署名

                return outms.GetBuffer();
            }
        }

        private static byte[] Decompress(byte[] dat)
        {
            using (var ms1 = new MemoryStream(dat))
            using (var gz = new GZipStream(ms1, CompressionMode.Decompress))
            using (var ms2 = new MemoryStream())
            {
                gz.CopyTo(ms2);
                return ms2.GetBuffer();
            }
        }

        private static byte[] Compress(byte[] dat)
        {
            using (var ms1 = new MemoryStream(dat))
            using (var ms2 = new MemoryStream())
            using (var gz = new GZipStream(ms2, CompressionMode.Compress))
            {
                ms1.CopyTo(gz);
                gz.Flush();
                return ms2.GetBuffer();
            }
        }
    }
}
