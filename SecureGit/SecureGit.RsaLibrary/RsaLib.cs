using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecureGit.RsaLibrary
{
    public class RsaLib
    {
        private int _keySize;
        private UnicodeEncoding ByteConverter;
        public string LastError { get; private set; }

        public RsaLib() : this(4096)
        {
        }

        public RsaLib(int KeySize)
        {
            _keySize = KeySize;

            //Create a UnicodeEncoder to convert between byte array and string.
            ByteConverter = new UnicodeEncoding();
        }

        // public void Main()
        // {
        //     try
        //     {
        //         //Create a UnicodeEncoder to convert between byte array and string.
        //         UnicodeEncoding ByteConverter = new UnicodeEncoding();

        //         //Create byte arrays to hold original, encrypted, and decrypted data.
        //         byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");
        //         byte[] encryptedData;
        //         byte[] decryptedData;

        //         //Create a new instance of RSACryptoServiceProvider to generate
        //         //public and private key data.
        //         using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
        //         {

        //             //Pass the data to ENCRYPT, the public key information 
        //             //(using RSACryptoServiceProvider.ExportParameters(false),
        //             //and a boolean flag specifying no OAEP padding.
        //             encryptedData = RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);

        //             //Pass the data to DECRYPT, the private key information 
        //             //(using RSACryptoServiceProvider.ExportParameters(true),
        //             //and a boolean flag specifying no OAEP padding.
        //             decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

        //             //Display the decrypted plaintext to the console. 
        //             // Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));
        //         }
        //     }
        //     catch (ArgumentNullException)
        //     {
        //         //Catch this exception in case the encryption did
        //         //not succeed.
        //         LastError = "Encryption failed.";

        //     }
        // }

        public byte[] RSAEncrypt(
            byte[] DataToEncrypt,
            RSAParameters RSAKeyInfo,
            bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }

                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                LastError = e.Message;
                return null;
            }

        }

        public byte[] RSADecrypt(
            byte[] DataToDecrypt,
            RSAParameters RSAKeyInfo,
            bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }

                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                LastError = e.Message;
                return null;
            }

        }

        public string Encrypt(
            string PlainText,
            string PrivateKey)
        {
            byte[] EncryptedBytes;
            byte[] PlainBytes = ByteConverter.GetBytes(
                PlainText);

            using(RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromJsonString(PrivateKey);

                EncryptedBytes = RSAEncrypt(
                    PlainBytes, 
                    RSA.ExportParameters(false),
                    false);
            }

            return ByteConverter.GetString(EncryptedBytes);
        }

        public string Decrypt(
            string CipherText,
            string PublicKey)
        {
            byte[] DecryptedBytes;
            byte[] CipherBytes = ByteConverter.GetBytes(
                CipherText);

            using(RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromJsonString(PublicKey);

                DecryptedBytes = RSAEncrypt(
                    CipherBytes, 
                    RSA.ExportParameters(true),
                    false);
            }

            return ByteConverter.GetString(DecryptedBytes);
        }

        public bool GenerateKeyPairs(
            string OutputPath,
            string PrivateKeyName,
            string PublicKeyName)
        {
            bool boRet = false;

            try
            {
                var rsa = RSA.Create();
                rsa.KeySize = _keySize;
                var privateKey = rsa.ToJsonString(true);
                var publicKey = rsa.ToJsonString(false);

                if (!Directory.Exists(OutputPath))
                    Directory.CreateDirectory(OutputPath);

                File.WriteAllText(
                    Path.Combine(
                        OutputPath, 
                        PrivateKeyName),
                    privateKey);

                File.WriteAllText(
                    Path.Combine(
                        OutputPath, 
                        PublicKeyName),
                    publicKey);

                boRet = true;
            }
            catch(Exception ex)
            {
                LastError = ex.Message;
            }

            return boRet;
        }
    }
}
