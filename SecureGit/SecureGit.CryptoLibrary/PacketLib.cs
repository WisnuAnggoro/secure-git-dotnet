using System;
using System.Security;
using SecureGit.CryptoLibrary.Models;

namespace SecureGit.CryptoLibrary
{
    public class PacketLib
    {
        private AesLib _aes;
        private RsaLib _rsa;
        private RngLib _rng;
        private JsonLib _jsonLib;
        public string LastError { get; private set; }

        public PacketLib()
        {
            _aes = new AesLib();
            _rsa = new RsaLib();
            _rng = new RngLib();
            _jsonLib = new JsonLib();
        }

        public Packet WrapPacket(
            SecureString PlainPayload,
            string RsaPublicKey)
        {
            try
            {
                // Generate AES Key
                SecureString aesKey = _rng.GenerateRandomSecureString(
                    _aes.KeySize);

                // Encrypt Plain Payload
                string encryptedPayload = _aes.Encrypt(
                    PlainPayload,
                    aesKey);

                // Encrypt AES Key
                string aesKeyEncrypted = _rsa.Encrypt(
                    aesKey,
                    RsaPublicKey);
                
                // Return packet
                return new Packet()
                {
                    Header = aesKeyEncrypted,
                    Payload = encryptedPayload
                };
            }
            catch(Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        public SecureString UnwrapPacket(
            Packet packet,
            SecureString RsaPrivateKey)
        {
            try
            {
                // Unwrap AES Key
                SecureString aesKey = _rsa.Decrypt(
                    packet.Header,
                    RsaPrivateKey);

                // Decrypt and return Payload
                return _aes.Decrypt(
                    packet.Payload,
                    aesKey);

            }
            catch(Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        public SecureString UnwrapPacket(
            string PacketString,
            SecureString RsaPrivateKey)
        {
            Packet packet = _jsonLib.Deserialize<Packet>(PacketString);
            return UnwrapPacket(
                packet,
                RsaPrivateKey);
        }

        public bool IsValidRsaPublicKey(
            string RsaPublicKey)
        {
            try
            {
                // Try to encrypt 32 byte random characters
                // (the result is ignored)
                _rsa.Encrypt(
                    _rng.GenerateRandomSecureString(32),
                    RsaPublicKey);

                // If success (and no exception is thrown), 
                // the key must be valid
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}