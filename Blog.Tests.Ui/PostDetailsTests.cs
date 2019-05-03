using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Linq;

namespace Blog.Tests.Ui
{
    [TestFixture]
    public class PostDetailsTests
    {
        private const string _url = "https://localhost:5001";
        private User normalUser;
        private User adminUser;

        public PostDetailsTests()
        {
            var settings = SettingsReader.GetGameConfig("testsettings.json");
            normalUser = settings.Users.First(u => !u.IsAdmin);
            adminUser = settings.Users.FirstOrDefault(u => u.IsAdmin);
        }

        [Test]
        public void All_Comments_Are_Displayed()
        {
            using (var driver =
                new ChromeDriver(ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory())))
            {
                driver.Url = _url + "/Home/Details/4080";
                var count = driver.FindElements(By.ClassName("comment")).Count;
                Assert.AreEqual(3, count);
                driver.Close();
            }
        }

    }
}