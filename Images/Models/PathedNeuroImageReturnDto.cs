namespace Images.Models;


/// <summary>
/// Модель, возвращаемая из бд
/// </summary>
public class PathedNeuroImageReturnDto
{
    public PathedNeuroImageReturnDto() { }

    public string? InfoName { get; set; }
    public string? InfoDescription { get; set; }
    public bool? InfoIsPosted { get; set; }
    public string? Filename { get; set; }
    public int? Id { get; set; }
}

