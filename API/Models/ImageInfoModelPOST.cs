namespace API.Models;


/// <summary>
/// Модель, в которую биндится пост запрос с информацией о изображении 
/// (например, при сохранении сгенерированной картинки в галерею), 
/// которую нужно добавить в ImageInfo соотстветвующего изображения.
/// </summary>
public class ImageInfoModelPOST
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsOnSale { get; set; }
    public int? Price { get; set; }
}