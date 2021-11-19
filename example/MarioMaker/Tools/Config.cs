using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MarioMaker
{
    public class Config
    {
        private string _filePath;

        public Config(string filePath)
        {
            _filePath = filePath;
        }

        public void Save(string key, string value)
        {
            JObject jo = new JObject();
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);

                jo = JsonConvert.DeserializeObject<JObject>(json);
            }
            jo.Add(key, value);
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(jo, Formatting.Indented));
        }

        public string Read(string key)
        {
            JObject jo = new JObject();
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);

                jo = JsonConvert.DeserializeObject<JObject>(json);
            }

            if (jo[key] == null)
            {
                return null;
            }
            else
            {
                return jo[key].ToString();
            }
        }
    }
}