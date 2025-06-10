namespace MAS;

public class RecommenderSystem
{
    private readonly List<Item> _items = new();
    private readonly float _gamma;
    private float[] _featureMins = Array.Empty<float>();
    private float[] _featureMaxs = Array.Empty<float>();
    private bool _isNormalizationInitialized = false;

    public RecommenderSystem(float gamma = 1.0f)
    {
        _gamma = gamma;
    }

    public void AddItem(Item item)
    {
        if (!_isNormalizationInitialized)
        {
            InitializeNormalization(item.OriginalFeatures.Length);
        }

        // Обновляем диапазоны нормализации
        UpdateNormalizationRanges(item.OriginalFeatures);

        // Нормализуем признаки объекта
        item.NormalizedFeatures = NormalizeFeatures(item.OriginalFeatures);
        _items.Add(item);
    }

    // Инициализация системы нормализации
    private void InitializeNormalization(int featureCount)
    {
        _featureMins = new float[featureCount];
        _featureMaxs = new float[featureCount];
        Array.Fill(_featureMins, float.MaxValue);
        Array.Fill(_featureMaxs, float.MinValue);
        _isNormalizationInitialized = true;
    }

    // Обновление диапазонов нормализации
    private void UpdateNormalizationRanges(float[] features)
    {
        for (int i = 0; i < features.Length; i++)
        {
            if (features[i] < _featureMins[i]) _featureMins[i] = features[i];
            if (features[i] > _featureMaxs[i]) _featureMaxs[i] = features[i];
        }
    }

    // Нормализация признаков к диапазону [0, 1]
    public float[] NormalizeFeatures(float[] features)
    {
        float[] normalized = new float[features.Length];

        for (int i = 0; i < features.Length; i++)
        {
            float range = _featureMaxs[i] - _featureMins[i];
            // Защита от деления на ноль
            normalized[i] = range > 0.0001f
                ? (features[i] - _featureMins[i]) / range
                : 0.5f; // Среднее значение при отсутствии диапазона
        }

        return normalized;
    }

    // Подготовка пользователя к рекомендациям
    public void PrepareUser(User user)
    {
        // Нормализуем предпочтения пользователя
        user.NormalizedPreferences = NormalizeFeatures(user.OriginalPreferences);

        int featureCount = user.NormalizedPreferences.Length;

        // Расчет вектора запроса: Q = 2 * gamma * weights * preferences
        user.QueryVector = new float[featureCount];
        for (int i = 0; i < featureCount; i++)
        {
            user.QueryVector[i] = 2 * _gamma * user.Weights[i] * user.NormalizedPreferences[i];
        }

        // Расчет константы пользователя: C = gamma * Σ(weights[i] * preferences[i]^2)
        user.UserConstant = 0;
        for (int i = 0; i < featureCount; i++)
        {
            user.UserConstant += user.Weights[i] *
                user.NormalizedPreferences[i] *
                user.NormalizedPreferences[i];
        }
        user.UserConstant *= _gamma;
    }

    // Расчет силы притяжения
    public float CalculateAffinity(User user, Item item)
    {
        float distanceSquared = 0;

        for (int i = 0; i < user.NormalizedPreferences.Length; i++)
        {
            float diff = item.NormalizedFeatures[i] - user.NormalizedPreferences[i];
            distanceSquared += user.Weights[i] * diff * diff;
        }

        return MathF.Exp(-_gamma * distanceSquared);
    }

    // Получение рекомендаций
    public List<Item> GetRecommendations(User user, int retrievalCount = 1000, int finalCount = 10)
    {
        // Этап 1: Быстрый отбор кандидатов
        var candidates = _items
            .Select(item => new
            {
                Item = item,
                LinearScore = CalculateLinearScore(user.QueryVector, item.NormalizedFeatures)
            })
            .OrderByDescending(x => x.LinearScore)
            .Take(retrievalCount)
            .ToList();

        // Этап 2: Точное ранжирование
        var rankedItems = candidates
            .Select(candidate => new
            {
                Item = candidate.Item,
                Affinity = CalculateAffinity(user, candidate.Item)
            })
            .OrderByDescending(x => x.Affinity)
            .Take(finalCount)
            .Select(x => x.Item)
            .ToList();

        return rankedItems;
    }

    private float CalculateLinearScore(float[] queryVector, float[] features)
    {
        float score = 0;
        for (int i = 0; i < queryVector.Length; i++)
        {
            score += queryVector[i] * features[i];
        }
        return score;
    }
}
