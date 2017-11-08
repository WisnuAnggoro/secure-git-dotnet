using Newtonsoft.Json;

namespace SecureGit.ConsoleApp.Helpers
{
    public static class DataHelper
    {
        public static bool GetJsonStringFromObject(
            object obj,
            out string strOutput
        )
        {
            bool boRet = false;
            

            try
            {
                strOutput = JsonConvert.SerializeObject(obj);
            }
            catch
            {
                strOutput = "";
            }

            return boRet;
        }
    }
}