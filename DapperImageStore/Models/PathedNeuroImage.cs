namespace DataAccessLibrary.Models;

#nullable disable
/// <summary>
/// Обертка над <see cref="NeuroImageInfo"/>, хранящая вместе с ним название файла.
/// </summary>
public class PathedNeuroImage
{
    //Обьекты создаются вручную, без маппера, поэтому конструктор с аргументами
    public PathedNeuroImage(string filename, NeuroImageInfo info)
    {
        Filename = filename;
        Info = info;
    }

    public PathedNeuroImage() { }

    public string Filename { get; set; }
    public NeuroImageInfo Info { get; set; }

}
