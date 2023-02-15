namespace Images.Models;

#nullable disable
/// <summary>
/// Обьект, представляющий результат операции с файлом.
/// </summary>
public class FileResult
{
    //Обтект создается вручную, не маппером, поэтому конструктор с параметрами
    public FileResult(string newFileName, string newFullName)
    {
        this.newFileName = newFileName;
        this.newFullName = newFullName;
    }

    /// <summary>
    /// Название файла. Является ключом для доступа к файлу в хранилище
    /// </summary>
    public string newFileName { get; set; }
    /// <summary>
    /// Полный путь до файла.
    /// </summary>
    public string newFullName { get; set; }

}
