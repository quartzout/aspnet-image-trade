using ImageGenerator.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;
using Microsoft.Extensions.Options;

namespace ImageGenerator.Classes;

/// <summary>
/// Реализация <see cref="IImageGenerator"/>, использующая Selenium для взаимодействия с сайтом https://www.pakin.org/random-art/,
/// генерирующим абстрактные картинки по запросу.
/// </summary>
public class PakinImageGenerator : IImageGenerator
{



    private string firstTabHandle = "";
    private string secondTabHandle = "";
    private readonly ChromeDriver driver;
    private readonly IOptions<PakinImageGeneratorOptions> options;

    // Конструктор вызывается в program.cs при запуске проекта. В конструкторе запускется драйвер хрома в безоконном режиме,
    // прогружает необходимую страницу и генерирует для теста картинку. Драйвер сохраняется в поле и ждет с открытым браузером
    // вызовов метода GeneratePicture
    public PakinImageGenerator(IOptions<PakinImageGeneratorOptions> options)
    {
        var opts = new ChromeOptions();
        opts.AddArgument("--headless");
        opts.AddArgument("--window-size=500,500");
        driver = new ChromeDriver(opts);

        //Весь код, выполняемый драйвером, блокирующий, поэтому его необходимо оборачивать в асинхронный Task.Run,
        //который возвращает Task и продолжает выполнение кода. Так как мы не возвращаем ничего из кода внутри Task, его можно
        //оставить без await ("fire-and-forget")
        Task.Run(async () => {

            //Драйвер открывает вторую вкладку, в которой будет открываться сгенерированное изображение и просиходить скриншот.
            //Это нужно потому, что при открытии изображения в новой вкладке оно будет всегда отображено по центру экрана.
            //Браузер открывается размером ровно со сгенерированное изображение, и в скриншот браузера попадает только картинка.
            firstTabHandle = driver.CurrentWindowHandle;
            secondTabHandle = driver.SwitchTo().NewWindow(WindowType.Tab).CurrentWindowHandle;
            driver.SwitchTo().Window(firstTabHandle); //переключение обратно на первую вкладку после открытия второй

            driver.Navigate().GoToUrl("https://www.pakin.org/random-art/");

            //Ждем, пока генератор прогрузится и появится кнопка "Generate"
            new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(ExpectedConditions.ElementToBeClickable(By.TagName("button")));
            
            //Если что то пойдет не так на генерации, проект крашнется еще в program.cs, а не при запросе на генерацию картинки
            await GeneratePicture();
        });
        this.options = options;
    }

    //Деструктор закрывает браузер
     ~PakinImageGenerator()
    {
        Task.Run(() =>
        {
            driver.Quit();
        });
    }

    public async Task<string> GeneratePicture()
    {

        await Task.Run(() =>
        {
            // Ждем появления кнопки на случай, если первый запрос на генерацию поступил раньше, чем успел отработать конструктор
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(dr => dr.FindElement(By.TagName("button")));

            // Нажимаем на галочку "make tileable" при определенном шансе
            // (в конце метода снова нажимаем чтобы вернуть ее в исходное состояние)
            var random = new Random();
            var isTileable = random.Next(100) < options.Value.TileableChangePercent;
            if (!isTileable) driver.FindElement(By.Id("tileable")).Click();

            // Нажимаем на кнопку "Сгенерировать"
            driver.FindElement(By.TagName("button")).Click();

            // Ожидаем, пока кнопка появится заного. Это значит, что генерация закончилась.
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(dr => dr.FindElement(By.TagName("button")));

            // По неизвестной причине при попытке взять аттрибут src с помощью FindElement, берется его значение, 
            // бывшее еще до генерации. Однако свойство PageSource возвращает актуальную страницу, поэтому таким костылем через
            // него берем адрес сгенерированной картинки
            string newsrc = driver.PageSource.Substring(driver.PageSource.IndexOf("()\" src") + 9, 63);

            //"C:\\Users\\quartzout\\RiderProjects\\Webapp174 razorpages\\ImageGenerator\\generated.png"
            driver.SwitchTo().Window(secondTabHandle); //Переключаемся на вторую вкладку
            driver.Navigate().GoToUrl(newsrc); //Открываем в ней картинку
            driver.GetScreenshot() //Скриншотим и сохраняем в файл
                .SaveAsFile(options.Value.GeneratedImageDirectory + "generated.png", ScreenshotImageFormat.Png);
            driver.SwitchTo().Window(firstTabHandle); //Переключаем обратно на главную вкладку

            if (!isTileable) driver.FindElement(By.Id("tileable")).Click();

        });

        return options.Value.GeneratedImageDirectory + "generated.png";


    }
}
