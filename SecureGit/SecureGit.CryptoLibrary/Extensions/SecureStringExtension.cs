using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SecureGit.CryptoLibrary.Extensions
{
    public static class SecureStringExtension
    {
        public static SecureString ToSecureString(
            this string plainString)
        {
            if (plainString == null)
                return null;

            SecureString secureString = new SecureString();
            foreach (char c in plainString.ToCharArray())
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }

        public static string ToPlainString(
            this SecureString secureString
        )
        {
            string plainString = "";

            IntPtr bstr = Marshal.SecureStringToBSTR(
                secureString);

            try
            {
                plainString = Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }

            return plainString;
        }
    }
}