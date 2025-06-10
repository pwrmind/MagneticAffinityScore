namespace MAS;
class Program
{
    static void Main()
    {
        // 1. Инициализация системы
        var recommender = new RecommenderSystem(gamma: 1.0f);
        
        // 2. Добавление объектов с характеристиками [Жанр, Язык, Рейтинг]
        // Жанр: 0-драма, 1-боевик, 2-комедия
        // Язык: 0-русский, 1-английский, 2-китайский
        // Рейтинг: 0-5
        var items = new List<Item> {
            new Item { Id = 1, OriginalFeatures = new float[] { 2.0f, 0.0f, 4.5f } }, // Комедия, русский
            new Item { Id = 2, OriginalFeatures = new float[] { 2.0f, 1.0f, 3.5f } }, // Комедия, английский
            new Item { Id = 3, OriginalFeatures = new float[] { 0.0f, 0.0f, 4.8f } }, // Драма, русский
            new Item { Id = 4, OriginalFeatures = new float[] { 1.0f, 2.0f, 3.2f } }  // Боевик, китайский
        };

        foreach (var item in items) recommender.AddItem(item);

        // 3. Создание пользователей
        // Пользователь 1: Любит комедии (2.0), предпочитает русский (0.0)
        var user1 = new User {
            OriginalPreferences = new float[] { 2.0f, 0.0f, 4.0f },
            Weights = new float[] { 3.0f, 5.0f, 1.0f } // Важен язык
        };
        
        // Пользователь 2: Любит боевики (1.0), смотрит на любом языке
        var user2 = new User {
            OriginalPreferences = new float[] { 1.0f, 0.5f, 3.5f },
            Weights = new float[] { 5.0f, 0.5f, 2.0f } // Важен жанр
        };
        
        recommender.PrepareUser(user1);
        recommender.PrepareUser(user2);

        // 4. Получение рекомендаций
        Console.WriteLine("Рекомендации для Пользователя 1 (важен язык):");
        var recs1 = recommender.GetRecommendations(user1);
        foreach (var item in recs1)
        {
            Console.WriteLine($"ID: {item.Id}, Norm: [{string.Join(", ", item.NormalizedFeatures.Select(f => f.ToString("F2")))}]");
        }

        Console.WriteLine("\nРекомендации для Пользователя 2 (важен жанр):");
        var recs2 = recommender.GetRecommendations(user2);
        foreach (var item in recs2)
        {
            Console.WriteLine($"ID: {item.Id}, Norm: [{string.Join(", ", item.NormalizedFeatures.Select(f => f.ToString("F2")))}]");
        }
        
        // Вывод информации о нормализации
        Console.WriteLine("\nСтатистика нормализации:");
        Console.WriteLine($"Минимумы: [{string.Join(", ", recommender.GetFeatureMins())}]");
        Console.WriteLine($"Максимумы: [{string.Join(", ", recommender.GetFeatureMaxs())}]");
    }
}
