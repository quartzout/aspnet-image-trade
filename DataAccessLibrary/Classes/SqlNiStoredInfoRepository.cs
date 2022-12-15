using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Interfaces;
using Microsoft.Extensions.Options;
using DataAccessLibrary.Classes.Options;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using static DataAccessLibrary.Interfaces.INeuroImageStoredInfoRepository;
using AutoMapper;
using DataAccessLibrary.Models;
using RazorPages.Identity.Classes;
using System.Dynamic;

namespace DataAccessLibrary.Classes;

//Implementation of IDbAccess for SQL Server using ADO.NET for messaging with DB
//and Dapper for mapping data to and from sql-queries
public partial class SqlNiStoredInfoRepository : INeuroImageStoredInfoRepository
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


    public SqlNiStoredInfoRepository(IOptions<SqlNiStoredInfoRepositoryOptions> options)
    {
        _options = options;

        var config = new MapperConfiguration(
            cfg => {
                cfg.CreateMap<NeuroImageInfo, PathedNeuroImageSendDto>();
                cfg.CreateMap<PathedNeuroImage, PathedNeuroImageSendDto>()
                    .IncludeMembers(src => src.Info);
                cfg.CreateMap<PathedNeuroImage, PathedNeuroImageResult>()
                    .ForMember(dest => dest.Id, opts => opts.MapFrom((_, _, _, context) => context.Items["Id"]));
            });

        _mapper = config.CreateMapper();
    }

    public IOptions<SqlNiStoredInfoRepositoryOptions> _options { get; }

    //Execure stored procedure that returns data from db with select
    private async Task<IEnumerable<PathedNeuroImageResult>> QueryStoredProcedure<TParam>(string storedProcedure, TParam parameters)
    {
        using var connection = new SqlConnection(_options.Value.SqlDbConnectionString);

        return await connection.QueryAsync<PathedNeuroImageResult, NeuroImageInfo, PathedNeuroImageResult>(
            sql: storedProcedure,
            map: (pathedImage, info) => { pathedImage.Info = info; return pathedImage; },
            param: parameters,
            splitOn: "IsInGallery",
            commandType: CommandType.StoredProcedure);
    }


    //Execute stored procedure that puts data in db
    private async Task<int> ExecuteStoredProcedure<TParam>(
                    string storedProcedure,
                         TParam parameters,
                    string connectionStringName = "Default")
    {
        using var connection = new SqlConnection(_options.Value.SqlDbConnectionString);

        return await connection.ExecuteAsync(storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure);
    }


    public async Task<PathedNeuroImageResult> Get(int id)
    {
        var results = await QueryStoredProcedure("dbo.spImageInfos_get", new { id });

        var result = results.FirstOrDefault();
        if (result == null) throw new SqlReadException("record couldn't be found");

        return result; // _mapper.Map<PathedNeuroImageResult>(result);
    }


    public async Task<IEnumerable<PathedNeuroImageResult>> GetAll()
    {
        var results = await QueryStoredProcedure(
        "dbo.spImageInfos_getAll",
        new { });

        return results;

        
    }

    public async Task<IEnumerable<PathedNeuroImageResult>> GetAllOnSale()
    {
        var results = await QueryStoredProcedure(
        "dbo.spImageInfos_getAllOnSale",
        new { });

        return results;
    }


    public async Task<IEnumerable<PathedNeuroImageResult>> GetOnSaleOfUser(string userID)
    {
        var results = await QueryStoredProcedure(
        "dbo.spImageInfos_getOnSaleOfUser",
        new { userID });

        return results;
    }

    public async Task<IEnumerable<PathedNeuroImageResult>> GetInHeapOfUser(string userID)
    {
        var results = await QueryStoredProcedure(
        "dbo.spImageInfos_getInHeapOfUser",
        new { userID });

        return results;
    }



    public async Task<IEnumerable<PathedNeuroImageResult>> GetInGalleryOfUser(string userID)
    {
        var results = await QueryStoredProcedure(
        "dbo.spImageInfos_getInGalleryOfUser",
        new { userID });

        return results;
    }



    public async Task Delete(int id)
    {
        int rowsAffected = await ExecuteStoredProcedure<dynamic>(
        "dbo.spImageInfos_delete",
        new { id });

        if (rowsAffected < 1) throw new SqlWriteException("no rows affected");
    }

    public async Task<PathedNeuroImageResult> Replace(int id, PathedNeuroImage image)
    {
        var sendDto = _mapper.Map<PathedNeuroImageSendDto>(image);

        var parameters = new DynamicParameters();
        parameters.AddDynamicParams(sendDto);
        parameters.Add("@Id", id);

        int rowsAffected = await ExecuteStoredProcedure<dynamic>(
        "dbo.spImageInfos_update",
        parameters);

        if (rowsAffected < 1) throw new SqlWriteException("rows affected are less than one");

        return _mapper.Map<PathedNeuroImageResult>(image, opt => opt.Items["Id"] = id);
    }



    public async Task<PathedNeuroImageResult> Create(PathedNeuroImage image)
    {

        var sendDto = _mapper.Map<PathedNeuroImageSendDto>(image);

        var parameters = new DynamicParameters();
        parameters.AddDynamicParams(sendDto);
        parameters.Add("@NewIdentity", dbType: DbType.Int32, direction: ParameterDirection.Output);

        int rowsAffected = await ExecuteStoredProcedure<dynamic>(
        "dbo.spImageInfos_insert",
        parameters);

       

        int newID = parameters.Get<int>("@NewIdentity");

        return _mapper.Map<PathedNeuroImageResult>(image, opt => opt.Items["Id"] = newID);

    }
}
