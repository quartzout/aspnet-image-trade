namespace ImageGenerator.Interfaces;

/// <summary>
/// Интерфейс, генерирующий изображения. 
/// </summary>
public interface IImageGenerator
{
	public Task<string> GeneratePicture();
}