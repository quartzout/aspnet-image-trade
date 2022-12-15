using Webapp174.Models.Interfaces;
using Webapp174.Models.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Classes;
using DataAccessLibrary;
using DataAccessLibrary.Classes.Options;
using System.Linq.Expressions;
using RazorPages.Models.Classes.AutoMapper;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using RazorPages.Models.Implementations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RazorPages.Identity.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using AutoMapper;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


//PathedInfoRepository
builder.Services.AddTransient<INeuroImageStoredInfoRepository, SqlNiStoredInfoRepository>();
builder.Services.Configure<SqlNiStoredInfoRepositoryOptions>(
	options => options.SqlDbConnectionString = builder.Configuration.GetConnectionString("ImagesSqlDb")
	);

//NeuroImageRepository
builder.Services.AddTransient<INeuroImageRepository, NeuroImageRepository>();

//FileRepository
builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.Configure<FileRepositoryOptions>(
    options => options.fileStorageAbsolutePath = builder.Environment.ContentRootPath + "wwwroot\\image-storage\\");



//Identity
builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<UserDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

builder.Services.AddDbContext<UserDbContext>(opts => opts.UseSqlServer(
    builder.Configuration.GetConnectionString("UsersSqlDb")));

builder.Services.AddAuthentication()

    // Добавляет схему для логина с гуглом. Никак не связан с Identity. CallbackPath по дефолту - /signIn-google, и если такой страницы
    //не существует, этот middleware будет сам обрабатывать возвращенный с гугла редирект, содержащий информацию пользователя,
    //и персистить эту информацию в SignInScheme (или в ту, которая установлена по дефолту). Из-за этого, когда используются мидлвейр 
    //екстерного логина без Identity, с ним вместе еще добавляется схема, поддерживающую signIn (куки). Однако для того чтобы использовать
    //екстерные логины вместе с Identity, необходимо создать страницу по адресу CallbackPath и прописать в ней signin или создание нового
    //пользователя а потом signin с помощью userManager и signInManager. Это нужно для того, чтобы пользователь с помощью экстерного
    //провайдера логинился в своего пользователя Identity, а не в рандомную куки-сессию с сохраненными там клеймами из гугла,
    //как произошло бы если бы страницы под CallbackPath не было бы.
    .AddGoogle(opts => {
        opts.ClientId = "712280448300-tolfe38v9lk8uab5vi9qeddpuk3ua1ij.apps.googleusercontent.com";
        opts.ClientSecret = "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";
        opts.CallbackPath = "/Identity/Login/";
    });



//https://stackoverflow.com/questions/52492666/what-is-the-point-of-configuring-defaultscheme-and-defaultchallengescheme-on-asp


//Настройка SignInManager, LoginManager и RoleManager Identity
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

//Настройка кук-схемы, использующейся Identity как дефолтная SignIn-схема
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);

    options.LoginPath = "/Identity/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
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

#region Old Identity
/*   
// :)
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();

//LoginManager
builder.Services.AddTransient<IdentityManager>();


//Give HttpContext access to LoginManager
builder.Services.AddHttpContextAccessor();
//UsersDataAccess
builder.Services.AddDbContext<UserDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("UsersSqlDb")));



//Auth
builder.Services.AddAuthentication()
	.AddCookie(authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme, opts =>
	{
		opts.LoginPath = "/Account/Login";
		opts.Cookie.Name = "identity_cookie";
		opts.ExpireTimeSpan = TimeSpan.FromHours(1);
		opts.SlidingExpiration = true;
	});

*/
#endregion


//Создается builder, который нужен для четырех вещей: добавление сервисов в контейнер Dependency Injection (builder.Servises, возвращающий IServiceCollection),
//добавление конфигруаций (builder.Configuration, возвращающий ConfigurationManager), настройка логирования (builder.Logging, возвращающий ILoggingBuilder),
//и общая настройка IHostBuilder и IWebHostBuilder (builder.Host, возвращающий IHostBuilder и builder.WebHost, возвращающий IWebHostBuilder).
//Как только все эти вещи сделаны, запускается builer.Build(), который собирает приложение.

//https://habr.com/ru/post/594971/ - разница между .net5 и .net6 и описание WebApplication и WebApplicatiobBuilder.

builder.Services.AddRazorPages(); //Добавляет в app все сервисы, необходимые
                                  //для работы с Razor Pages. 

builder.Services.AddControllers();

//Добавляют связи между интерфейсами и реализующими их конкретными классам. Когда контроллеру понадобиться передать
//реализацию интерфейсов, ему будет переданы именно эти реализации. AddSingleton, AddTransient и AddScoped делают одно и то же,
//но с разными областями видимости. 

//Mock - обьект, используемый в тестировании, который имитирует поведение реального обьекта.

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



//Если название текущего Enviroment равно Development
if (app.Environment.IsDevelopment()) {

	app.UseDeveloperExceptionPage();  //При ошибке показывать в браузере ее детали
	app.UseStatusCodePages();
}

app.UseStaticFiles();
//Статические файлы - отправляются всем клиентам в неизменном виде, например, картинки и другие медиафайлы.
//Могут быть закешированы в браузере клиента, чтобы упростить последующие загрузки сайта. По умолчанию хранятся в папке wwwroot.
//Динамические файлы же формируются в момент запроса и зависят от параметров, например, генерируемые razerом страницы html.
//Функция UseStaticFiles обьявляет, что мы используем статические файлы.

app.UseRouting();

app.UseAuthentication();  //Не смотря на то, что это звучит контринтуитивно, UseAuthentication должен стоять после UseRouting,
							//так как для логирования пользователя он использует какую-то информацию про ендпоинты из UseRouting
app.UseAuthorization();

app.MapControllers();
/*app.MapControllerRoute(
    name: "ajax",
    pattern: "ajax/{controller}/{action}/");
*/
app.MapRazorPages();



app.UseEndpoints(_ => { });



//MVC работает по следующей схеме - каждый поступивший запрос указывает на какой то контроллер и какое-то действие (action) в этом контроллере.
//Фреймворк вызывает это действие у этого контроллера, и в него передаются все необходимые контроллеру зависимости из контейнера DI. Действие обрабатывает
//запрос, определяет, какой
//
//требуется отправить клиенту как ответ на запрос (и вообще, нужно ли отправлять View, либо же делать какое-то другое действие,
//например переадресацию на другое действие или возвращение ошибки), генерирует обьект ViewModel для передачи нужному View, в котором содержится
//вся информация, которую должен отобразить View, и возвращает сгенерированный View. Фреймворк MVC ловит возвращенный View и отправляет его клиенту как response.

//По стандарту, запросы для контроллеров размечены по следующей схеме:  https:\\<адрес сайта>\<имя контроллера>\<имя действия>\<индекс>
//Индекс - необязательный параметр, передающийся в действие. Если он не указан, он будет равен Null. Если имя действия не указано, будет использовано
//действие Index. Если имя контроллера не указано, будет использоваться Home. Из-за этого необходимо, чтобы в проекте по дефолту был контроллер Home с действием
//Index. Изменять схему маршрутизации для запросов можно в финальном middleware - endpoints.

//Действия контроллеров возвращат тип IActionResult, обобщающий все возможные типы данных, которые может возвратить действие в контроллере. 
//Среди них могут быть, например, ViewResult или RedirectToActionResult.

//Контроллеры должны быть названы с припиской Controller на конце, например HomeController. Какой именно View возвратиться из действия, не указывается
//напрямую в этом действии. Вместо этого фреймворк ищет в папке Views папку с названием контроллера, из которого исходит действие, а в ней - файл .cshtml,
//названный также, как и действие. Тоесть, действие Index контроллера HomeController вернет View, расположенный по адресу Views/Home/Index.cshtml

/* You should always use view models to and from your controller actions.
 Those view models are classes that are specifically defined to meet the requirements of the given view.

So back to the main point: fields are supposed to be modified only from within the given class. 
They should not be accessed directly from the outside. They represent and hold internal state of the class. 
Properties on the other hand is what should be exposed to the outside world*/

// Razor Pages - отличная от MVC система, позволяющая организовыввать razor-страницы и модели к ним проще. Razor Page представляет собой сразу же
// и controller, и model и view, устраняя необходимость изменять код в нескольких разных файлах для изменения одной страницы, как это нужно в MVC.
// Razor Page состоит из cshtml файла, такого же, как и в MVC, и класса, наследуюемого от PageModel, в котором прописываются одновременно свойства ViewModel 
//и методы OnPost и OnGet, обрабатывающие запросы, подобно контроллерам. По дефолту обьявленные свойства могут задаваться в методах и читаться в файле cshtml.
//Чтобы отправить информацию из cshtml в методы, необходимо выставить свостйтвам аттрибут BindProperties. В этом случае при отправке запроса дополнительные 
//данные через ? будут биндиться к заданным своствам, и их смогут прочитать методы OnPost и OnGet.

// В одном asp.net проекте можно совмещать MVC и Razor pages. Что будет использоваться зависит от того, присутствуют ли MVC и RazorPages серсвисы в DI
// и соответствующие middleware в request pipeline, и как настроена маршрутизация.

app.Run();
//Когда middleware сервисы настроены, можно запускать приложение.


