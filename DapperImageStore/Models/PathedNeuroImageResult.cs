namespace DataAccessLibrary.Models;


#nullable disable
/// <summary>
/// Обертка над <see cref="NeuroImageInfo"/>, содержащая вместе с ним название файла изображения и id сохраненной информации
/// в бд
/// </summary>
public class PathedNeuroImageResult
{
    public int Id { get; set; }
    public string Filename { get; set; }
    public NeuroImageInfo Info { get; set; }
}
