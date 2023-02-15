using Mvc.Models;

namespace API.Models
{
    public class ImagesResultGet
    {
        public IEnumerable<ImageGetDto> Images { get; set; } = new List<ImageGetDto>();
    }
}
