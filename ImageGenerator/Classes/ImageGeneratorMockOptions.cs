namespace ImageGenerator.Classes;


#nullable disable
public class PakinImageGeneratorOptions
{
    public const string SectionName = "PakinImageGeneratorOptions";

    /// <summary>
    /// С каким шансом в процентах изображения будут генерироваться с аттрибутом "make tileable"
    /// </summary>
    public int TileableChangePercent { get; set; }
    /// <summary>
    /// Путь к папке, куда будут сохраняться сгенерированные изображения
    /// </summary>
    public string GeneratedImageDirectory { get; set; }
}