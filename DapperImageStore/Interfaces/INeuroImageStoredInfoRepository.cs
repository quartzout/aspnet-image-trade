using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using static DataAccessLibrary.Interfaces.INeuroImageStoredInfoRepository;
using DataAccessLibrary.Models;
using RazorPages.Identity.Classes;

namespace DataAccessLibrary.Interfaces;


//database-independent interface for storing StoredNeuroImage.
public interface INeuroImageStoredInfoRepository
{

    Task<PathedNeuroImageResult> Get(int id);

    Task Delete(int id);

    Task<IEnumerable<PathedNeuroImageResult>> GetAll();

    Task<IEnumerable<PathedNeuroImageResult>> GetAllOnSale();

    Task<IEnumerable<PathedNeuroImageResult>> GetOnSaleOfUser(string userID);

    Task<IEnumerable<PathedNeuroImageResult>> GetInHeapOfUser(string userID);

    Task<IEnumerable<PathedNeuroImageResult>> GetInGalleryOfUser(string userId);


    Task<PathedNeuroImageResult> Create(PathedNeuroImage info);

    Task<PathedNeuroImageResult> Replace(int id, PathedNeuroImage image);

    

}
