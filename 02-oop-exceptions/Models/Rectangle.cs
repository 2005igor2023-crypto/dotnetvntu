using System;
using OopExceptions.Exceptions;

namespace OopExceptions.Models;

/// <summary>
/// Похідний клас для геометричної фігури "Прямокутник" (включаючи окремий випадок "Квадрат").
/// </summary>
public class Rectangle : Shape
{
    private double _width;
    private double _height;

    /// <summary>
    /// Ширина прямокутника.
    /// Валідація через власний виняток InvalidShapeDimensionException при значенні <= 0.
    /// </summary>
    public double Width
    {
        get => _width;
        set
        {
            if (value <= 0)
            {
                throw new InvalidShapeDimensionException("Ширина прямокутника", value);
            }
            _width = value;
        }
    }

    /// <summary>
    /// Висота прямокутника.
    /// Валідація через власний виняток InvalidShapeDimensionException при значенні <= 0.
    /// </summary>
    public double Height
    {
        get => _height;
        set
        {
            if (value <= 0)
            {
                throw new InvalidShapeDimensionException("Висота прямокутника", value);
            }
            _height = value;
        }
    }

    /// <summary>
    /// Конструктор прямокутника за шириною та висотою.
    /// </summary>
    public Rectangle(double width, double height)
        : base("Прямокутник")
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Перевантажений конструктор для окремого випадку прямокутника - квадрата (рівні сторони).
    /// </summary>
    public Rectangle(double side)
        : base("Квадрат")
    {
        Width = side;
        Height = side;
    }

    /// <summary>
    /// Перевантажений конструктор із можливістю задати довільну назву, ширину та висоту.
    /// </summary>
    public Rectangle(string name, double width, double height)
        : base(name)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Розрахунок площі прямокутника: S = a * b
    /// </summary>
    public override double GetArea() => Width * Height;

    /// <summary>
    /// Розрахунок периметра прямокутника: P = 2 * (a + b)
    /// </summary>
    public override double GetPerimeter() => 2 * (Width + Height);

    /// <summary>
    /// Перевизначення віртуального методу виведення інформації з розширенням специфічними даними прямокутника.
    /// </summary>
    public override void PrintInfo()
    {
        Console.WriteLine($"[Прямокутник] Назва: {Name,-10} | {Width:F2} x {Height:F2,-5} | Площа: {GetArea(),8:F2} | Периметр: {GetPerimeter(),8:F2}");
    }
}
