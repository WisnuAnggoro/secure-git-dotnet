using Newtonsoft.Json;

namespace SecureGit.CryptoLibrary
{
    public class JsonLib
    {
        public JsonLib()
        {

        }

        public string Serialize<T>(
            T objectModel)
        {
            return JsonConvert.SerializeObject(objectModel);
        }

        public T Deserialize<T>(
            string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}