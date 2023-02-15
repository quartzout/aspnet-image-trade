using ImageGenerator.Interfaces;
using Microsoft.Extensions.Options;

namespace ImageGenerator.Classes;

/// <summary>
/// Мокает <see cref="IImageGenerator"/>, копируя случайную из восьми имеющихся картинок в отведенную директорию.
/// Класс настроек задает директорию, где хранятся картинки для выбора и директорию, куда копируются "сгенерированные" изображения
/// </summary>
public class ImageGeneratorMock : IImageGenerator
{
	private const int NumberOfImages = 8;
	private readonly IOptions<ImageGeneratorMockOptions> _options;

	public ImageGeneratorMock(IOptions<ImageGeneratorMockOptions> options)
	{
		_options = options;
	}
	
	public string GeneratePicture()
	{
		//Выбрать картинку
		Random rnd = new Random();
		int imageNumber = rnd.Next(1, NumberOfImages);
		string filename = imageNumber + ".jpg";
		string pickedImagePath = _options.Value.OriginalImagesDirectory + filename;

		//Скопировать в директорию сгенерированных картинок
		string generatedImagePath = _options.Value.GeneratedImageDirectory + filename;
		File.Copy(pickedImagePath, generatedImagePath);

		//вернуть полученный путь
		return generatedImagePath;

    }

}