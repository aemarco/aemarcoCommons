using System.IO;
using Extensions.netExtensions;
using Newtonsoft.Json;

namespace Extensions.StorageExtensions
{
    public static class JsonStoreExtensions
    {

        public static T StoreFromFile<T>(this string filePath) 
            where T: new()
        {
            if (!File.Exists(filePath)) return new T();
            
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            catch
            {
                new FileInfo(filePath).TryDelete();
                return new T();
            }
        }

        public static void StoreToFile(this string filePath, object store)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(store, Formatting.Indented));
        }


    }
}
