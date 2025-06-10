namespace MAS;

public class User
{
    public float[] OriginalPreferences { get; set; } // Исходные предпочтения
    public float[] NormalizedPreferences { get; set; } // Нормализованные предпочтения
    public float[] Weights { get; set; }             // Индивидуальные веса признаков
    public float[] QueryVector { get; set; }         // Вектор запроса
    public float UserConstant { get; set; }          // Константа для пользователя
}
