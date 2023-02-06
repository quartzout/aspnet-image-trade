using Webapp174.Models.Interfaces;
using Webapp174.Models.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Classes;
using DataAccessLibrary;
using DataAccessLibrary.Classes.Options;
using System.Linq.Expressions;
using Users.Models.Classes.AutoMapper;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Users.Models.Implementations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Users.Identity.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using AutoMapper;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
//После создания builder с помощью него регестрируются сервисы для dependency injection
//с помощью методов AddTransient, AddScoped и AddSingleton
//AddTransient - при каждом новом требовании сервиса создается новый объект реализации
//AddScoped - при каждом новом html-запросе создается новый объект реализации
//AddSingleton - сервис создается единожды при первом запросе


//добавление сервисов для хранения изображений из проекта Images
builder.Services.AddTransient<IInfoStorage, InfoStorage>();
builder.Services.Configure<NeuroImageInfoStorageOptions>(
	options => options.SqlDbConnectionString = builder.Configuration.GetConnectionString("ImagesSqlDb"));

builder.Services.AddTransient<INeuroImageStorage, NeuroImageStorage>();

builder.Services.AddTransient<IFileStorage, FileStorage>();
builder.Services.Configure<FileStorageOptions>(
    options => options.fileStorageAbsolutePath = builder.Environment.ContentRootPath + "wwwroot\\image-storage\\");


//https://stackoverflow.com/questions/52492666/what-is-the-point-of-configuring-defaultscheme-and-defaultchallengescheme-on-asp

//Добавление сервисов, предоставляемых Identity. Они являются оберткой над стандартным сервисом аутентификации и авторизации
//и предоставляют методы по хранению и управлению пользователями и ролями. UserManager позволяет создавать и хранит пользователей,
//SignInManager позволяет регистрироваться, входить и выходить из сессии, RoleManager позволяет задавать роли для авторизации.
builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<UserDbContext>() //куда будут сохраняться пользователи
    .AddDefaultTokenProviders();

//Класс контекста для Entity Framework
builder.Services.AddDbContext<UserDbContext>(opts => opts.UseSqlServer(
    builder.Configuration.GetConnectionString("UsersSqlDb")));



//Настройка SignInManager, SignInManager и RoleManager 
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 0;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.SignIn.RequireConfirmedEmail = true;
    

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});


//Настройка cookie-схемы, использующейся Identity как дефолтная SignIn-схема
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);

    options.LoginPath = "/Identity/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});


builder.Services.AddAuthentication()

    //Добавляет схему для логина с гуглом. Никак не связан с Identity. CallbackPath по дефолту - /signIn-google, и если такой страницы
    //не существует, этот middleware будет сам обрабатывать возвращенный с гугла редирект, содержащий информацию пользователя,
    //и персистить эту информацию в SignInScheme (или в ту, которая установлена по дефолту). Из-за этого, когда используются middleware 
    //внешнего логина без Identity, с ним вместе еще добавляется схема, поддерживающую signIn (куки). Однако для того чтобы использовать
    //екстерные логины вместе с Identity, необходимо создать страницу по адресу CallbackPath и прописать в ней signin или создание нового
    //пользователя а потом signin с помощью userManager и signInManager. Это нужно для того, чтобы пользователь с помощью внешнего
    //провайдера логинился в своего пользователя Identity, а не в рандомную куки-сессию с сохраненными там клеймами из гугла,
    //как произошло бы если бы страницы под CallbackPath не было бы.
    .AddGoogle(opts => {
        opts.ClientId = "712280448300-tolfe38v9lk8uab5vi9qeddpuk3ua1ij.apps.googleusercontent.com";
        opts.ClientSecret = "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";
        opts.CallbackPath = "/Identity/Login/";
    });



//PictureGenerator
builder.Services.AddTransient<IPictureGenerator, PictureGeneratorMock>();
builder.Services.Configure<PictureGeneratorMockOptions>(options => {
	options.ImageStorageDirectory = builder.Environment.ContentRootPath + "Generator Images\\imgs\\";
    options.GeneratedImageDirectory = builder.Environment.ContentRootPath + "Generator Images\\Generated Images\\";});

//MyHelper
builder.Services.AddTransient<MyHelper>();


//AutoMapper
builder.Services.AddAutoMapper(typeof(MyProfile));



//Кроме добавления сервисов (через builder.Add[Scope], возвращающий IServiceCollection) с помощью созданного builer можно
//добавлять конфигруации (builder.Configuration, возвращающий ConfigurationManager),
//настраивать логирование (builder.Logging, возвращающий ILoggingBuilder),
//и настраивать IHostBuilder и IWebHostBuilder (builder.Host, возвращающий IHostBuilder и builder.WebHost, возвращающий IWebHostBuilder).
//Как только все эти вещи сделаны, запускается builer.Build(), который собирает приложение.

//https://habr.com/ru/post/594971/ - разница между .net5 и .net6 и описание WebApplication и WebApplicatiobBuilder.


//This method configures the MVC services for the commonly used features with controllers for an API. 
//This combines the effects of AddMvcCore(IServiceCollection), AddApiExplorer(IMvcCoreBuilder), AddAuthorization(IMvcCoreBuilder),
//AddCors(IMvcCoreBuilder), AddDataAnnotations(IMvcCoreBuilder), and AddFormatterMappings(IMvcCoreBuilder).
//To add services for controllers with views call AddControllersWithViews(IServiceCollection) on the resulting builder.
//To add services for pages call AddRazorPages(IServiceCollection) on the resulting builder.
builder.Services.AddControllers(); 



var app = builder.Build();
//Получение обьекта WebApplication. С помощью этого обьекта настраивается request pipeline - цепочка сервисов,
//называемых middleware-сервисами, обрабатывающих все поступающие запросы.
//Запрос, поступающий на сервер, проходит через каждый middleware-сервис по очереди, и они модифицируют его перед
//отправкой дальше по цепочке. Какой то из таких сервисов, зачастую последний - endpoints - формирует response и отправляет обратно по цепочке,
//пока запрос не ударится об самый первый middleware - после чего он уже отправляется на клиент.
//Примерами middleware будет аутентификация и авторизация пользователя или переадресация при ошибочном запросе.
//Сервисы в цепочке выстраиваются в таком же порядке, в каком были вызваны функции их подключения в коде ниже.
//Все сервисы по очеереди получают обьект HTTPContext и модифицируют его response (пока он идет внутрь по цепочке, когда
//он отправляется обратно модифицировть его не принято, и middleware на обратном пути делают такие посторонние вещи
//как отлов ошибкок, логирование и т.д.


//В енве разработки ошибки 500 описываются подробно
if (app.Environment.IsDevelopment()) {

	app.UseDeveloperExceptionPage();  
	app.UseStatusCodePages();
}

app.UseStaticFiles();
//Статические файлы - отправляются всем клиентам в неизменном виде, например, картинки, медиафайлы, js, css.
//Могут быть закешированы в браузере клиента, чтобы ускорить последующие загрузки сайта. По умолчанию хранятся в папке wwwroot.
//Все остальное же формируется в момент запроса и зависит от параметров, например, генерируемые с помощью razer страницы html.
//Такой динамический контент генерируется в эндпоинтах.
//Отлов запроса на получение статического файла происходит до авторизации. 
//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0#static-file-authorization

//По адресу запроса и данным из предидущих сервисов определяет и назначает запросу эндпоинт, который будет исполнен в конце цепи.
app.UseRouting();

//Пытается аутентифицировать пользователя. Заполняет у запроса данные User, либо устанавливает его как анонимного. 
app.UseAuthentication();  

//Определяет, достаточно ли у определенного в UseAuthentication пользователя прав (клеймы или роли) для того, чтобы пустить его
//на назначенный эндпоинт   
app.UseAuthorization();

//Добавляет эндпоинты для созданных api-контроллеров
app.MapControllers();

//Вызывает делегат (лямбда \ метод контроллера) назначенный в UseRouting, и замыкает цепь.
app.UseEndpoints(_ => { });


app.Run();
//Когда middleware сервисы настроены, можно запускать приложение.


//MVC:

//По стандарту, запросы для контроллеров размечены по следующей схеме:  https:\\<адрес сайта>\<имя контроллера>\<имя действия>\<индекс>
//Индекс - необязательный параметр, передающийся в действие. Если он не указан, он будет равен Null. Если имя действия не указано, будет использовано
//действие Index. Если имя контроллера не указано, будет использоваться Home. Из-за этого необходимо, чтобы в проекте по дефолту был контроллер Home с действием
//Index. Изменять схему маршрутизации для запросов можно в финальном middleware - UseEndpoints.

//Методы контроллеров возвращат тип IActionResult, обобщающий все возможные типы данных, которые может возвратить действие в контроллере. 
//Среди них могут быть, например, ViewResult или RedirectToActionResult.

//Контроллеры должны быть названы с припиской Controller на конце, например HomeController. Какой именно View возвратиться из действия, не указывается
//напрямую в этом действии. Вместо этого фреймворк ищет в папке Views папку с названием контроллера, из которого исходит действие, а в ней - файл .cshtml,
//названный также, как и действие. Тоесть, действие Index контроллера HomeController вернет View, расположенный по адресу Views/Home/Index.cshtml

/* You should always use view models to and from your controller actions.
 Those view models are classes that are specifically defined to meet the requirements of the given view.

So back to the main point: fields are supposed to be modified only from within the given class. 
They should not be accessed directly from the outside. They represent and hold internal state of the class. 
Properties on the other hand is what should be exposed to the outside world*/


// Razor Pages:

// Отличная от MVC система, позволяющая организовыввать razor-страницы и модели к ним проще. Razor Page представляет собой сразу же
// и controller, и model и view, устраняя необходимость изменять код в нескольких разных файлах для изменения одной страницы, как это нужно в MVC.
// Razor Page состоит из cshtml файла, такого же, как и в MVC, и класса, наследуюемого от PageModel, в котором прописываются одновременно свойства ViewModel 
//и методы OnPost и OnGet, обрабатывающие запросы, подобно контроллерам. По дефолту обьявленные свойства могут задаваться в методах и читаться в файле cshtml.
//Чтобы отправить информацию из cshtml в методы, необходимо выставить свостйтвам аттрибут BindProperties. В этом случае при отправке запроса дополнительные 
//данные через ? будут биндиться к заданным своствам, и их смогут прочитать методы OnPost и OnGet.



// В одном asp.net проекте можно совмещать MVC и Razor pages. Что будет использоваться зависит от того, присутствуют ли MVC и Users серсвисы в DI
// и соответствующие middleware в request pipeline, и как настроена маршрутизация.



