# ImageTrade (API)
Мой пет-проект на C# и ASP.NET. Backend REST API для веб приложения, где пользователи могут создавать, продавать и покупать за виртуальную валюту алгоритмически сгенерированные картинки.

## Стек:
- C# 8.0
- ASP.NET Core
- Entity Framework (ORM для хранения пользователей)
- Dapper (ORM для хранения изображений)
- Identity
- AutoMapper

Большинство кода подробно закомментировано и у всех публично доступных интерфейсов, классов и методов есть описания.

## Репозиторий состоит из трех проектов:
- **[API](https://github.com/quartzout/image-trade-api/tree/master/API)** - является входной точкой приложения, содержит api-контроллеры, обрабатывающие запросы. Контроллер Account, осуществляющий регистрацию и логин пользователя, существует в двух версиях - [AccountController](https://github.com/quartzout/image-trade-api/blob/master/API/Controllers/AccountController.cs), использующий Cookie, и [JwtAccountController](https://github.com/quartzout/image-trade-api/blob/master/API/Controllers/JwtAccountController.cs), использующий JWT-токены. Авторизация на любом контроллере работает с любой из этих двух схем.
Проект зависит от двух других проектов:

- **[Images](https://github.com/quartzout/image-trade-api/tree/master/Images)** - Предоставляет интерфейс [INeuroImageStorage](https://github.com/quartzout/image-trade-api/blob/master/Images/Interfaces/INeuroImageStorage.cs) для хранения изображений и информации о них. Его реализация [NeuroImageStorage](https://github.com/quartzout/image-trade-api/blob/master/Images/Classes/NeuroImageStorage.cs) использует два других интерфейса - [IInfoStorage](https://github.com/quartzout/image-trade-api/blob/master/Images/Interfaces/IInfoStorage.cs) и [IFileStorage](https://github.com/quartzout/image-trade-api/blob/master/Images/Interfaces/IFileStorage.cs) для хранения информации о изображениях (реализован с помощью Dapper и Sql-бд) и самих файлов изображения (реализован с помощью файловой системы) соответственно. Для связи с БД используется Dapper вместо Entity Framework, так как я хотел изучить его.

- **[Users](https://github.com/quartzout/image-trade-api/tree/master/Users)** - Предоставляет доступ к пользователям и серсивам авторизации с помощью Identity c использованием Entity Framework как ORM.

- **[ImageGenerator](https://github.com/quartzout/image-trade-api/tree/master/ImageGenerator)** - Предоставляет доступ к [IImageGenerator](https://github.com/quartzout/image-trade-api/tree/master/Images/Interfaces/IImageGenerator.cs), который позволяет генерировать случайные алгоритмические изображения.
Текущая реализация с помощью Selenium генерирует картинку на [сайте Скотта Пэкина](https://www.pakin.org/random-art/) и сохраняет ее в файлы.

- **[Razor Front](https://github.com/quartzout/image-trade-api/tree/master/Razor)** - Незаконченный фронт для API на Razor Pages, котоый я перестал делать, так
как посчитал, что перейти на отдельный React фронт будет удобнее.  


## API:

Все эндпоинты возвращают объект ValidationError вместе с ошибками 400, в которых указаны проблемы:

    {
        "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        "title": "One or more validation errors occurred.",
        "status": 400,
        "traceId": "00-dcf32e9b6b9f4b47a95bea739a52fb19-74d4a123e5cee200-00",
        "errors": {
            "": [
                "Cannot find user"
            ]
        }
    }

- Account - Действия аккаунта с Cookie
  - **POST api/account/login/**
  
        Залогинить пользователя с помощью Cookie
        В тело принимает string Email, string Password, bool rememberMe
    
  - **POST api/account/register/**
  
        Зарегестрировать пользователя с помощью Cookie
        В тело запрос принимает string Email, string DisplayName, string Password, string Description, bool rememberMe 
        
  - **POST api/account/login/**

        Разлогинить пользователя (убрать Cookie)
        
- JwtAccount - Действия аккаунта с токеном JWT.
  - **POST api/jwtaccount/login/**
  
        Залогинить пользователя с помощью jwt
        В тело принимает string Email, string Password, bool rememberMe
        Возвращает string token
    
  - **POST api/jwtaccount/register/**
  
        Зарегестрировать пользователя с помощью Jwt
        В тело запрос принимает string Email, string DisplayName, string Password, string Description, bool rememberMe 
        Возвращает string token
        
- User - информация о пользователе

  Формат пользователя:
  
        {
          "id": "f167435d-4465-4076-897e-6f9411487b28",
          "displayName": "name",
          "email": "email@gmail.com",
          "coinBalance": 21,
          "description": "desc"
        }
      

  - **GET api/user/current/** 
  
        *требуется авторизация*
        Вернуть информацию о текущем залогиненном пользователе
        Возвращает string Id, string displayName, string email, string description, int coinBalance
    
  - **GET api/user/find/<email>/**
  
        Вернуть информацию о пользователе под почтой <email>
        Возвращает string Id, string displayName, string email, string description, int coinBalance
        
- UserImages - Изоюражения пользователя
  
  Формат изображения:
  
      {
        "id": "218",
        "name": "name",
        "description": "desc",
        "generatedAgoTimespanSegments": {
            "days": 0,
            "hours": 6,
            "minutes": 45,
            "seconds": 7
        },
        "webFullName": "image-storage/c851ef8e-bf72-4e66-b914-822c19904436.jpg",
        "isInGallery": true,
        "isOnSale": true,
        "price": 10
      }

  - **GET api/images/<email>/<status>** 
  
        Вернуть все изображения пользователя с почтой <email>.
        status: 
          inHeap - сгенерированные изображения без названия, не сохраненные в галерею. Требуется авторизация.
          inGallery - изображения, которые пользователь сохранил в галерею (в том числе выставленные на продажу)
          onSale - изображения, которые пользователь выставил на продажу
        Возвращает список из изображений.
        
        
- UserActions - Различные действия авторизированных пользователей. *требует авторизации*
  

  - **POST api/useractions/generate/** 
  
        Генерирует изображение, сохраняет его в разлеле необработанных (inHeap) у авторизированного пользователя.
        Возвращает сгенерированное изображение
    
  - **POST api/useractions/updateimageinfo/**
  
        Перезаписывает информацию о переданном изображении на переданное в тело.
        В тело запрос принимает тот же формат, что и возвращает, за исклчением свойств 
        generatedAgoTimespanSegments, webFullName и id, их перезаписать нельзя
        Возвращает обновленное изображение.
  
  - **POST api/useractions/buy/<imageID>**
  
        Посылает запрос на покупку изображения с id <imageID> авторизированным пользователем у владельца изображения. 
        
        
        
        
        
           
    
        
        
     
   
    
    






