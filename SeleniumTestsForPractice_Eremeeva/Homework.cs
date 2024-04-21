using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTestsForPractice_Eremeeva;

public class Homework
{
    public ChromeDriver driver;

    public string MainUrl = "https://staff-testing.testkontur.ru";
    
    public void Authorization(string user, string password)
    {
        driver.Navigate().GoToUrl(MainUrl);
        
        driver.FindElement(By.Id("Username")).SendKeys(user);
        driver.FindElement(By.Name("Password")).SendKeys(password);
        
        var signIn = driver.FindElement(By.Name("button"));
        signIn.Click();
        
        // добавила для загрузки страницы. Потому что, если в начале теста сразу переходить по урл, команда GoToUrl не успевает выполниться 
        //явное ожидание видимости меню профиля
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ProfileMenu']")));
    }

    public void ProfileMenuClick()
    {
        // Нажать на меню профиля 
        var profileMenu = driver.FindElement(By.CssSelector("[data-tid='ProfileMenu']"));
        profileMenu.Click();
    }

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions", "--enable-logging");
        
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        
        Authorization("herzenswearme@gmail.com", "1q2w3e4r%T");
    }

    [Test]
    // Проверка выхода из учетной записи
    public void ProfileLogout()
    {
        ProfileMenuClick();
        // 2. Нажать в меню на "Выйти"
        var logoutButton = driver.FindElement(By.CssSelector("[data-tid='Logout']"));
        logoutButton.Click();
        
        // 3. Проверяем, что действительно вышли
        var logoutMessage = driver.FindElement(By.XPath("//h3[contains(text(), 'Вы вышли из учетной записи')]")).Text;
        Assert.That(logoutMessage == "Вы вышли из учетной записи", "Выход из учётной записи не выполнен");
    }

    [Test]
    // Проверка открытия страницы диалога с сотрудником
    public void OpeningUserChat()
    {
        // 1. Перейти на страницу "Диалоги"
        driver.Navigate().GoToUrl(MainUrl + "/messages");
        // 2. Нажать на диалог с сотрудником
        var employeeDialog = driver.FindElement(By.CssSelector("a[href*='/messages/b0e38a1c-ec65-489f-b334-d94cd1ca7cfc']"));
        employeeDialog.Click();
        
        // 3. Проверить, что страница диалога открылось
        var message = driver.FindElement(By.XPath("//div[contains(text(), 'привет)')]")).Text;
        Assert.That(message == "привет)", "Диалог с сотрудником не открывается");
    }

    [Test]
    // Проверка кликабельности тогла "Только мои" на странице "Мероприятия"
    public void ClickableOnlyMyEventsToggle()
    {
        // 1. Перейти на страницу "Мероприятия"
        driver.Navigate().GoToUrl(MainUrl + "/events");
        // 2. Нажать на тогл "Только мои"
        var onlyMyToggle = driver.FindElement(By.CssSelector("[data-tid='OnlyMyRadio']"));
        onlyMyToggle.Click();
        
        // 3. Проверить активность тогла
        driver.FindElement(By.CssSelector("[data-tid='OnlyMyRadio'] input"))
            .GetAttribute("checked").Should().Be("true", "На странице мероприятий тогл 'Только мои' не активен");
    }

    [Test]
    // Проверка отображения Новогодней темы
    public void XmasThemeOn()
    {
        // // 1. Нажать на меню профиля
        ProfileMenuClick();
        // 2. Нажать в меню на "Настройки"
        var settingsButton = driver.FindElement(By.CssSelector("[data-tid='Settings']"));
        settingsButton.Click();
        // 3. Нажать на тогл "Новогодняя тема"
        var xMasThemeToggle = driver.FindElement(By.XPath("//div[contains(text(), 'Новогодняя тема')]"));
        xMasThemeToggle.Click();
        // 4. Нажать на кнопку "Сохранить"
        var saveButton = driver.FindElement(By.XPath("//span[contains(text(), 'Сохранить')]"));
        saveButton.Click();
        
        // 5. Проверить наличие и значение новогодней темы в куках
        var xMasCookie= driver.Manage().Cookies.AllCookies
            .FirstOrDefault(cookie => cookie.Name.Contains("newYearAtmosphereStatus"));
        var xMasCookieValue = xMasCookie.Value;
        Assert.That(xMasCookieValue == "true", "Значение Cookie новогодней темы != true");
    }

    [Test]
    // Проверка соответствия даты по умолчанию с текущей системной датой в поле выбора даты на странице "Мероприятия"
    public void EventsCorrectDate()
    {
        // 1. Перейти на страницу "Мероприятия"
        driver.Navigate().GoToUrl(MainUrl + "/events");
        
        // 2. Ищем дату на странице и извлекаем ее значение
        var calendar = driver.FindElement(By.CssSelector("[data-tid='DatePicker'] input"))
            .GetAttribute("value");
        // 3. Парсим полученное текстовое значение даты в формат даты
        DateOnly calendarDate;
        DateOnly.TryParseExact(calendar, "dd/MM/yyyy", out calendarDate);
        
        // 4. Получаем текущую системную дату
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        
        // 5. Проверяем совпадает ли дата со страницы с системной датой
        Assert.That(calendarDate == currentDate, "[Поле выбора даты на странице 'Мероприятия'] Отображаемая по умолчанию дата не совпадает с текущей датой");
    }

    [TearDown]
    public void TearDown()
    {
        driver.Close();
        driver.Quit();
    }
}