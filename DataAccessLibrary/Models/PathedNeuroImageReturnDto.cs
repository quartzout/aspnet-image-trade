namespace DataAccessLibrary.Classes;

public partial class SqlNiStoredInfoRepository
{
    //Dto thats being returned from sqlserver via dapper
    public class PathedNeuroImageReturnDto
    {
        public PathedNeuroImageReturnDto() { }

        public string? InfoName { get; set; }
        public string? InfoDescription { get; set; }
        public bool? InfoIsPosted { get; set; }
        public string? Filename { get; set; }
        public int? Id { get; set; }
    }
}
