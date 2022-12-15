using DapperImageStore.Models;

namespace RazorPages.Models.Classes.UI;

#nullable disable
public class ImageModelGET
{
    public string Name { get; set; }
    public string Description { get; set; }
    public TimeSpan GeneratedAgo { get; set; }
    public TimespanSegments TimespanSegments { get; set; }
    public string WebFullName { get; set; }
    public int Id { get; set; }

}

