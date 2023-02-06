using Microsoft.Extensions.Options;
using Webapp174.Models.Interfaces;

namespace Webapp174.Models.Mocks;

/// <summary>
/// Мокает <see cref="IPictureGenerator"/>, копируя случайную из восьми имеющихся картинок в отведенную директорию.
/// Класс настроек задает директорию, где хранятся картинки для выбора и директорию, куда копируются "сгенерированные" изображения
/// </summary>
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
		//Выбрать картинку
		Random rnd = new Random();
		int imageNumber = rnd.Next(1, NumberOfImages);
		string filename = imageNumber + ".jpg";
		string pickedImagePath = _options.Value.ImageStorageDirectory + filename;

		//Скопировать в директорию сгенерированных картинок
		string generatedImagePath = _options.Value.GeneratedImageDirectory + filename;
		File.Copy(pickedImagePath, generatedImagePath);

		//вернуть полученный путь
		return generatedImagePath;

    }

}