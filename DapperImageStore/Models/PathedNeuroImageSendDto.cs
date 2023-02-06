namespace DataAccessLibrary.Classes;

#nullable disable

/// <summary>
/// Модель, отсылаемая на бд
/// </summary>
public class PathedNeuroImageSendDto
{
    public PathedNeuroImageSendDto() { }

    public string? Filename { get; set; }

    public string OwnerId { get; set; }
    public DateTime GenerationDate { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsInGallery { get; set; }
    public bool? IsOnSale { get; set; }
    public int? Price { get; set; }
    public int? SortingImportance { get; set; }


        
}
