namespace RazorPages.Models.Classes.UI;


/// <summary>
/// Модель, в которую биндится пост запрос с информацией о изображении 
/// (например, при сохранении сгенерированной картинки в галерею), 
/// которую нужно добавить в NeuroImageInfo соотстветвующего изображения.
/// </summary>
public class ImageInfoModelPOST
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }
    public bool IsOnSale { get; set; }
    public int? Price { get; set; }
}