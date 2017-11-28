using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChronoBot.Services
{
    /// <summary>
    /// Provides simple storage of data as JSON
    /// Eventually to be replaced with a database
    /// </summary>
    public class LocalStorage
    {
        private readonly string dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        public LocalStorage()
        {
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);
        }

        public async Task WriteData<T>(T data, string fileName)
        {
            string filePath = GetFilePath(fileName);

            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(data));
        }

        public async Task<T> ReadData<T>(string fileName)
        {
            string filePath = GetFilePath(fileName);

            if (File.Exists(filePath))
                return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(filePath));
            else
                return default(T);
        }

        private string GetFilePath(string fileName)
        {
            return string.Concat(Path.Combine(dataFolder, fileName), ".json");
        }
    }
}
