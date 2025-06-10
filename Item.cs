namespace MAS;

public class Item
{
    public int Id { get; set; }
    public float[] OriginalFeatures { get; set; }  // Исходные характеристики
    public float[] NormalizedFeatures { get; set; } // Нормализованные характеристики
}
