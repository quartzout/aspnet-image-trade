using Images.Models;

namespace Images.Models;

#nullable disable
/// <summary>
/// Модель, возвращаемая функциями в DapperImageStore, полностью описывающее изображение, полученное из базы данных
/// </summary>
public class ImageResult
{
    public ImageInfo Info { get; set; }
    /// <summary>
    /// Id изображения в бд
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Полный путь до файла изображения
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Возвращает web-путь к изображению, готового для вставки в html, полученному из полного локального пути к файлу изображения.
    /// Метод определен в модели и именован по формату "Get<property>" для того, чтобы автомаппер мог сам замаппить FullName из
    /// ImageResult в WebFullName из ImageGetDto
    /// </summary>
    public string GetWebFullName()
    {
        string result = FullName;
        try
        {
            result = result.Split(new[] { "\\wwwroot\\" }, StringSplitOptions.None)[1];
            result = result.Replace(@"\", "/");
        }
        catch { }
        return result;
    }

    /// <summary>
    /// Возвращает TimespanSegments с временем, прошедшим с генерации файла
    /// Метод определен в модели и именован по формату "Get<property>" для того, чтобы автомаппер мог сам заполнить GeneratedAgoTimespanSegments
    /// в ImageGetDto и ImageModelGET из ImageResult
    /// </summary>
    public TimespanSegments GetGeneratedAgoTimespanSegments()
    {
        return new(timespan: DateTime.Now - Info.GenerationDate);
    }
}
