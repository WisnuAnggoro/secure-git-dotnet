using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using SecureGit.CryptoLibrary.Extensions;

namespace SecureGit.CryptoLibrary
{
    public class RngLib
    {
        private RNGCryptoServiceProvider _rngCsp;
        public string LastError { get; private set; }        
        private UnicodeEncoding ByteConverter;

        public RngLib()
        {
            _rngCsp = new RNGCryptoServiceProvider();
            
            //Create a UnicodeEncoder to convert between byte array and string.
            ByteConverter = new UnicodeEncoding();
        }

        public byte[] GenerateRandomBytes(
            int LengthInByte)
        {
            try
            {
                byte[] result = new byte[LengthInByte];
                _rngCsp.GetNonZeroBytes(result);
                return result;
            }
            catch(Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        public SecureString GenerateRandomSecureString(
            int LengthInByte)
        {
            // return ByteConverter.GetString(
            //     GenerateRandomBytes(
            //         LengthInByte))
            //         .ToSecureString();

            byte[] b = GenerateRandomBytes(
                LengthInByte);
            
            string s = StringUtility.ByteArrayToStringByteArray(
                b);
            
            SecureString ss = s.ToSecureString();

            return ss;
            
            // return StringUtility.ByteArrayToStrByteArray(
            //     b,
            //     (ushort)b.Length)
            //         .ToSecureString();
        }
    }
}