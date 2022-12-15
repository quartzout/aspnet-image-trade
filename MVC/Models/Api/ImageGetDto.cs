using DapperImageStore.Models;

namespace RazorPages.Models.Api
{
	public class ImageGetDto
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
        public TimespanSegments TimespanSegments { get; set; }
        public string WebFullName { get; set; }
        public bool IsInGallery { get; set; }
		public bool IsOnSale { get; set; }
		public int Price { get; set; }
	}
}
