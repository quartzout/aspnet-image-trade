namespace DataAccessLibrary.Models;


#nullable disable
public class PathedNeuroImageResult
{
    public int Id { get; set; }
    public string Filename { get; set; }
    public NeuroImageInfo Info { get; set; }
}
