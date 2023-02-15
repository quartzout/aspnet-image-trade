namespace Images.Models;


#nullable disable
/// <summary>
/// Обертка над <see cref="ImageInfo"/>, содержащая вместе с ним название файла изображения и id сохраненной информации
/// в бд
/// </summary>
public class PathedImageResult
{
    public int Id { get; set; }
    public string Filename { get; set; }
    public ImageInfo Info { get; set; }
}
