using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Images.Interfaces;
using Microsoft.Extensions.Options;
using Images.Classes.Options;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using static Images.Interfaces.IInfoStorage;
using AutoMapper;
using Images.Models;
using Users.Identity.Classes;
using System.Dynamic;
using Images.Classes;

namespace Images.Classes;

/// <summary>
/// Имплементация <see cref="IInfoStorage"/>, хранящая информацию изображений в SQLServer.
/// </summary>
public partial class InfoStorage : IInfoStorage
{

    public class SqlWriteException : Exception
    {
        public SqlWriteException() { }

        public SqlWriteException(string message) : base(message) { }
    }
    public class SqlReadException : Exception
    {
        public SqlReadException() { }

        public SqlReadException(string message) : base(message) { }
    }

    private readonly IMapper _mapper;


    public InfoStorage(IOptions<ImageInfoStorageOptions> options)
    {
        _options = options;

        var config = new MapperConfiguration(
            cfg => {

                //Для отправки в post-методы. Разворачиваем сложный тип PathedImage в плоский SendDto
                cfg.CreateMap<ImageInfo, PathedImageSendDto>();
                cfg.CreateMap<PathedImage, PathedImageSendDto>()
                    .IncludeMembers(src => src.Info);

                //в post-методах даппер не биндит результат на модель, так как эти методы не возвращают изображений.
                //вместо этого эти методы возвращают ту же самое переданное им изображение, но с добавленным 
                //к ней полем id сохраненного изображения (поэтому входная модель маппится на модель result, разница в которых
                // - как раз наличие поля id). Так как этот id мапперу нужно взять извне, а не из оригинальной модели, 
                //при маппинге в его специальный словарь Items передается id под соотсветствующим ключом.
                cfg.CreateMap<PathedImage, PathedImageResult>()
                    .ForMember(dest => dest.Id, opts => opts.MapFrom((_, _, _, context) => context.Items["Id"]));
            });

        _mapper = config.CreateMapper();
    }

    public IOptions<ImageInfoStorageOptions> _options { get; }

    /// <summary>
    /// Вспомогательный метод, открывающий соединение с бд, 
    /// достающий из бд данные по указанной процедуре и биндящий их на IEnumerable из сложных моделей.
    /// parameters задают аргументы процедуры в виде анонимного обьекта.
    /// </summary>
    private async Task<IEnumerable<PathedImageResult>> QueryStoredProcedure(string storedProcedure, object parameters)
    {
        using var connection = new SqlConnection(_options.Value.SqlDbConnectionString);

        //Dapper предоставляет расширения класса connection Query, QueryAsync, Execute и ExecuteAsync, позволяющие делать
        //запросы в бд и исполнять процедуры с автоматическим маппингом моделей в sql-запросы и
        //биндингом полученных данных на модели. Отправная модель должна быть плоская.

        //Query и QueryAsync запрашивает и возвращает данные из бд, Execute и ExecuteAsync исполняют процедуру и возвращают
        //количество задействованных записей.

        //С помощью дополнительных generic аргументов можно заставить функцию заполнять для каждой строчки в
        //результате по две модели вместо одной (PathedImageResult, куда запишется id и filename, и ImageInfo,
        //куда запишутся все остальные поля), а с помощью лямбды map функция кладет одну полученную модель внутрь другой и
        //возвращает родительскую. Таким образом можно реализовать биндинг сложных моделей
        //https://github.com/DapperLib/Dapper#multi-mapping

        return await connection.QueryAsync<
            PathedImageResult,
            ImageInfo,
            PathedImageResult>(    //Третий generic определяет тип возвращаемого из map обьекта.
                                        //В итоге метод вернет IEnumerable из этого дженерика
            commandType: CommandType.StoredProcedure, //Мы не делаем sql-запрос, а вызываем процедуру
            sql: storedProcedure,       //вместо запроса указываем имя вызываемой процедуры
            map: (pathedImage, info) => { pathedImage.Info = info; return pathedImage; },
            splitOn: "IsInGallery",     //Этот аргумент определяет, на каком столбце метод закончит заполнение первой модели и
                                        //начнет заполнение второй. IsInGallery - первый столбец с информацией, до него были
                                        //id и filename
            param: parameters);         //Аргументы, передаваемые в sql-процедуру.
            
    }


    /// <summary>
    /// Вспомогательный метод, открывающий соединение с бд, 
    /// исполняющий указанную процедуру, не возвращающую данные. 
    /// Вовзращает количество задействованных процедурой строчек. parameters задают аргументы процедуры.
    /// </summary>
    private async Task<int> ExecuteStoredProcedure(
                    string storedProcedure,
                         object parameters,
                    string connectionStringName = "Default")
    {
        using var connection = new SqlConnection(_options.Value.SqlDbConnectionString);

        return await connection.ExecuteAsync(storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure);
    }


    public async Task<PathedImageResult> Get(int id)
    {
        var results = await QueryStoredProcedure("dbo.spImageInfos_get", new { id });

        var result = results.FirstOrDefault();
        if (result == null) throw new SqlReadException("record couldn't be found");

        return result; 
    }


    public async Task<IEnumerable<PathedImageResult>> GetAll()
    {
        var results = await QueryStoredProcedure(
        "dbo.spImageInfos_getAll",
        new { });

        return results;

        
    }

    public async Task<IEnumerable<PathedImageResult>> GetAllOnSale()
    {
        var results = await QueryStoredProcedure(
        "dbo.spImageInfos_getAllOnSale",
        new { });

        return results;
    }

    public async Task<IEnumerable<PathedImageResult>> GetAllOfUserOfStatus(string userID, ImageStatus status)
    {
        var results = await QueryStoredProcedure(
        $"dbo.spImageInfos_get{status}OfUser", //для каждого статуса в бд предусмотрена отдельная процедура
        new { userID });

        return results;
    }


    public async Task Delete(int id)
    {
        int rowsAffected = await ExecuteStoredProcedure(
        "dbo.spImageInfos_delete",
        new { id });

        if (rowsAffected < 1) throw new SqlWriteException("no rows affected");
    }


    public async Task<PathedImageResult> Create(PathedImage image)
    {
        //В post-методах Replace и Create необходимо вместе с количеством задействованных строк возвращать id сохраненной
        //записи. Так как ExecuteAsync не может возвращать дополнительные значение, нужно перехватить эти значения другим образом.
        
        var sendDto = _mapper.Map<PathedImageSendDto>(image); //плоский обьект, который будет замапплен даппером в sql-запрос

        //Вместо анонимного обьекта с аргументами создаем специальный дапперовский тип DynamicParameters, позволяющий указать
        //вместе с аргументом его тип. Если указать тип как Output, аргумент станет параметром, в который sql-процедура
        //произведет return, и после ее выполнения мы сможем считать значение этого параметра.
        var parameters = new DynamicParameters();
        parameters.AddDynamicParams(sendDto);
        parameters.Add("@NewIdentity", dbType: DbType.Int32, direction: ParameterDirection.Output);

        int rowsAffected = await ExecuteStoredProcedure(
        "dbo.spImageInfos_insert",
        parameters);

        int newID = parameters.Get<int>("@NewIdentity"); //считываем вывод

        //маппим входную модель на выходную, добавляя к ней полученный id, передав его в opt мапперу
        return _mapper.Map<PathedImageResult>(image, opt => opt.Items["Id"] = newID);

    }


    public async Task<PathedImageResult> Replace(int id, PathedImage image)
    {
        var sendDto = _mapper.Map<PathedImageSendDto>(image);

        var parameters = new DynamicParameters();
        parameters.AddDynamicParams(sendDto);
        parameters.Add("@Id", id);

        int rowsAffected = await ExecuteStoredProcedure(
        "dbo.spImageInfos_update",
        parameters);

        if (rowsAffected < 1) throw new SqlWriteException("rows affected are less than one");

        return _mapper.Map<PathedImageResult>(image, opt => opt.Items["Id"] = id);
    }

}
