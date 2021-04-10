using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SpisSekcjiManager.Utils
{
    public static class FacebookExtensions
    {
        public static void FacebookLogin(this ChromeDriver chromeDriver, User user)
        {
            chromeDriver.Navigate().GoToUrl("https://mbasic.facebook.com/login.php/");
            chromeDriver.FindElement(By.CssSelector("button.t")).Click();
            Thread.Sleep(500);
            chromeDriver.FindElement(By.CssSelector("input[name='email']")).SendKeys(user.Email);
            chromeDriver.FindElement(By.CssSelector("input[name='pass']")).SendKeys(user.Password);
            chromeDriver.FindElement(By.CssSelector("input[name='login']")).Click();
        }
    }
}