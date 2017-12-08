using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecureGit.CryptoLibrary
{
    public static class StringUtility
    {
        public static string RemoveNonHexa(
            string str)
        {
            int i = 0;
            string sRet = string.Empty;

            if (str == string.Empty) return sRet;

            str = str.ToUpper();
            for (i = 0; i < str.Length; i++)
            {
                if ("0123456789ABCDEF".Contains(str[i])) sRet += str[i];
            }

            return sRet;
        }
        public static byte StringByteToByte(string strByte)
        {
            return Convert.ToByte(strByte, 16);
        }

        public static byte[] StringByteArrayToByteArray(
            string StringByteArray)
        {
            int i, j;
            string sTmp;
            StringByteArray = RemoveNonHexa(StringByteArray);
            int StringLength = StringByteArray.Length % 2 == 0 ?
                StringByteArray.Length :
                StringByteArray.Length - 1;
            byte[] ByteArray = new byte[StringLength / 2];
            
            for (i = 0, j = 0; i < StringLength; i += 2, ++j)
            {
                sTmp = StringByteArray.Substring(i, 2);
                ByteArray[j] = StringByteToByte(sTmp);
            }

            return ByteArray;
        }

        public static string ByteArrayToStringByteArray(
            byte[] ByteArray,
            int? ArrayLength = null)
        {
            string sRet = String.Empty;
            string sTmp;

            if (ByteArray != null && ByteArray.Length > 0)
            {
                if (ArrayLength == null || ArrayLength > ByteArray.Length)
                    ArrayLength = ByteArray.Length;

                for (int i = 0; i < ArrayLength; ++i)
                {
                    sTmp = ByteArray[i].ToString("X");
                    if (sTmp.Length == 1) sTmp = "0" + sTmp;
                    sRet += sTmp;
                }
            }

            return sRet;
        }
    }
}