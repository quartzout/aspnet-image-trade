using API.Models;
using Images.Models;

namespace API.Models
{
    /// <summary>
    /// Модель, полностью описывающая изображение. Возвращается из API
    /// </summary>
    public class ImageGetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TimespanSegments GeneratedAgoTimespanSegments { get; set; }
        public string WebFullName { get; set; }
        public bool IsInGallery { get; set; }
        public bool IsOnSale { get; set; }
        public int Price { get; set; }
    }
}
