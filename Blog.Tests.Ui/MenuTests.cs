using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Blog.Tests.Ui
{
    [TestFixture]
    public class MenuTests
    {
        private const string _url = "https://localhost:5001";
        private User normalUser;
        private User adminUser;

        public MenuTests()
        {
            var settings = SettingsReader.GetGameConfig("testsettings.json");
            normalUser = settings.Users.First(u => !u.IsAdmin);
            adminUser = settings.Users.FirstOrDefault(u => u.IsAdmin);
        }
       
        [Test]
        public void Click_link_in_menu_navbar()
        {
            using (var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory())))
            {
                driver.Url = _url;
                driver.Manage().Window.Maximize();
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 1, 0));
                wait.Until(w => w.FindElement(By.XPath(".//*[@id='lnkHome']")));
                var link = driver.FindElement(By.XPath(".//*[@id='lnkHome']"));
                Actions actions = new Actions(driver);
                actions.MoveToElement(link).Click().Perform();
                driver.Close();
            }
        }

        [Test]
        public void Login_with_okta_account()
        {
            using (var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory())))
            {
                driver.Url = _url+"/Account/Login";
                driver.Manage().Window.Maximize();
                Actions actions = new Actions(driver);
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 1, 0));

                wait.Until(w => w.FindElement(By.Id("okta-signin-username")));
                var login = driver.FindElement(By.XPath(".//*[@id='okta-signin-username']"));
                var password = driver.FindElement(By.XPath(".//*[@id='okta-signin-password']"));
                var btnSubmit = driver.FindElement(By.XPath(".//*[@id='okta-signin-submit']"));
                login.SendKeys(normalUser.Login);
                password.SendKeys(normalUser.Password);
                actions.MoveToElement(btnSubmit).Click().Perform();
                driver.Close();
            }
        }     
    }
}