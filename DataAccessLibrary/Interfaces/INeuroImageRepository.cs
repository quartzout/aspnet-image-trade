
using DataAccessLibrary.Models;
using RazorPages.Identity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLibrary.Interfaces;

public interface INeuroImageRepository
{
   
    Task<NeuroImageResult> StoreCopy(string copyImageFrom, NeuroImageInfo info, bool deleteOriginal = false);

    Task<NeuroImageResult> UpdateInfo(int idToUpdate, NeuroImageInfo info);

    Task<NeuroImageResult> UpdateFile(int idToUpdate, string copyFrom, bool deleteOriginal = false);


    Task Delete(int id);

    Task<NeuroImageResult> GetById(int id);


    Task<IEnumerable<NeuroImageResult>> GetAll();

    Task<IEnumerable<NeuroImageResult>> GetAllOnSale();


    Task<IEnumerable<NeuroImageResult>> GetOnSaleOfUser(string userID);

    Task<IEnumerable<NeuroImageResult>> GetInGalleryOfUser(string userID);

    Task<IEnumerable<NeuroImageResult>> GetInHeapOfUser(string userID);










}
