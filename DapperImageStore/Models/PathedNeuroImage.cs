namespace DataAccessLibrary.Models;

#nullable disable
public class PathedNeuroImage
{
    //class instances are created manually, not with mapper
    public PathedNeuroImage(string filename, NeuroImageInfo info)
    {
        Filename = filename;
        Info = info;
    }

    public PathedNeuroImage() { }

    public string Filename { get; set; }
    public NeuroImageInfo Info { get; set; }

}
