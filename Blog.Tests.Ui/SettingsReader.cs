using Newtonsoft.Json;
using System.IO;

namespace Blog.Tests.Ui
{
    public static class SettingsReader
    {
        public static TestsSettings GetGameConfig(string configPath)
        {

            using (StreamReader r = new StreamReader(configPath))
            {
                var json = r.ReadToEnd();
                var settings = JsonConvert.DeserializeObject<TestsSettings>(json);
                return settings;
            }
        }
    }


    public class TestsSettings
    {
        public User[] Users { get; set; }
    }

    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}