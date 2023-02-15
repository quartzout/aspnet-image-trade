namespace Images.Models;

#nullable disable
/// <summary>
/// Обертка над <see cref="ImageInfo"/>, хранящая вместе с ним название файла.
/// </summary>
public class PathedImage
{
    //Обьекты создаются вручную, без маппера, поэтому конструктор с аргументами
    public PathedImage(string filename, ImageInfo info)
    {
        Filename = filename;
        Info = info;
    }

    public PathedImage() { }

    public string Filename { get; set; }
    public ImageInfo Info { get; set; }

}
