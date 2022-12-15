using Microsoft.Extensions.Options;
using Webapp174.Models.Interfaces;

namespace Webapp174.Models.Mocks;

public class PictureGeneratorMock : IPictureGenerator
{
	private const int NumberOfImages = 8;
	private readonly IOptions<PictureGeneratorMockOptions> _options;

	public PictureGeneratorMock(IOptions<PictureGeneratorMockOptions> options)
	{
		_options = options;
	}
	
	public string GeneratePicture()
	{
		//Pick an image
		Random rnd = new Random();
		int imageNumber = rnd.Next(1, NumberOfImages);
		string filename = imageNumber + ".jpg";
		string pickedImagePath = _options.Value.ImageStorageDirectory + filename;

		//Copy to generated image directory
		string generatedImagePath = _options.Value.GeneratedImageDirectory + filename;
		File.Copy(pickedImagePath, generatedImagePath);

		return generatedImagePath;

    }

}