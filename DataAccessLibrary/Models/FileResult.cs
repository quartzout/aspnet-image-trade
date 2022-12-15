namespace DataAccessLibrary.Models;

#nullable disable
public class FileResult
{
    //FileResult is created by hand, not with mapper
    public FileResult(string newFileName, string newFullName)
    {
        this.newFileName = newFileName;
        this.newFullName = newFullName;
    }

    public string newFileName { get; set; }
    public string newFullName { get; set; }

}
