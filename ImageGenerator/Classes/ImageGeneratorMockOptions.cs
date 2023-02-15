namespace ImageGenerator.Classes;


#nullable disable
public class ImageGeneratorMockOptions
{
    public const string SectionName = "ImageGeneratorMockOptions";

    /// <summary>
    /// Путь к папке, в которой хранятся подготовленные изображения, случайное из которых будет копироваться при "генерации"
    /// </summary>
    public string OriginalImagesDirectory { get; set; }
    /// <summary>
    /// Путь к папке, куда будут сохраняться "сгенерированные" изображения
    /// </summary>
    public string GeneratedImageDirectory { get; set; }
}