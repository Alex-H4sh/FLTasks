using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FLtasks
{
    class TaskFunctions
    {
        public static void login(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://localhost/litecart/admin");
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Click();
            IWebElement success = driver.FindElement(By.CssSelector(".notice.success"));
            Assert.IsTrue(success.Text.Equals("You are now logged in as admin"), "You are admin");
            driver.Quit();
        }
        public static string[] color_value(IWebElement el, string xp_)
        {
            string orig_price_norm_c = el.FindElement(By.XPath(xp_)).GetCssValue("color");
            string[] delimiterChars = { "rgba(", ")", ", " };
            string[] colors = orig_price_norm_c.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);
            return colors;
        }
        public static string[] color_value(IWebDriver el, string xp_)
        {
            string orig_price_norm_c = el.FindElement(By.XPath(xp_)).GetCssValue("color");
            string[] delimiterChars = { "rgba(", ")", ", " };
            string[] colors = orig_price_norm_c.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);
            return colors;
        }
        public static void unhide(IWebDriver driver, IWebElement element)
        {
            String script = "arguments[0].style.opacity=1;"
              + "arguments[0].style['transform']='translate(0px, 0px) scale(1)';"
              + "arguments[0].style['MozTransform']='translate(0px, 0px) scale(1)';"
              + "arguments[0].style['WebkitTransform']='translate(0px, 0px) scale(1)';"
              + "arguments[0].style['msTransform']='translate(0px, 0px) scale(1)';"
              + "arguments[0].style['OTransform']='translate(0px, 0px) scale(1)';"
              + "return true;";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(script, element);
        }
        public static string gen_email(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghiklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()) + "@gmail.com";
        }
    }

    [TestClass]
    public class FLTasks
    {
        static IWebDriver driver;
        static IWebDriver f_driver;
        static IWebDriver ie_driver;

        [TestMethod]
        public void Task1()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.google.com");
            Assert.IsTrue(driver.Url.Contains("google.com"), "Wrong site");
            driver.Close();
        }

        [TestMethod]
        public void Task3()
        {
            driver = new ChromeDriver();
            TaskFunctions.login(driver);
            driver.Close();
        }

        [TestMethod]
        public void Task4()
        {
            // Chrome
            driver = new ChromeDriver();
            TaskFunctions.login(driver);
            driver.Close();

            // Old Firefox
            FirefoxOptions old_options = new FirefoxOptions();
            old_options.BrowserExecutableLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe"; // geckdriver 0.20, FireFox 45.5
            old_options.UseLegacyImplementation = true;
            f_driver = new FirefoxDriver(old_options);
            f_driver.Navigate().GoToUrl("https://localhost/litecart/admin");
            TaskFunctions.login(f_driver);
            f_driver.Quit();

            // IE
            ie_driver = new InternetExplorerDriver();
            TaskFunctions.login(ie_driver);
            ie_driver.Quit();
        }

        [TestMethod]
        public void Task6()
        {
            //Firefox nightly
            FirefoxOptions options = new FirefoxOptions();
            options.AcceptInsecureCertificates = true;
            options.BrowserExecutableLocation = @"C:\Program Files\Nightly\firefox.exe"; // geckdriver 0.20, Nightly 56
            f_driver = new FirefoxDriver(options);
            f_driver.Navigate().GoToUrl("https://localhost/litecart/admin");
            TaskFunctions.login(f_driver);
            f_driver.Quit();
        }

        [TestMethod]
        public void Task7()
        {
            driver = new ChromeDriver();
            TaskFunctions.login(driver);

            int size = driver.FindElements(By.XPath("//ul[@id='box-apps-menu']/li/a")).Count;
            for (int i = 0; i < size; i++)
            {
                IList<IWebElement> menu2 = driver.FindElements(By.XPath("//ul[@id='box-apps-menu']/li/a"));
                menu2[i].Click();

                if ((driver.FindElements(By.XPath("//ul[@class='docs']/li/a")).Count) > 0)
                {
                    for (int k = 0; k < (driver.FindElements(By.XPath("//ul[@class='docs']/li/a")).Count); k++)
                    {
                        IList<IWebElement> a = driver.FindElements(By.XPath("//ul[@class='docs']/li/a"));
                        if (k <= (a.Count - 1))
                        {
                            a[k].Click();
                            Assert.IsTrue(driver.FindElements(By.TagName("h1")).Count > 0, driver.Title + " doesnt have h1");
                        }
                    }
                }
            }
            driver.Close();
        }

        [TestMethod]
        public void Task8()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://localhost/litecart/");
            IList<IWebElement> ducks = driver.FindElements(By.XPath("//ul//a[@class='link']"));
            foreach (IWebElement el in ducks)
            {
                bool check = false;
                if (el.FindElements(By.XPath(".//div[contains(@class,'sticker')]")).Count == 1)
                    check = true;
                Assert.IsTrue(check, el.Text + " no stickers");
            }
            driver.Close();
        }

        [TestMethod]
        public void Task9()
        {
            driver = new ChromeDriver();
            TaskFunctions.login(driver);

            // Step 1-1a
            driver.Navigate().GoToUrl("http://localhost/litecart/admin/?app=countries&doc=countries");
            IList<IWebElement> rows = driver.FindElements(By.XPath("//table[@class='dataTable']//tr[@class='row']"));
            List<string> x = new List<string>();
            List<string> multi_tz_links = new List<string>();
            foreach (IWebElement el in rows)
            {
                IWebElement td = el.FindElement(By.XPath("./td[5]//a"));
                x.Add(td.Text);
                string s = el.FindElement(By.XPath("./td[6]")).Text;
                if (s != "0")
                {
                    multi_tz_links.Add(td.GetAttribute("href"));
                }
            }
            List<string> y = new List<string>(x);
            x.Sort();
            Assert.IsFalse(!(y.SequenceEqual(x)), "Wrong order!");

            // Step 1b
            foreach (string href in multi_tz_links)
            {
                driver.Navigate().GoToUrl(href);
                List<string> u = new List<string>();
                IList<IWebElement> in_rows = driver.FindElements(By.XPath("//table[@class='dataTable']//tr//td[3]//input"));
                foreach (IWebElement ch in in_rows)
                {
                    u.Add(ch.Text);
                }

                List<string> z = new List<string>(u);
                u.Sort();
                Assert.IsFalse(!(z.SequenceEqual(u)), "Wrong order!");
            }

            // Step 2
            driver.Navigate().GoToUrl("http://localhost/litecart/admin/?app=geo_zones&doc=geo_zones");
            IList<IWebElement> countries = driver.FindElements(By.XPath("//table[@class='dataTable']//tr//td[3]/a"));
            List<string> link = new List<string>();

            foreach (IWebElement con in countries)
            {
                link.Add(con.GetAttribute("href"));
            }

            foreach (string href in link)
            {
                driver.Navigate().GoToUrl(href);
                List<string> u = new List<string>();
                IList<IWebElement> in_rows = driver.FindElements(By.XPath("//table[@id='table-zones']//tr//td[3]//select//option[@selected='selected']"));
                foreach (IWebElement ch in in_rows)
                {
                    u.Add(ch.Text);
                    System.Console.WriteLine(ch.Text);
                }

                List<string> z = new List<string>(u);
                u.Sort();
                Assert.IsFalse(!(z.SequenceEqual(u)), "Wrong order!");
            }
            driver.Close();
        }

        [TestMethod]
        public void Task10()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://localhost/litecart/");
            IWebElement duck = driver.FindElement(By.XPath("//div[@id='box-campaigns']//li"));

            // a) на главной странице и на странице товара совпадает текст названия товара
            string orig_name = duck.FindElement(By.XPath(".//div[@class='name']")).Text;
            // б) на главной странице и на странице товара совпадают цены(обычная и акционная)
            string orig_price_norm = duck.FindElement(By.XPath(".//*[@class='regular-price']")).Text;
            string orig_price_sale = duck.FindElement(By.XPath(".//*[@class='campaign-price']")).Text;

            // Проверка серого цвета цены
            string[] orig_pr_n_c = TaskFunctions.color_value(duck, ".//*[@class='regular-price']");
            Assert.IsFalse(!((orig_pr_n_c[0].Equals(orig_pr_n_c[1])) && (orig_pr_n_c[1].Equals(orig_pr_n_c[2]))), "Orig price is not gray!");

            // Проверка красного цвета акцонной цены
            string[] orig_pr_s_c = TaskFunctions.color_value(duck, ".//*[@class='campaign-price']");
            Assert.IsFalse(!(!(orig_pr_s_c[0].Equals("0")) && (orig_pr_s_c[1].Equals(orig_pr_s_c[2])) && (orig_pr_s_c[1].Equals("0"))), "Sale price is not red!");

            // Проверка жирности шрифта акционной цены
            int orig_pr_s_bl = Int32.Parse(duck.FindElement(By.XPath(".//*[@class='campaign-price']")).GetCssValue("font-weight"));
            Assert.IsFalse(orig_pr_s_bl < 700, "Sale price is not bold");

            // Проверка перечеркнутости цены
            string orig_pr_n_dec = duck.FindElement(By.XPath(".//*[@class='regular-price']")).GetCssValue("text-decoration-line");
            Assert.IsFalse(!(orig_pr_n_dec.Equals("line-through")), "Price is not crossed");

            // Шрифт акционной цены должен быть больше
            string orig_pr_s_fs = duck.FindElement(By.XPath(".//*[@class='campaign-price']")).GetCssValue("font-size");
            orig_pr_s_fs = orig_pr_s_fs.Substring(0, 2);
            string orig_pr_n_fs = duck.FindElement(By.XPath(".//*[@class='regular-price']")).GetCssValue("font-size");
            orig_pr_n_fs = orig_pr_n_fs.Substring(0, 2);
            Assert.IsFalse(Int32.Parse(orig_pr_n_fs) >= Int32.Parse(orig_pr_s_fs), "Wrong font size of proces!");

            // На новой странице
            duck.FindElement(By.XPath("./a")).Click();
            string new_name = driver.FindElement(By.XPath("//h1[@class='title']")).Text;

            string new_price_norm = driver.FindElement(By.XPath("//*[@class='regular-price']")).Text;
            string new_price_sale = driver.FindElement(By.XPath("//*[@class='campaign-price']")).Text;

            Assert.IsFalse(!new_name.Equals(orig_name), "Names are different");

            Assert.IsFalse(!new_price_norm.Equals(orig_price_norm), "Prices are different");
            Assert.IsFalse(!new_price_sale.Equals(orig_price_sale), "Sale prices are different");

            string[] new_pr_n_c = TaskFunctions.color_value(driver, "//*[@class='regular-price']");
            Assert.IsFalse(!((new_pr_n_c[0].Equals(new_pr_n_c[1])) && (new_pr_n_c[1].Equals(new_pr_n_c[2]))), "Orig price is not gray!");

            // Проверка красного цвета акцонной цены
            string[] new_pr_s_c = TaskFunctions.color_value(driver, "//*[@class='campaign-price']");
            Assert.IsFalse(!(!(new_pr_s_c[0].Equals("0")) && (new_pr_s_c[1].Equals(new_pr_s_c[2])) && (new_pr_s_c[1].Equals("0"))), "Sale price is not red!");

            // Проверка жирности шрифта акционной цены
            int new_pr_s_bl = Int32.Parse(driver.FindElement(By.XPath("//*[@class='campaign-price']")).GetCssValue("font-weight"));
            Assert.IsFalse(new_pr_s_bl < 700, "Sale price is not bold");

            // Проверка перечеркнутости цены
            string new_pr_n_dec = driver.FindElement(By.XPath("//*[@class='regular-price']")).GetCssValue("text-decoration-line");
            Assert.IsFalse(!(new_pr_n_dec.Equals("line-through")), "Price is not crossed");

            // Шрифт акционной цены должен быть больше
            string new_pr_s_fs = driver.FindElement(By.XPath("//*[@class='campaign-price']")).GetCssValue("font-size");
            new_pr_s_fs = new_pr_s_fs.Substring(0, 2);
            string new_pr_n_fs = driver.FindElement(By.XPath("//*[@class='regular-price']")).GetCssValue("font-size");
            new_pr_n_fs = new_pr_n_fs.Substring(0, 2);
            Assert.IsFalse(Int32.Parse(new_pr_n_fs) >= Int32.Parse(new_pr_s_fs), "Wrong font size of proces!");
            driver.Close();
        }

        [TestMethod]
        public void Task11()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://localhost/litecart/");
            IWebElement reg = driver.FindElement(By.XPath("//form[@name='login_form']//tr[5]/td/a"));
            reg.Click();

            IWebElement list = driver.FindElement(By.XPath("//select[@name='country_code']"));
            TaskFunctions.unhide(driver, list);
            IWebElement usa = list.FindElement(By.XPath(".//option[contains(text(),'United States')]"));
            usa.Click();

            driver.FindElement(By.XPath("//button[@name='create_account']")).Submit();

            driver.FindElement(By.XPath("//input[@name='firstname']")).SendKeys("Jho");
            driver.FindElement(By.XPath("//input[@name='lastname']")).SendKeys("Smith");
            driver.FindElement(By.XPath("//input[@name='address1']")).SendKeys("Times Garden");
            driver.FindElement(By.XPath("//input[@name='postcode']")).SendKeys("55555");

            driver.FindElement(By.XPath("//input[@name='email']")).SendKeys(TaskFunctions.gen_email(10));
            driver.FindElement(By.XPath("//input[@name='phone']")).SendKeys("+18934284903");

            driver.FindElement(By.XPath("//input[@name='password']")).SendKeys("root");
            driver.FindElement(By.XPath("//input[@name='confirmed_password']")).SendKeys("root");

            driver.FindElement(By.XPath("//input[@name='city']")).SendKeys("California");
            driver.FindElement(By.XPath("//button[@name='create_account']")).Submit();
            driver.Close();
        }
    }
}
