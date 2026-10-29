using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToObjects;

public record Offer(string Shop, string Product, string Category, decimal Price, bool InStock);
public record WishItem(string Product, decimal MaxPrice);

internal class Program
{
    private static bool _simulateFailure = false;
    private static List<Offer> _loadedOffers = new();
    private static int _lastRespondingShopsCount = 0;

    // Фіксований список бажань покупця (4-5 позицій)
    private static readonly List<WishItem> WishList = new()
    {
        new("Ноутбук Asus ZenBook 14", 39000m),
        new("Смартфон Apple iPhone 15", 35500m),
        new("Навушники Sony WH-1000XM5", 14500m),
        new("Павербанк Xiaomi 20000mAh", 1600m),
        new("Смартфон Google Pixel 8 Pro", 38000m)
    };

    private static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Початкове завантаження пропозицій
        await LoadOffersAsync(concurrent: true);

        bool isRunning = true;
        while (isRunning)
        {
            PrintMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 8);
            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    await LoadOffersAsync(concurrent: true);
                    break;
                case 2:
                    await LoadOffersAsync(concurrent: false);
                    break;
                case 3:
                    ShowCheapestOffers();
                    break;
                case 4:
                    ShowCategoryStatistics();
                    break;
                case 5:
                    SearchByPriceRange();
                    break;
                case 6:
                    CompareWithWishList();
                    break;
                case 7:
                    ExportCheapestToCsv();
                    break;
                case 8:
                    _simulateFailure = !_simulateFailure;
                    Console.WriteLine($"Симуляція збою магазину 'ЦифраПлюс': {(_simulateFailure ? "УВІМКНЕНО (викличе помилку)" : "ВИМКНЕНО (штатний режим)")}");
                    break;
                case 0:
                    isRunning = false;
                    break;
            }

            if (isRunning)
            {
                Console.WriteLine("\nНатисніть Enter для повернення в меню...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("Роботу програми завершено.");
    }

    private static void PrintMenu()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    Практична робота №4 | Варіант 1                    ");
        Console.WriteLine("    Порівняння цін в інтернет-магазинах (LINQ + Async) ");
        Console.WriteLine("=======================================================");
        Console.WriteLine($"Завантажено пропозицій: {_loadedOffers.Count} (успішно відповіло магазинів: {_lastRespondingShopsCount} з 3)");
        Console.WriteLine($"Стан збою магазину: {(_simulateFailure ? "[УВІМКНЕНО]" : "[ВИМКНЕНО]")}");
        Console.WriteLine("1. Завантажити пропозиції одночасно (Task.WhenAll)");
        Console.WriteLine("2. Завантажити пропозиції послідовно (await у циклі)");
        Console.WriteLine("3. Найдешевша пропозиція кожного товару (MinBy серед наявних)");
        Console.WriteLine("4. Статистика за категоріями (кількість, мін, сер, макс)");
        Console.WriteLine("5. Пошук у ціновому діапазоні (OrderBy + ThenBy)");
        Console.WriteLine("6. Порівняння зі списком бажань (LINQ query syntax join)");
        Console.WriteLine("7. Зберегти звіт найдешевших пропозицій у CSV-файл");
        Console.WriteLine("8. Перемкнути режим збою магазину 'ЦифраПлюс'");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("-------------------------------------------------------");
    }

    // =========================================================================
    // Асинхронне завантаження даних
    // =========================================================================

    private static async Task LoadOffersAsync(bool concurrent)
    {
        string mode = concurrent ? "одночасне (Task.WhenAll)" : "послідовне (по черзі)";
        Console.WriteLine($"--- Завантаження даних з магазинів: {mode} ---");

        string[] shops = { "ТехноМаркет", "ЦифраПлюс", "Електросвіт" };
        List<Offer> allOffers = new();
        int successfulShops = 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        if (concurrent)
        {
            var tasks = shops.Select(shop => FetchShopOffersSafeAsync(shop)).ToArray();
            var results = await Task.WhenAll(tasks);

            foreach (var shopOffers in results)
            {
                if (shopOffers != null)
                {
                    allOffers.AddRange(shopOffers);
                    successfulShops++;
                }
            }
        }
        else
        {
            foreach (string shop in shops)
            {
                var shopOffers = await FetchShopOffersSafeAsync(shop);
                if (shopOffers != null)
                {
                    allOffers.AddRange(shopOffers);
                    successfulShops++;
                }
            }
        }

        stopwatch.Stop();
        _loadedOffers = allOffers;
        _lastRespondingShopsCount = successfulShops;

        Console.WriteLine($"\n[Успішно] Завантаження завершено за {stopwatch.ElapsedMilliseconds} мс.");
        Console.WriteLine($"Отримано пропозицій: {_loadedOffers.Count} від {successfulShops} з {shops.Length} магазинів.");
    }

    private static async Task<List<Offer>?> FetchShopOffersSafeAsync(string shopName)
    {
        try
        {
            return await GetOffersFromShopAsync(shopName);
        }
        catch (HttpRequestException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Помилка з'єднання] Магазин '{shopName}': {ex.Message}");
            Console.ResetColor();
            return null;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Непередбачена помилка] Магазин '{shopName}': {ex.Message}");
            Console.ResetColor();
            return null;
        }
    }

    private static async Task<List<Offer>> GetOffersFromShopAsync(string shopName)
    {
        // Імітація мережевої затримки
        await Task.Delay(350);

        if (_simulateFailure && shopName == "ЦифраПлюс")
        {
            throw new HttpRequestException("Помилка 503: Сервер магазину перевантажений або тимчасово недоступний.");
        }

        return shopName switch
        {
            "ТехноМаркет" => new List<Offer>
            {
                new("ТехноМаркет", "Ноутбук Lenovo IdeaPad 3", "Ноутбуки", 22500m, true),
                new("ТехноМаркет", "Ноутбук Asus ZenBook 14", "Ноутбуки", 39999m, true),
                new("ТехноМаркет", "Ноутбук Apple MacBook Air M2", "Ноутбуки", 47500m, true),
                new("ТехноМаркет", "Смартфон Samsung Galaxy S24", "Смартфони", 31999m, true),
                new("ТехноМаркет", "Смартфон Apple iPhone 15", "Смартфони", 36200m, true),
                new("ТехноМаркет", "Смартфон Google Pixel 8 Pro", "Смартфони", 39500m, false), // Немає в наявності
                new("ТехноМаркет", "Навушники Sony WH-1000XM5", "Аксесуари", 14900m, true),
                new("ТехноМаркет", "Павербанк Xiaomi 20000mAh", "Аксесуари", 1550m, true),
                new("ТехноМаркет", "Миша Logitech MX Master 3S", "Аксесуари", 4200m, true)
            },
            "ЦифраПлюс" => new List<Offer>
            {
                new("ЦифраПлюс", "Ноутбук Lenovo IdeaPad 3", "Ноутбуки", 21900m, true),
                new("ЦифраПлюс", "Ноутбук Asus ZenBook 14", "Ноутбуки", 38500m, true),
                new("ЦифраПлюс", "Ноутбук Apple MacBook Air M2", "Ноутбуки", 48900m, false), // Закінчився
                new("ЦифраПлюс", "Смартфон Samsung Galaxy S24", "Смартфони", 32500m, true),
                new("ЦифраПлюс", "Смартфон Apple iPhone 15", "Смартфони", 35400m, true),
                new("ЦифраПлюс", "Смартфон Google Pixel 8 Pro", "Смартфони", 38900m, false), // Немає в наявності
                new("ЦифраПлюс", "Навушники Sony WH-1000XM5", "Аксесуари", 14200m, true),
                new("ЦифраПлюс", "Павербанк Xiaomi 20000mAh", "Аксесуари", 1650m, true),
                new("ЦифраПлюс", "Миша Logitech MX Master 3S", "Аксесуари", 4050m, true)
            },
            "Електросвіт" => new List<Offer>
            {
                new("Електросвіт", "Ноутбук Lenovo IdeaPad 3", "Ноутбуки", 23100m, true),
                new("Електросвіт", "Ноутбук Asus ZenBook 14", "Ноутбуки", 37900m, true),
                new("Електросвіт", "Ноутбук Apple MacBook Air M2", "Ноутбуки", 46900m, true),
                new("Електросвіт", "Смартфон Samsung Galaxy S24", "Смартфони", 31400m, true),
                new("Електросвіт", "Смартфон Apple iPhone 15", "Смартфони", 35900m, true),
                new("Електросвіт", "Смартфон Google Pixel 8 Pro", "Смартфони", 40200m, false), // Немає в наявності
                new("Електросвіт", "Навушники Sony WH-1000XM5", "Аксесуари", 15100m, false), // Закінчився
                new("Електросвіт", "Павербанк Xiaomi 20000mAh", "Аксесуари", 1490m, true),
                new("Електросвіт", "Миша Logitech MX Master 3S", "Аксесуари", 4300m, true)
            },
            _ => new List<Offer>()
        };
    }

    // =========================================================================
    // Запити LINQ to Objects
    // =========================================================================

    /// <summary>
    /// LINQ Запит 1: Знаходить найдешевшу пропозицію для кожного товару лише серед наявних (MinBy).
    /// Граничний випадок: якщо товару немає в наявності у жодному магазині, він позначається як "немає в наявності".
    /// </summary>
    private static void ShowCheapestOffers()
    {
        Console.WriteLine("--- Найдешевші пропозиції для кожного товару ---");
        if (_loadedOffers.Count == 0)
        {
            Console.WriteLine("Немає даних. Спочатку завантажте пропозиції.");
            return;
        }

        var groups = _loadedOffers
            .GroupBy(o => o.Product)
            .OrderBy(g => g.Key);

        Console.WriteLine($"{"Товар",-30} | {"Магазин",-14} | {"Ціна",-12} | {"Статус"}");
        Console.WriteLine(new string('-', 70));

        foreach (var group in groups)
        {
            var inStockOffers = group.Where(o => o.InStock);
            var cheapest = inStockOffers.MinBy(o => o.Price);

            if (cheapest != null)
            {
                Console.WriteLine($"{cheapest.Product,-30} | {cheapest.Shop,-14} | {cheapest.Price,9:F2} грн | В наявності");
            }
            else
            {
                Console.WriteLine($"{group.Key,-30} | {"-",-14} | {"-",-12} | [НЕМАЄ В НАЯВНОСТІ]");
            }
        }
    }

    /// <summary>
    /// LINQ Запит 2: Статистика за категоріями (кількість, мінімальна, середня та максимальна ціна).
    /// </summary>
    private static void ShowCategoryStatistics()
    {
        Console.WriteLine("--- Статистика за категоріями товарів ---");
        if (_loadedOffers.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return;
        }

        var categoryStats = _loadedOffers
            .GroupBy(o => o.Category)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count(),
                InStockCount = g.Count(x => x.InStock),
                MinPrice = g.Min(x => x.Price),
                AvgPrice = g.Average(x => x.Price),
                MaxPrice = g.Max(x => x.Price)
            })
            .OrderBy(s => s.Category);

        Console.WriteLine($"{"Категорія",-15} | {"Всього",-7} | {"В наявн.",-8} | {"Мін. ціна",-11} | {"Сер. ціна",-11} | {"Макс. ціна",-11}");
        Console.WriteLine(new string('-', 75));

        foreach (var stat in categoryStats)
        {
            Console.WriteLine($"{stat.Category,-15} | {stat.Count,7} | {stat.InStockCount,8} | {stat.MinPrice,9:F2} грн | {stat.AvgPrice,9:F2} грн | {stat.MaxPrice,9:F2} грн");
        }
    }

    /// <summary>
    /// LINQ Запит 3: Фільтрація за ціновим діапазоном із сортуванням OrderBy та ThenBy.
    /// </summary>
    private static void SearchByPriceRange()
    {
        Console.WriteLine("--- Пошук товарів у ціновому діапазоні ---");
        if (_loadedOffers.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return;
        }

        decimal minPrice = ReadDecimal("Введіть мінімальну ціну (грн): ");
        decimal maxPrice = ReadDecimal("Введіть максимальну ціну (грн): ");

        if (minPrice > maxPrice)
        {
            Console.WriteLine("Помилка: мінімальна ціна не може перевищувати максимальну.");
            return;
        }

        var results = _loadedOffers
            .Where(o => o.InStock && o.Price >= minPrice && o.Price <= maxPrice)
            .OrderBy(o => o.Price)
            .ThenBy(o => o.Shop)
            .ToList();

        Console.WriteLine($"\nЗнайдено пропозицій у діапазоні від {minPrice:F2} до {maxPrice:F2} грн: {results.Count}");
        if (results.Count == 0)
        {
            Console.WriteLine("Жоден товар у наявності не потрапляє у зазначений діапазон.");
            return;
        }

        Console.WriteLine($"{"Товар",-30} | {"Магазин",-14} | {"Ціна",-12}");
        Console.WriteLine(new string('-', 62));

        foreach (var item in results)
        {
            Console.WriteLine($"{item.Product,-30} | {item.Shop,-14} | {item.Price,9:F2} грн");
        }
    }

    /// <summary>
    /// LINQ Запит 4: Порівняння зі списком бажань за допомогою query-синтаксису (Join).
    /// </summary>
    private static void CompareWithWishList()
    {
        Console.WriteLine("--- Порівняння пропозицій магазинів зі списком бажань ---");
        if (_loadedOffers.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return;
        }

        // Використання Join у query-синтаксисі LINQ
        var matchedWishes = from wish in WishList
                            join offer in _loadedOffers on wish.Product equals offer.Product
                            where offer.InStock && offer.Price <= wish.MaxPrice
                            orderby offer.Price
                            select new
                            {
                                wish.Product,
                                offer.Shop,
                                offer.Price,
                                wish.MaxPrice,
                                Savings = wish.MaxPrice - offer.Price
                            };

        var list = matchedWishes.ToList();
        Console.WriteLine($"Знайдено вигідних варіантів за списком бажань: {list.Count}");

        Console.WriteLine($"{"Товар",-28} | {"Магазин",-13} | {"Ціна",-10} | {"Макс. ціна",-10} | {"Економія",-10}");
        Console.WriteLine(new string('-', 80));

        foreach (var m in list)
        {
            Console.WriteLine($"{m.Product,-28} | {m.Shop,-13} | {m.Price,8:F2} грн | {m.MaxPrice,8:F2} грн | +{m.Savings,7:F2} грн");
        }
    }

    /// <summary>
    /// LINQ Запит 5: Збереження звіту найдешевших пропозицій у CSV-файл.
    /// </summary>
    private static void ExportCheapestToCsv()
    {
        Console.WriteLine("--- Збереження звіту в CSV-файл ---");
        if (_loadedOffers.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return;
        }

        string filePath = "cheapest_offers.csv";

        var cheapestRows = _loadedOffers
            .GroupBy(o => o.Product)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var best = g.Where(x => x.InStock).MinBy(x => x.Price);
                return new
                {
                    Product = g.Key,
                    Category = g.First().Category,
                    Shop = best != null ? best.Shop : "Немає в наявності",
                    Price = best != null ? best.Price.ToString("F2", CultureInfo.InvariantCulture) : "0.00",
                    Status = best != null ? "В наявності" : "Немає в наявності"
                };
            });

        StringBuilder sb = new();
        sb.AppendLine("Product;Category;Shop;Price;Status");

        foreach (var row in cheapestRows)
        {
            sb.AppendLine($"\"{row.Product}\";\"{row.Category}\";\"{row.Shop}\";{row.Price};\"{row.Status}\"");
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        Console.WriteLine($"[Успішно] Звіт збережено у файл '{Path.GetFullPath(filePath)}'.");
    }

    // =========================================================================
    // Допоміжні методи введення
    // =========================================================================

    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int val) && val >= min && val <= max)
            {
                return val;
            }
            Console.WriteLine($"Введіть число від {min} до {max}.");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? text = Console.ReadLine();
            if (decimal.TryParse(text, out decimal val) && val >= 0)
            {
                return val;
            }
            Console.WriteLine("Введіть коректне число.");
        }
    }
}
