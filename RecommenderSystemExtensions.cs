namespace MAS;

// Расширение для доступа к статистике нормализации
public static class RecommenderSystemExtensions
{
    public static float[] GetFeatureMins(this RecommenderSystem rs)
    {
        return rs.GetType().GetField("_featureMins", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance)?
            .GetValue(rs) as float[] ?? Array.Empty<float>();
    }
    
    public static float[] GetFeatureMaxs(this RecommenderSystem rs)
    {
        return rs.GetType().GetField("_featureMaxs", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance)?
            .GetValue(rs) as float[] ?? Array.Empty<float>();
    }
}