# ImageTrade (API)
Мой пет-проект на C# и ASP.NET. Backend REST API для веб приложения, где пользователи могут создавать, продавать и покупать за виртуальную валюту алгоритмически сгенерированные картинки.

## Стек:
- C# 8.0
- ASP.NET Core
- Entity Framework (ORM для хранения пользователей)
- Dapper (ORM для хранения изображений)
- Identity
- AutoMapper

## Репозиторий состоит из трех проектов:
- **[API](https://github.com/quartzout/image-trade-api/tree/master/API)** - является входной точкой приложения, содержит api-контроллеры, обрабатывающие запросы. Контроллер Account, осуществляющий регистрацию и логин пользователя, существует в двух версиях - [AccountController](https://github.com/quartzout/image-trade-api/blob/master/API/Controllers/AccountController.cs), использующий Cookie, и [JwtAccountController](https://github.com/quartzout/image-trade-api/blob/master/API/Controllers/JwtAccountController.cs), использующий JWT-токены. Зависит от двух других проектов:

- **[Images](https://github.com/quartzout/image-trade-api/tree/master/Images)** - Предоставляет интерфейс [INeuroImageStorage](https://github.com/quartzout/image-trade-api/blob/master/Images/Interfaces/INeuroImageStorage.cs) для хранения изображений и информации о них. Его реализация [NeuroImageStorage](https://github.com/quartzout/image-trade-api/blob/master/Images/Classes/NeuroImageStorage.cs) использует два других интерфейса - [IInfoStorage](https://github.com/quartzout/image-trade-api/blob/master/Images/Interfaces/IInfoStorage.cs) и [IFileStorage](https://github.com/quartzout/image-trade-api/blob/master/Images/Interfaces/IFileStorage.cs) для хранения информации о изображениях (реализован с помощью Dapper и Sql-бд) и самих файлов изображения (реализован с помощью файловой системы) соответственно. Для связи с БД используется Dapper вместо Entity Framework, так как я хотел изучить его.

- **[Users](https://github.com/quartzout/image-trade-api/tree/master/Users)** - Предоставляет доступ к пользователям и серсивам авторизации с помощью Identity c использованием Entity Framework как ORM.







