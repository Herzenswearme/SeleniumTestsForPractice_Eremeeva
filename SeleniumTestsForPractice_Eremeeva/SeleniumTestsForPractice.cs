using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTestsForPractice_Eremeeva;

public class SeleniumTestsForPractice
{
    public ChromeDriver driver;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        
        // Авторизация
        Authorization();
    }

    [Test]
    public void UrlComparison()
    {
        var news = driver.FindElement(By.CssSelector("[data-tid = 'Title']"));
        var currentUrl = driver.Url;
        currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
    }

    [Test]
    public void NavigationTest()
    {
        // клик на боковее меню
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();
        // клик на сообщества
        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();
        // проверяем, что Сообщества есть на старнице + урл правильный
        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        Assert.That(driver.Url == "https://staff-testing.testkontur.ru/communities", "На странице 'Сообщества' на найдено сообществ");
    } 
    
    public void Authorization()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("user");

        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("1q2w3e4r%T");
        
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
    }

    [TearDown]
    public void TearDown()
    {
        //закрываем браузер и убиваем процесс драйвера
        driver.Quit();
    }

}
