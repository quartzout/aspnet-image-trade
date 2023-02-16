namespace ImageGenerator.Interfaces;

/// <summary>
/// Интерфейс, генерирующий изображения. 
/// </summary>
public interface IImageGenerator
{
	/// <summary>
	/// Генерирует изображение, сохраняет его в файловой системе и возвращает полный путь к нему
	/// </summary>
	public Task<string> GeneratePicture();
}