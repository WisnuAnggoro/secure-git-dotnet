using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using SecureGit.CryptoLibrary.Extensions;

namespace SecureGit.CryptoLibrary
{
    public class AesLib
    {
        private int _keySizeInBits;
        private UnicodeEncoding ByteConverter;
        public int KeySize { get; private set; }
        public string LastError { get; private set; }

        public AesLib() : this(128)
        {
        }

        public AesLib(int KeySizeInBits)
        {
            _keySizeInBits = KeySizeInBits;

            KeySize = KeySizeInBits / 8;

            //Create a UnicodeEncoder to convert between byte array and string.
            ByteConverter = new UnicodeEncoding();
        }

        public byte[] EncryptStringToBytes(
            SecureString PlainText,
            byte[] Key)
        {
            // Check arguments.
            if (PlainText == null || PlainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = new byte[]
                {
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                };

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(
                    aesAlg.Key,
                    aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = 
                        new CryptoStream(
                            msEncrypt, 
                            encryptor, 
                            CryptoStreamMode.Write))
                    {
                        string s = PlainText.ToPlainString();
                        using (StreamWriter swEncrypt = 
                            new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(
                                s);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        public SecureString DecryptStringFromBytes(
            byte[] cipherText,
            byte[] Key)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            SecureString plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = new byte[]
                {
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                };


                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(
                    aesAlg.Key,
                    aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(
                        msDecrypt,
                        decryptor,
                        CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd().ToSecureString();
                        }
                    }
                }

            }

            return plaintext;
        }

        public string Encrypt(
            SecureString PlainSecureString,
            SecureString KeyInBytes)
        {
            try
            {
                string s = KeyInBytes.ToPlainString();
                byte[] b = StringUtility.StringByteArrayToByteArray(
                    s);

                byte[] encrypted = EncryptStringToBytes(
                    PlainSecureString,                    
                    b);

                return StringUtility.ByteArrayToStringByteArray(
                    encrypted,
                    (ushort)encrypted.Length);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        public SecureString Decrypt(
            string CipherSecureString,
            SecureString KeyInBytes)
        {
            try
            {
                // byte[] CipherInByteArray = new byte[CipherSecureString.Length / 2];
                // byte[] KeyInByteArray = new byte[keyInBytes.ToPlainString().Length / 2];

                byte[] CipherInByteArray =  StringUtility.StringByteArrayToByteArray(
                    CipherSecureString);
                byte[] KeyInByteArray = StringUtility.StringByteArrayToByteArray(
                    KeyInBytes.ToPlainString());

                return DecryptStringFromBytes(
                    CipherInByteArray,
                    KeyInByteArray);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }
    }
}