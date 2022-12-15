using DapperImageStore.Models;

namespace DataAccessLibrary.Models;

#nullable disable
public class NeuroImageResult
{
    public NeuroImageInfo Info { get; set; }
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FullName { get; set; }

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

    public TimeSpan GetGeneratedAgo()
    {
        return DateTime.Now - Info.GenerationDate;
    }

    public TimespanSegments GetTimespanSegments()
    {
        TimespanSegments df = new(timespan: GetGeneratedAgo());
        return df;
    }
}
