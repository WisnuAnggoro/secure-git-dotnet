using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecureGit.Logics
{
    public class RsaLib
    {
        public string LastError { get; private set; }

        public RsaLib()
        {
        }

        public void Main()
        {
            try
            {
                //Create a UnicodeEncoder to convert between byte array and string.
                UnicodeEncoding ByteConverter = new UnicodeEncoding();

                //Create byte arrays to hold original, encrypted, and decrypted data.
                byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");
                byte[] encryptedData;
                byte[] decryptedData;

                //Create a new instance of RSACryptoServiceProvider to generate
                //public and private key data.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Pass the data to ENCRYPT, the public key information 
                    //(using RSACryptoServiceProvider.ExportParameters(false),
                    //and a boolean flag specifying no OAEP padding.
                    encryptedData = RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);

                    //Pass the data to DECRYPT, the private key information 
                    //(using RSACryptoServiceProvider.ExportParameters(true),
                    //and a boolean flag specifying no OAEP padding.
                    decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

                    //Display the decrypted plaintext to the console. 
                    // Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));
                }
            }
            catch (ArgumentNullException)
            {
                //Catch this exception in case the encryption did
                //not succeed.
                // Console.WriteLine("Encryption failed.");
                string s = "a";

            }
        }

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

        public void GenerateKeyPairs()
        {
            var rsa = RSA.Create();
            rsa.KeySize = 4096;
            var privateKey = rsa.ToJsonString(true);
            var publicKey = rsa.ToJsonString(false);
            File.WriteAllText("private.json", privateKey);
            File.WriteAllText("public.json", publicKey);
        }
    }
}
