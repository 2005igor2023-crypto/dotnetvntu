using System;
using System.Text;

namespace CSharpBasics;

internal class Program
{
    private static void Main()
    {
        // Налаштування коректного кодування консолі для відображення української мови
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Початкові тестові дані (бали студентів за тестування від 0 до 100)
        int[] scores = { 85, 92, 58, 74, 90, 63, 45, 99, 78, 88 };

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 7);

            Console.WriteLine();

            // Використання switch expression для керування пунктами меню
            isRunning = choice switch
            {
                1 => HandleAddScore(ref scores),
                2 => HandleShowAllScores(scores),
                3 => HandleShowAverage(scores),
                4 => HandleShowMinMax(scores),
                5 => HandleShowCategoryDistribution(scores),
                6 => HandleGenerateRandomScores(ref scores),
                7 => HandleClearScores(ref scores),
                0 => false,
                _ => true
            };

            if (isRunning)
            {
                Console.WriteLine("\nНатисніть Enter для повернення в меню...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("Програму успішно завершено. Гарного дня!");
    }

    // =========================================================================
    // Відображення меню
    // =========================================================================
    private static void PrintMenu()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    Практична робота №1 | Варіант 1   ");
        Console.WriteLine("          Статистика результатів тестування            ");
        Console.WriteLine("=======================================================");
        Console.WriteLine("1. Додати результат тестування (бал від 0 до 100)");
        Console.WriteLine("2. Показати всі результати");
        Console.WriteLine("3. Показати середній бал");
        Console.WriteLine("4. Показати найвищий і найнижчий бал (з позиціями)");
        Console.WriteLine("5. Показати розподіл за категоріями");
        Console.WriteLine("6. Згенерувати випадкові бали (тестовий набір)");
        Console.WriteLine("7. Очистити всі результати (перевірка порожнього масиву)");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("-------------------------------------------------------");
    }

    // =========================================================================
    // Обробники пунктів меню (Handle-методи)
    // =========================================================================

    private static bool HandleAddScore(ref int[] scores)
    {
        Console.WriteLine("--- Додавання нового результату ---");
        int newScore = ReadInt("Введіть бал студента (від 0 до 100): ", 0, 100);

        AddScore(ref scores, newScore);

        Console.WriteLine($"\n[Успішно] Результат {newScore} додано до списку (поточна кількість: {scores.Length}).");
        return true;
    }

    private static bool HandleShowAllScores(int[] scores)
    {
        Console.WriteLine("--- Список усіх результатів ---");

        if (scores.Length == 0)
        {
            Console.WriteLine("Немає даних. Масив результатів порожній.");
            return true;
        }

        Console.WriteLine($"Усього результатів: {scores.Length}");
        Console.Write("Бали: ");
        for (int i = 0; i < scores.Length; i++)
        {
            Console.Write($"[{i + 1}] {scores[i]}");
            if (i < scores.Length - 1)
            {
                Console.Write(", ");
            }
        }
        Console.WriteLine();

        return true;
    }

    private static bool HandleShowAverage(int[] scores)
    {
        Console.WriteLine("--- Середній бал тестування ---");

        if (scores.Length == 0)
        {
            Console.WriteLine("Немає даних. Неможливо обчислити середній бал для порожнього масиву.");
            return true;
        }

        double average = CalculateAverage(scores);
        Console.WriteLine($"Кількість робіт: {scores.Length}");
        Console.WriteLine($"Середній бал: {average:F2} із 100 можливих.");
        return true;
    }

    private static bool HandleShowMinMax(int[] scores)
    {
        Console.WriteLine("--- Найвищий та найнижчий результат ---");

        if (scores.Length == 0)
        {
            Console.WriteLine("Немає даних. Масив результатів порожній.");
            return true;
        }

        int maxIndex = FindMaxIndex(scores);
        int minIndex = FindMinIndex(scores);

        // Позиція виводиться у форматі індексу 0-based та номеру за порядком 1-based
        Console.WriteLine($"Найвищий бал: {scores[maxIndex]} (позиція №{maxIndex + 1}, індекс [{maxIndex}])");
        Console.WriteLine($"Найнижчий бал: {scores[minIndex]} (позиція №{minIndex + 1}, індекс [{minIndex}])");
        return true;
    }

    private static bool HandleShowCategoryDistribution(int[] scores)
    {
        Console.WriteLine("--- Розподіл результатів за академічними категоріями ---");

        if (scores.Length == 0)
        {
            Console.WriteLine("Немає даних. Масив результатів порожній.");
            return true;
        }

        int excellent = 0;   // 90+
        int good = 0;        // 75-89
        int satisfactory = 0;// 60-74
        int unsatisfactory = 0; // <60

        foreach (int score in scores)
        {
            string category = GetScoreCategory(score);
            switch (category)
            {
                case "відмінно":
                    excellent++;
                    break;
                case "добре":
                    good++;
                    break;
                case "задовільно":
                    satisfactory++;
                    break;
                case "незадовільно":
                    unsatisfactory++;
                    break;
            }
        }

        int total = scores.Length;

        Console.WriteLine($"• Відмінно    (90-100 балів): {excellent,3} ст. ({(double)excellent / total * 100:F1}%)");
        Console.WriteLine($"• Добре       (75-89  балів): {good,3} ст. ({(double)good / total * 100:F1}%)");
        Console.WriteLine($"• Задовільно  (60-74  бали ): {satisfactory,3} ст. ({(double)satisfactory / total * 100:F1}%)");
        Console.WriteLine($"• Незадовільно (<60   балів): {unsatisfactory,3} ст. ({(double)unsatisfactory / total * 100:F1}%)");
        Console.WriteLine($"--------------------------------------------------");
        Console.WriteLine($"Разом перевірено робіт: {total}");

        return true;
    }

    private static bool HandleGenerateRandomScores(ref int[] scores)
    {
        Console.WriteLine("--- Генерація випадкового набору балів ---");
        int count = ReadInt("Введіть бажану кількість студентів (від 1 до 100): ", 1, 100);

        scores = new int[count];
        Random rnd = Random.Shared;

        for (int i = 0; i < count; i++)
        {
            scores[i] = rnd.Next(0, 101); // від 0 до 100 включно
        }

        Console.WriteLine($"\n[Успішно] Згенеровано {count} випадкових результатів.");
        return true;
    }

    private static bool HandleClearScores(ref int[] scores)
    {
        Console.WriteLine("--- Очищення результатів ---");
        scores = Array.Empty<int>();
        Console.WriteLine("[Успішно] Масив результатів очищено. Тепер він порожній (0 елементів).");
        return true;
    }

    // =========================================================================
    // Робочі методи обчислень та обробки даних
    // =========================================================================

    /// <summary>
    /// Розширює масив та додає новий бал за допомогою Array.Resize.
    /// </summary>
    public static void AddScore(ref int[] scores, int newScore)
    {
        Array.Resize(ref scores, scores.Length + 1);
        scores[^1] = newScore;
    }

    /// <summary>
    /// Обчислює середнє арифметичне значення елементів масиву.
    /// Повертає 0, якщо масив порожній.
    /// </summary>
    public static double CalculateAverage(int[] scores)
    {
        if (scores.Length == 0)
        {
            return 0.0;
        }

        long sum = 0;
        foreach (int score in scores)
        {
            sum += score;
        }

        return (double)sum / scores.Length;
    }

    /// <summary>
    /// Знаходить індекс першого входження найбільшого бала в масиві.
    /// </summary>
    public static int FindMaxIndex(int[] scores)
    {
        if (scores.Length == 0)
        {
            return -1;
        }

        int maxIndex = 0;
        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i] > scores[maxIndex])
            {
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    /// <summary>
    /// Знаходить індекс першого входження найменшого бала в масиві.
    /// </summary>
    public static int FindMinIndex(int[] scores)
    {
        if (scores.Length == 0)
        {
            return -1;
        }

        int minIndex = 0;
        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i] < scores[minIndex])
            {
                minIndex = i;
            }
        }

        return minIndex;
    }

    /// <summary>
    /// Визначає текстову академічну категорію за балом за допомогою switch expression.
    /// </summary>
    public static string GetScoreCategory(int score) => score switch
    {
        >= 90 => "відмінно",
        >= 75 => "добре",
        >= 60 => "задовільно",
        _ => "незадовільно"
    };

    // =========================================================================
    // Допоміжне безпечне введення з консолі (TryParse)
    // =========================================================================

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка: введіть коректне ціле число в межах від {min} до {max}.");
            Console.ResetColor();
        }
    }
}
