using System;
using System.Collections.Generic;
using System.Text;
using GenericsDelegatesEvents.Collections;
using GenericsDelegatesEvents.Events;

namespace GenericsDelegatesEvents;

internal class Program
{
    // Лічильник витіснених елементів для лямбда-підписника
    private static int _removedItemsCount = 0;

    // Словник фіксованих курсів валют, що імітує базу даних банку
    private static readonly Dictionary<string, decimal> BankDatabase = new(StringComparer.OrdinalIgnoreCase)
    {
        ["USD"] = 41.50m,
        ["EUR"] = 44.80m,
        ["PLN"] = 10.30m,
        ["GBP"] = 52.40m,
        ["CHF"] = 46.20m,
        ["CAD"] = 30.15m
    };

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Створення кешу місткістю 3 елементи для наочної демонстрації витіснення
        Cache<string, decimal> rateCache = new(capacity: 3);

        // Підписка 1: Іменований метод для логування в консоль
        rateCache.ItemRemoved += OnItemRemovedNamedMethod;

        // Підписка 2: Лямбда-вираз для підрахунку та виведення лічильника
        rateCache.ItemRemoved += (sender, e) =>
        {
            _removedItemsCount++;
            Console.WriteLine($"[Лямбда-підписник] Загальна кількість витіснених записів: {_removedItemsCount}");
        };

        // Початкове наповнення кешу
        rateCache.GetOrAdd("USD", LoadRate);
        rateCache.GetOrAdd("EUR", LoadRate);

        bool isRunning = true;
        while (isRunning)
        {
            PrintMenu(rateCache);
            int choice = ReadInt("Ваш вибір: ", 0, 5);
            Console.WriteLine();

            isRunning = choice switch
            {
                1 => HandleGetRate(rateCache),
                2 => HandleShowCache(rateCache),
                3 => HandleFilterByThreshold(rateCache),
                4 => HandleSecondCacheDemo(),
                5 => HandleClearCache(rateCache),
                0 => false,
                _ => true
            };

            if (isRunning)
            {
                Console.WriteLine("\nНатисніть Enter для повернення в меню...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("Роботу програми завершено.");
    }

    private static void PrintMenu(Cache<string, decimal> cache)
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    Практична робота №3 | Варіант 1                    ");
        Console.WriteLine("          Кеш курсів валют (Cache<TKey, TValue>)       ");
        Console.WriteLine("=======================================================");
        Console.WriteLine($"Поточний стан кешу: {cache.Count} / {cache.Capacity} записів");
        Console.WriteLine("1. Отримати курс валюти (USD, EUR, PLN, GBP, CHF, CAD)");
        Console.WriteLine("2. Показати поточний вміст кешу");
        Console.WriteLine("3. Показати валюти з курсом, вищим за поріг (делегат Action + лямбда)");
        Console.WriteLine("4. Демонстрація другого узагальненого кешу Cache<int, string>");
        Console.WriteLine("5. Очистити кеш валют");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("-------------------------------------------------------");
    }

    private static bool HandleGetRate(Cache<string, decimal> cache)
    {
        Console.WriteLine("--- Отримання курсу валюти ---");
        Console.Write("Введіть код валюти (наприклад: USD, EUR, PLN, GBP, CHF, CAD): ");
        string? code = Console.ReadLine()?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(code))
        {
            Console.WriteLine("Код валюти не може бути порожнім.");
            return true;
        }

        try
        {
            decimal rate = cache.GetOrAdd(code, LoadRate, out bool fromCache);
            string sourceInfo = fromCache ? "[З КЕШУ]" : "[ЗАВАНТАЖЕНО З СЕРВЕРА]";
            Console.WriteLine($"\nРезультат: 1 {code} = {rate:F2} UAH {sourceInfo}");
        }
        catch (KeyNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Помилка: {ex.Message}");
            Console.ResetColor();
        }

        return true;
    }

    private static bool HandleShowCache(Cache<string, decimal> cache)
    {
        Console.WriteLine("--- Вміст кешу валют ---");
        if (cache.Count == 0)
        {
            Console.WriteLine("Кеш наразі порожній.");
            return true;
        }

        int index = 1;
        cache.ForEach((code, rate) =>
        {
            Console.WriteLine($"{index++}. Валюта: {code,-5} | Курс: {rate,8:F2} UAH");
        });

        return true;
    }

    private static bool HandleFilterByThreshold(Cache<string, decimal> cache)
    {
        Console.WriteLine("--- Фільтрація валют за пороговим значенням курсу ---");
        if (cache.Count == 0)
        {
            Console.WriteLine("Кеш наразі порожній. Спочатку додайте валюти.");
            return true;
        }

        decimal threshold = ReadDecimal("Введіть мінімальний поріг курсу (UAH): ");

        int count = 0;
        Console.WriteLine($"\nВалюти у кеші з курсом >= {threshold:F2} UAH:");

        // Використання ForEach з лямбда-виразом, який захоплює змінну threshold (замикання)
        cache.ForEach((code, rate) =>
        {
            if (rate >= threshold)
            {
                Console.WriteLine($"• {code,-5} : {rate:F2} UAH");
                count++;
            }
        });

        if (count == 0)
        {
            Console.WriteLine("Жодна валюта у кеші не відповідає вказаному критерію.");
        }

        return true;
    }

    private static bool HandleSecondCacheDemo()
    {
        Console.WriteLine("--- Демонстрація іншого типу кешу: Cache<int, string> ---");
        Console.WriteLine("Створюємо кеш користувачів за числовим ID місткістю 2 елементи.");

        Cache<int, string> userCache = new(capacity: 2);

        userCache.ItemRemoved += (s, e) =>
        {
            Console.WriteLine($"[Подія другого кешу] Видалено користувача Id={e.Key}: '{e.Value}'");
        };

        // Завантаження через передачу лямбда-виразу у ролі Func<int, string>
        Console.WriteLine("\nЗапит 1: ID = 101");
        string u1 = userCache.GetOrAdd(101, id =>
        {
            Console.WriteLine($"-> Імітація читання з БД користувача #{id}...");
            return $"Користувач #{id} (Слободян Ігор)";
        }, out bool fromCache1);
        Console.WriteLine($"Отримано: {u1} (з кешу: {fromCache1})");

        Console.WriteLine("\nЗапит 2: ID = 101 (повторно)");
        string u1Repeat = userCache.GetOrAdd(101, id => $"Користувач #{id}", out bool fromCache2);
        Console.WriteLine($"Отримано: {u1Repeat} (з кешу: {fromCache2})");

        Console.WriteLine("\nЗапит 3: ID = 102");
        string u2 = userCache.GetOrAdd(102, id => $"Користувач #{id} (Студент 2ПІ-24б)", out _);
        Console.WriteLine($"Отримано: {u2}");

        Console.WriteLine("\nЗапит 4: ID = 103 (переповнення місткості, видалення найстарішого)");
        string u3 = userCache.GetOrAdd(103, id => $"Користувач #{id} (Новий гість)", out _);
        Console.WriteLine($"Отримано: {u3}");

        Console.WriteLine("\nПоточний вміст другого кешу:");
        userCache.ForEach((id, name) => Console.WriteLine($"ID: {id} -> {name}"));

        return true;
    }

    private static bool HandleClearCache(Cache<string, decimal> cache)
    {
        cache.Clear();
        Console.WriteLine("Кеш курсів валют успішно очищено.");
        return true;
    }

    /// <summary>
    /// Імітація повільного звернення до сервера банку.
    /// Передається як делегат Func&lt;string, decimal&gt; у метод GetOrAdd.
    /// </summary>
    public static decimal LoadRate(string code)
    {
        Console.WriteLine($"[Сервер банку] Виконується запит курсу для валюти {code}...");

        if (BankDatabase.TryGetValue(code, out decimal rate))
        {
            return rate;
        }

        throw new KeyNotFoundException($"Валюта '{code}' не знайдена в базі банку.");
    }

    /// <summary>
    /// Іменований метод для обробки події ItemRemoved.
    /// </summary>
    private static void OnItemRemovedNamedMethod(object? sender, ItemRemovedEventArgs<string, decimal> e)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[Іменований підписник] Видалено з кешу найстаріший запис: {e.Key} = {e.Value:F2} UAH (час: {e.RemovedAt:HH:mm:ss})");
        Console.ResetColor();
    }

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
            Console.WriteLine("Введіть коректне додатне число.");
        }
    }
}
