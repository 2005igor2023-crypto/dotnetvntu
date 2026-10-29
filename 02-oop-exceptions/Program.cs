using System;
using System.Collections.Generic;
using System.Text;
using OopExceptions.Exceptions;
using OopExceptions.Models;

namespace OopExceptions;

internal class Program
{
    private static void Main()
    {
        // Налаштування UTF-8 для коректного виведення кирилиці в консолі
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Початкова поліморфна колекція фігур базового типу Shape,
        // яка містить об'єкти похідних класів Circle та Rectangle (демонстрація поліморфізму)
        List<Shape> shapes = new()
        {
            new Circle("Мале коло", 3.0),
            new Rectangle("Прямокутник А", 4.0, 5.0),
            new Circle("Велике коло", 7.0),
            new Rectangle(6.0), // Створення квадрата через перевантажений конструктор
            new Rectangle("Пластина", 2.5, 8.0)
        };

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 7);

            Console.WriteLine();

            isRunning = choice switch
            {
                1 => HandleAddShape(shapes),
                2 => HandleShowAllShapes(shapes),
                3 => HandleSortByArea(shapes),
                4 => HandleShowTotalStats(shapes),
                5 => HandleFilterByType(shapes),
                6 => HandleDemonstrateExceptionHandling(),
                7 => HandleClearShapes(shapes),
                0 => false,
                _ => true
            };

            if (isRunning)
            {
                Console.WriteLine("\nНатисніть Enter для повернення в меню...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("Програму завершено. Успіхів у навчанні!");
    }

    // =========================================================================
    // Відображення меню
    // =========================================================================
    private static void PrintMenu()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    Практична робота №2 | Варіант 1   ");
        Console.WriteLine("         Геометричні фігури (ООП та виключення)        ");
        Console.WriteLine("=======================================================");
        Console.WriteLine("1. Додати фігуру (коло або прямокутник)");
        Console.WriteLine("2. Показати всі фігури колекції (поліморфний вивід)");
        Console.WriteLine("3. Відсортувати фігури за площею (IComparable<Shape>)");
        Console.WriteLine("4. Показати сумарну площу та сумарний периметр");
        Console.WriteLine("5. Фільтрувати фігури за типом (pattern matching 'is')");
        Console.WriteLine("6. Продемонструвати обробку винятку InvalidShapeDimensionException");
        Console.WriteLine("7. Очистити колекцію (перевірка порожнього списку)");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("-------------------------------------------------------");
    }

    // =========================================================================
    // Обробники пунктів меню
    // =========================================================================

    private static bool HandleAddShape(List<Shape> shapes)
    {
        Console.WriteLine("--- Додавання нової геометричної фігури ---");
        Console.WriteLine("Оберіть тип фігури:");
        Console.WriteLine("1. Коло");
        Console.WriteLine("2. Прямокутник / Квадрат");
        int typeChoice = ReadInt("Вибір типу (1 або 2): ", 1, 2);

        Console.Write("Введіть назву фігури (або натисніть Enter для назви за замовчуванням): ");
        string? customName = Console.ReadLine()?.Trim();

        try
        {
            if (typeChoice == 1)
            {
                double radius = ReadDouble("Введіть радіус кола: ");
                Circle newCircle = string.IsNullOrWhiteSpace(customName)
                    ? new Circle(radius)
                    : new Circle(customName, radius);

                shapes.Add(newCircle);
                Console.WriteLine($"\n[Успішно] Фігуру '{newCircle.Name}' додано до колекції.");
            }
            else
            {
                double width = ReadDouble("Введіть ширину прямокутника: ");
                double height = ReadDouble("Введіть висоту прямокутника: ");

                Rectangle newRect = string.IsNullOrWhiteSpace(customName)
                    ? (Math.Abs(width - height) < 0.0001 ? new Rectangle(width) : new Rectangle(width, height))
                    : new Rectangle(customName, width, height);

                shapes.Add(newRect);
                Console.WriteLine($"\n[Успішно] Фігуру '{newRect.Name}' додано до колекції.");
            }
        }
        catch (InvalidShapeDimensionException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[Перехоплено власний виняток InvalidShapeDimensionException]");
            Console.WriteLine($"Деталі: {ex.Message}");
            if (ex.DimensionName is not null)
            {
                Console.WriteLine($"Вимір: {ex.DimensionName}, некоректне значення: {ex.InvalidValue}");
            }
            Console.ResetColor();
        }
        catch (ArgumentException ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[Помилка аргументу]: {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[Непередбачена помилка]: {ex.Message}");
            Console.ResetColor();
        }
        finally
        {
            Console.WriteLine("(Завершено блок спроби додавання фігури)");
        }

        return true;
    }

    private static bool HandleShowAllShapes(List<Shape> shapes)
    {
        Console.WriteLine("--- Список усіх фігур у колекції (демонстрація поліморфізму) ---");

        if (shapes.Count == 0)
        {
            Console.WriteLine("Немає даних. Колекція фігур порожня.");
            return true;
        }

        Console.WriteLine($"Усього фігур у колекції: {shapes.Count}\n");

        int index = 1;
        // Демонстрація поліморфізму: через посилання типу Shape викликається
        // перевизначений метод PrintInfo() конкретного похідного класу (Circle/Rectangle)
        foreach (Shape shape in shapes)
        {
            Console.Write($"{index++,2}. ");
            shape.PrintInfo();
        }

        return true;
    }

    private static bool HandleSortByArea(List<Shape> shapes)
    {
        Console.WriteLine("--- Сортування фігур за площею (IComparable<Shape>) ---");

        if (shapes.Count == 0)
        {
            Console.WriteLine("Немає даних. Колекція порожня, сортування неможливе.");
            return true;
        }

        // List<T>.Sort() використовує метод CompareTo, визначений в інтерфейсі IComparable<Shape>
        shapes.Sort();

        Console.WriteLine("[Успішно] Колекцію відсортовано за зростанням площі:\n");

        int index = 1;
        foreach (Shape shape in shapes)
        {
            Console.WriteLine($"{index++,2}. {shape.Name,-15} | Площа: {shape.GetArea(),8:F2} | Периметр: {shape.GetPerimeter(),8:F2}");
        }

        return true;
    }

    private static bool HandleShowTotalStats(List<Shape> shapes)
    {
        Console.WriteLine("--- Підсумкова статистика колекції ---");

        if (shapes.Count == 0)
        {
            Console.WriteLine("Немає даних. Колекція фігур порожня.");
            return true;
        }

        double totalArea = 0;
        double totalPerimeter = 0;

        foreach (Shape shape in shapes)
        {
            totalArea += shape.GetArea();
            totalPerimeter += shape.GetPerimeter();
        }

        Console.WriteLine($"• Кількість фігур: {shapes.Count}");
        Console.WriteLine($"• Сумарна площа  : {totalArea:F2}");
        Console.WriteLine($"• Сумарний периметр: {totalPerimeter:F2}");
        Console.WriteLine($"• Середня площа фігури: {(totalArea / shapes.Count):F2}");

        return true;
    }

    private static bool HandleFilterByType(List<Shape> shapes)
    {
        Console.WriteLine("--- Фільтрація фігур за типом (Pattern Matching) ---");

        if (shapes.Count == 0)
        {
            Console.WriteLine("Немає даних. Колекція фігур порожня.");
            return true;
        }

        Console.WriteLine("1. Тільки кола (Circle)");
        Console.WriteLine("2. Тільки прямокутники (Rectangle)");
        int filterChoice = ReadInt("Ваш вибір (1 або 2): ", 1, 2);

        int count = 0;
        Console.WriteLine();

        foreach (Shape shape in shapes)
        {
            if (filterChoice == 1 && shape is Circle circle)
            {
                circle.PrintInfo();
                count++;
            }
            else if (filterChoice == 2 && shape is Rectangle rectangle)
            {
                rectangle.PrintInfo();
                count++;
            }
        }

        if (count == 0)
        {
            Console.WriteLine("Фігур обраного типу в колекції не знайдено.");
        }
        else
        {
            Console.WriteLine($"Знайдено фігур: {count}");
        }

        return true;
    }

    private static bool HandleDemonstrateExceptionHandling()
    {
        Console.WriteLine("--- Демонстрація генерації та перехоплення винятку ---");
        Console.WriteLine("Спробуємо навмисно створити коло з радіусом -5.0 та прямокутник з висотою 0:");

        // Демонстрація 1: Спроба створення кола з від'ємним радіусом
        try
        {
            Console.WriteLine("\n[Спроба 1] new Circle(-5.0)...");
            _ = new Circle(-5.0);
            Console.WriteLine("Цей рядок не виконається через виняток.");
        }
        catch (InvalidShapeDimensionException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"-> Успішно перехоплено InvalidShapeDimensionException: {ex.Message}");
            Console.ResetColor();
        }
        finally
        {
            Console.WriteLine("-> Виконано блок finally для Спроби 1.");
        }

        // Демонстрація 2: Спроба створення прямокутника з нульовою висотою
        try
        {
            Console.WriteLine("\n[Спроба 2] new Rectangle(10.0, 0.0)...");
            _ = new Rectangle(10.0, 0.0);
            Console.WriteLine("Цей рядок не виконається через виняток.");
        }
        catch (InvalidShapeDimensionException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"-> Успішно перехоплено InvalidShapeDimensionException: {ex.Message}");
            Console.ResetColor();
        }
        finally
        {
            Console.WriteLine("-> Виконано блок finally для Спроби 2.");
        }

        Console.WriteLine("\nЯк бачимо, програма не завершилась аварійно, а коректно обробила виняткові ситуації.");
        return true;
    }

    private static bool HandleClearShapes(List<Shape> shapes)
    {
        Console.WriteLine("--- Очищення колекції фігур ---");
        shapes.Clear();
        Console.WriteLine("[Успішно] Усі фігури видалено з колекції. Колекція порожня (0 елементів).");
        return true;
    }

    // =========================================================================
    // Допоміжні методи валідованого введення
    // =========================================================================

    private static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
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
            Console.WriteLine($"Помилка: введіть ціле число в межах від {min} до {max}.");
            Console.ResetColor();
        }
    }

    private static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            // Підтримка введення як з крапкою, так і з комою
            if (input is not null)
            {
                input = input.Replace(',', '.');
            }

            if (double.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, out double value))
            {
                return value;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Помилка: введіть коректне дробове або ціле число.");
            Console.ResetColor();
        }
    }
}
