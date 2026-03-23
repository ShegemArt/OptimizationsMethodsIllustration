using static System.Math;
namespace Points;

public class Point2D
{
    /// <summary>
    /// Значение координаты x
    /// </summary>
    public required double X { get; init; }

    /// <summary>
    /// Значение координаты y
    /// </summary>
    public required double Y { get; init; }

    /// <summary>
    /// Норма (длина) вектора
    /// </summary>
    public double Norma()
    {
        return Sqrt(X * X + Y * Y);
    }

    /// <summary>
    /// Скалярное произведение
    /// </summary>
    public double Dot(Point2D b)
    {
        return X * b.X + Y * b.Y;
    }

    /// <summary>
    /// Сложение векторов
    /// </summary>
    public static Point2D operator +(Point2D a, Point2D b)
    {
        return new Point2D
        {
            X = a.X + b.X,
            Y = a.Y + b.Y
        };
    }

    /// <summary>
    /// Вычитание векторов
    /// </summary>
    public static Point2D operator -(Point2D a, Point2D b)
    {
        return new Point2D
        {
            X = a.X - b.X,
            Y = a.Y - b.Y
        };
    }

    /// <summary>
    /// Унарный минус (−a)
    /// </summary>
    public static Point2D operator -(Point2D a)
    {
        return new Point2D
        {
            X = -a.X,
            Y = -a.Y
        };
    }

    /// <summary>
    /// Умножение вектора на число (a * k)
    /// </summary>
    public static Point2D operator *(Point2D a, double k)
    {
        return k * a;
    }

    /// <summary>
    /// Умножение числа на вектор (k * a)
    /// </summary>
    public static Point2D operator *(double k, Point2D a)
    {
        
        return new Point2D
        {
            X = k * a.X,
            Y = k * a.Y
        };
    }
}