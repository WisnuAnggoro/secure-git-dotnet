using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SecureGit.CryptoLibrary.Extensions;
using SecureGit.RsaLibrary.Models;

namespace SecureGit.CryptoLibrary
{
    public class RsaLib
    {
        private int _keySize;
        private UnicodeEncoding ByteConverter;
        public string LastError { get; private set; }

        public RsaLib() : this(2048)
        {
        }

        public RsaLib(int KeySize)
        {
            _keySize = KeySize;

            //Create a UnicodeEncoder to convert between byte array and string.
            ByteConverter = new UnicodeEncoding();
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

        public string Encrypt(
            SecureString PlainSecureString,
            string PublicKey)
        {
            byte[] PlainBytes = StringUtility.StringByteArrayToByteArray(
                PlainSecureString.ToPlainString());

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromJsonString(PublicKey);
                
                byte[] EncryptedBytes = RSAEncrypt(
                    PlainBytes,
                    RSA.ExportParameters(false),
                    false);
                
                return StringUtility.ByteArrayToStringByteArray(
                    EncryptedBytes);
            }
        }

        public SecureString Decrypt(
            string CipherSecureString,
            SecureString PrivateKey)
        {
            // byte[] DecryptedBytes;
            // byte[] CipherBytes = ByteConverter.GetBytes(
            //     CipherSecureString);

            // byte[] DecryptedBytes = new byte[CipherSecureString.Length / 2];
            // byte[] CipherBytes = new byte[CipherSecureString.Length / 2];

            byte[] CipherBytes = StringUtility.StringByteArrayToByteArray(
                CipherSecureString);

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromJsonString(PrivateKey.ToPlainString());

                byte[] DecryptedBytes = RSADecrypt(
                    CipherBytes,
                    RSA.ExportParameters(true),
                    false);
                
                return StringUtility.ByteArrayToStringByteArray(
                    DecryptedBytes).ToSecureString();
            }

            // return ByteConverter
            //     .GetString(DecryptedBytes)
            //     .ToSecureString();
            
        }

        // For keys that will be saved in file
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
            catch (Exception ex)
            {
                LastError = ex.Message;
            }

            return boRet;
        }

        // For keys that never expose
        // Only live in memory
        public bool GenerateKeyPairs(
            out SecureString PrivateKey,
            out string PublicKey)
        {
            bool boRet = false;

            PrivateKey = new SecureString();
            PublicKey = null;

            try
            {
                var rsa = RSA.Create();
                rsa.KeySize = _keySize;

                PrivateKey = rsa.ToJsonSecureString(true);
                PublicKey = rsa.ToJsonString(false);
                
                boRet = true;
            }
            catch
            {
                PrivateKey = null;
                PublicKey = null;
            }

            return boRet;
        }

        // public string ExtractSecureString(
        //     SecureString secureString
        // )
        // {
        //     return secureString.ToPlainString();
        // }

        // public SecureString ConvertToSecureString(
        //     string plainString
        // )
        // {
        //     return plainString.ToSecureString();
        // }
    }

    internal static class RsaLibExtensions
    {
        #region JSON
        internal static void FromJsonString(this RSA rsa, string jsonString)
        {
            if (String.IsNullOrEmpty(jsonString))
                throw new ArgumentNullException();

            try
            {
                var paramsJson = JsonConvert.DeserializeObject<RsaParametersJson>(
                    jsonString);

                RSAParameters parameters = new RSAParameters();

                parameters.Modulus = paramsJson.Modulus != null ? Convert.FromBase64String(paramsJson.Modulus) : null;
                parameters.Exponent = paramsJson.Exponent != null ? Convert.FromBase64String(paramsJson.Exponent) : null;
                parameters.P = paramsJson.P != null ? Convert.FromBase64String(paramsJson.P) : null;
                parameters.Q = paramsJson.Q != null ? Convert.FromBase64String(paramsJson.Q) : null;
                parameters.DP = paramsJson.DP != null ? Convert.FromBase64String(paramsJson.DP) : null;
                parameters.DQ = paramsJson.DQ != null ? Convert.FromBase64String(paramsJson.DQ) : null;
                parameters.InverseQ = paramsJson.InverseQ != null ? Convert.FromBase64String(paramsJson.InverseQ) : null;
                parameters.D = paramsJson.D != null ? Convert.FromBase64String(paramsJson.D) : null;
                rsa.ImportParameters(parameters);
            }
            catch
            {
                throw new Exception("Invalid JSON RSA key.");
            }
        }

        internal static string ToJsonString(this RSA rsa, bool includePrivateParameters)
        {
            RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

            var paramsJson = new RsaParametersJson()
            {
                Modulus = parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
                Exponent = parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
                P = parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
                Q = parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
                DP = parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
                DQ = parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
                InverseQ = parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
                D = parameters.D != null ? Convert.ToBase64String(parameters.D) : null
            };

            return JsonConvert.SerializeObject(paramsJson, Formatting.Indented);
        }
        #endregion

        #region SecureString
        internal static SecureString ToJsonSecureString(
            this RSA rsa,
            bool includePrivateParameters)
        {
            return rsa
                .ToJsonString(includePrivateParameters)
                .ToSecureString();
        }
        #endregion

        #region XML

        // public static void FromXmlString(this RSA rsa, string xmlString)
        // {
        //     RSAParameters parameters = new RSAParameters();

        //     XmlDocument xmlDoc = new XmlDocument();
        //     xmlDoc.LoadXml(xmlString);

        //     if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
        //     {
        //         foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
        //         {
        //             switch (node.Name)
        //             {
        //                 case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
        //                 case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
        //                 case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
        //                 case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
        //                 case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
        //                 case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
        //                 case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
        //                 case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         throw new Exception("Invalid XML RSA key.");
        //     }

        //     rsa.ImportParameters(parameters);
        // }

        // public static string ToXmlString(this RSA rsa, bool includePrivateParameters)
        // {
        //     RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

        //     return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
        //           parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
        //           parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
        //           parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
        //           parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
        //           parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
        //           parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
        //           parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
        //           parameters.D != null ? Convert.ToBase64String(parameters.D) : null);
        // }

        #endregion
    }
}
