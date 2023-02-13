using Users.Identity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLibrary.Models;

#nullable disable
/// <summary>
/// Модель, описывающая изображение. Не содержит путь к файлу или id изображения в бд, имеет только связанные с ним характеристики
/// </summary>
public class NeuroImageInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsInGallery { get; set; }
    public DateTime GenerationDate { get; set; }
    public int? SortingImportance { get; set; }
    public bool? IsOnSale { get; set; }
    public int? Price { get; set; }
    public string OwnerId { get; set;}

    public NeuroImageInfo(bool isInGallery, DateTime generationDate, string ownerId)
    {
        IsInGallery = isInGallery;
        GenerationDate = generationDate;
        OwnerId = ownerId;
    }

    public NeuroImageInfo() { }
}
