using Optimizations;
using Points;
internal class Program
{
    private static void Main(string[] args)
    {
        //Метод градиентного спуска
        Point2D startPoint = new Point2D() { X = 100, Y = 100 };

        Optimizer optimizer = new Optimizer(
            x => GetFunction(x),
            x => Gradient(x),
            x => MatlabFunc(x)
            ); 

        Point2D resGradientDescent = optimizer.GradientDescent(startPoint, 0.5);
        Console.WriteLine($"Метод градиентного спуска: {resGradientDescent.X}, {resGradientDescent.Y}");

        //Метод Ньютона
        Point2D resNewton = optimizer.Newton(startPoint);
        Console.WriteLine($"Метод Ньютона: {resNewton.X}, {resNewton.Y}");

        // Метод сопряженных градиентов
        Point2D resConjugateGradient = optimizer.ConjugateGradient(startPoint);
        Console.WriteLine($"Метод сопряженных градиентов: {resConjugateGradient.X}, {resConjugateGradient.Y}");
    }
    #region Функция для оптимизации и вспомогательные функции
    /// <summary>
    /// Выдает значение функция, которую необходимо оптимизировать
    /// </summary>
    /// <param name="x">Точка, в которой ищется значение функции</param>
    /// <returns></returns>
    public static double GetFunction(Point2D x)
    {
        return 100 * (x.Y - x.X * x.X) * (x.Y - x.X * x.X)
            + 5 * (1 - x.X) * (1 - x.X);
    }

    /// <summary>
    /// Выдает значение следующего шага оптимизации с помощью метода Ньютона: x_new = x_prev - f(x_prev)/f'(x_prev)
    /// Данная функция подсчитана для конкретной функции из Fuction с помощью Matlab.
    /// </summary>
    /// <param name="x_prev">Предыдущая точка шага оптимизации</param>
    /// <returns></returns>
    public static Point2D MatlabFunc(Point2D x_prev)
    {
        return new Point2D()
        {
            X = x_prev.X + (2 * x_prev.X * (-100 * x_prev.X * x_prev.X + 100 * x_prev.Y)
            - 10 * x_prev.X + 200 * x_prev.X * (-x_prev.X * x_prev.X + x_prev.Y) + 10)
            / (10 * (40 * x_prev.X * x_prev.X - 40 * x_prev.Y + 1))
            - (x_prev.X * (-200 * x_prev.X * x_prev.X + 200 * x_prev.Y)) / (5 * (40 * x_prev.X * x_prev.X - 40 * x_prev.Y + 1)),

            Y = x_prev.Y + (x_prev.X * (2 * x_prev.X * (-100 * x_prev.X * x_prev.X + 100 * x_prev.Y) - 10 * x_prev.X + 200 * x_prev.X * (-x_prev.X * x_prev.X + x_prev.Y) + 10))
            / (5 * (40 * x_prev.X * x_prev.X - 40 * x_prev.Y + 1)) - ((-200 * x_prev.X * x_prev.X + 200 * x_prev.Y) * (120 * x_prev.X * x_prev.X - 40 * x_prev.Y + 1))
            / (200 * (40 * x_prev.X * x_prev.X - 40 * x_prev.Y + 1))
        };

    }

    /// <summary>
    /// Выдает значение градиента функции f'.
    /// Данная функция подсчитана для конкретной функции из Fuction с помощью Matlab.
    /// </summary>
    /// <param name="x">Точка, где считается градиент</param>
    /// <returns></returns>
    public static Point2D Gradient(Point2D x)
    {
        return new Point2D()
        {
            X = 10 * x.X - 2 * x.X *
            (-100 * x.X * x.X + 100 * x.Y) -
            200 * x.X * (-x.X * x.X + x.Y) - 10,

            Y = -200 * x.X * x.X + 200 * x.Y
        };
    }

    #endregion
}