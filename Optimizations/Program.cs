using Optimizations;
using static System.Math;
internal class Program
{
    private static void Main(string[] args)
    {
        //Метод градиентного спуска
        Point2D startPoint = new Point2D() { X = 100, Y = 100 };
        int functionNumber = 0;
        Point2D resGradientDescent = gradientDescent(startPoint, 0.5, functionNumber);
        Console.WriteLine($"Метод градиентного спуска: {resGradientDescent.X}, {resGradientDescent.Y}");

        //Метод Ньютона
        Point2D resNewton = NewtonRun(startPoint, functionNumber);
        Console.WriteLine($"Метод Ньютона: {resNewton.X}, {resNewton.Y}");

        // Метод сопряженных градиентов
        Point2D resConjugateGradient = ConjugateGradient(startPoint, functionNumber);
        Console.WriteLine($"Метод сопряженных градиентов: {resConjugateGradient.X}, {resConjugateGradient.Y}");
    }

    #region Функции для оптимизации и вспомогательные функции
    public static double Function(Point2D x, int numberFunction)
    {
        if (numberFunction == 0)
        {
            return 100 * (x.Y - x.X * x.X) * (x.Y - x.X * x.X)
                + 5 * (1 - x.X) * (1 - x.X);
        }
        else
        {
            return (x.X * x.X + x.Y - 11) * (x.X * x.X + x.Y - 11)
                + (x.X + x.Y * x.Y - 7) * (x.X + x.Y * x.Y - 7);
        }
    }

    public static double Distance(Point2D x, Point2D y)
    {
        return Sqrt((x.X - y.X) * (x.X - y.X) + (x.Y - y.Y) * (x.Y - y.Y));
    }

    public static double Norma(Point2D x)
    {
        return Sqrt(x.X * x.X + x.Y * x.Y);
    }

    public static Point2D MatlabFunc(Point2D x_prev, int numberFunction)
    {
        if (numberFunction == 0)
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
        else
        {
            return new Point2D()
            {
                X = x_prev.X - ((x_prev.X + x_prev.Y) * (2 * x_prev.Y + 4 * x_prev.Y * (x_prev.Y * x_prev.Y + x_prev.X - 7) + 2 * x_prev.X * x_prev.X - 22))
                / (-12 * x_prev.X * x_prev.X * x_prev.X - 36 * x_prev.X * x_prev.X * x_prev.Y * x_prev.Y + 82 * x_prev.X * x_prev.X + 4 * x_prev.X * x_prev.Y + 42 * x_prev.X - 12 * x_prev.Y * x_prev.Y * x_prev.Y + 130 * x_prev.Y * x_prev.Y + 26 * x_prev.Y - 273)
                + ((6 * x_prev.Y * x_prev.Y + 2 * x_prev.X - 13) * (2 * x_prev.X + 4 * x_prev.X * (x_prev.X * x_prev.X + x_prev.Y - 11) + 2 * x_prev.Y * x_prev.Y - 14))
                / (2 * (-12 * x_prev.X * x_prev.X * x_prev.X - 36 * x_prev.X * x_prev.X * x_prev.Y * x_prev.Y + 82 * x_prev.X * x_prev.X + 4 * x_prev.X * x_prev.Y + 42 * x_prev.X - 12 * x_prev.Y * x_prev.Y * x_prev.Y + 130 * x_prev.Y * x_prev.Y + 26 * x_prev.Y - 273)),

                Y = x_prev.Y - ((x_prev.X + x_prev.Y) * (2 * x_prev.X + 4 * x_prev.X * (x_prev.X * x_prev.X + x_prev.Y - 11) + 2 * x_prev.Y * x_prev.Y - 14))
                / (-12 * x_prev.X * x_prev.X * x_prev.X - 36 * x_prev.X * x_prev.X * x_prev.Y * x_prev.Y + 82 * x_prev.X * x_prev.X + 4 * x_prev.X * x_prev.Y + 42 * x_prev.X - 12 * x_prev.Y * x_prev.Y * x_prev.Y + 130 * x_prev.Y * x_prev.Y + 26 * x_prev.Y - 273)
                + ((6 * x_prev.X * x_prev.X + 2 * x_prev.Y - 21) * (2 * x_prev.Y + 4 * x_prev.Y * (x_prev.Y * x_prev.Y + x_prev.X - 7) + 2 * x_prev.X * x_prev.X - 22))
                / (2 * (-12 * x_prev.X * x_prev.X * x_prev.X - 36 * x_prev.X * x_prev.X * x_prev.Y * x_prev.Y + 82 * x_prev.X * x_prev.X + 4 * x_prev.X * x_prev.Y + 42 * x_prev.X - 12 * x_prev.Y * x_prev.Y * x_prev.Y + 130 * x_prev.Y * x_prev.Y + 26 * x_prev.Y - 273))
            };
        }
    }

    public static Point2D Gradient(Point2D x, int numberFunction)
    {
        if (numberFunction == 0)
        {
            return new Point2D()
            {
                X = 10 * x.X - 2 * x.X *
                (-100 * x.X * x.X + 100 * x.Y) -
                200 * x.X * (-x.X * x.X + x.Y) - 10,

                Y = -200 * x.X * x.X + 200 * x.Y
            };
        }
        else
        {
            return new Point2D()
            {
                X = 2 * x.X + 4 * x.X * (x.X * x.X + x.Y - 11) + 2 * x.Y * x.Y - 14,
                Y = 2 * x.Y + 4 * x.Y * (x.Y * x.Y + x.X - 7) + 2 * x.X * x.X - 22
            };
        }
    }

    #endregion

    #region Метод градиентного спуска
    public static Point2D gradientDescent(Point2D x0, double alpha, int functionNumber, double lamda = 0.5, double tol = 10e-6)
    {
        double step = alpha;
        Point2D x_k = x0;
        Point2D grad = Gradient(x_k, functionNumber);
        Point2D x_m = new Point2D()
        {
            X = x_k.X - grad.X * step,
            Y = x_k.Y - grad.Y * step,
        };
        double f_k = Function(x_k, functionNumber);
        double f_m = Function(x_m, functionNumber);
        int iterations = 1;
        while (f_m - f_k > -tol * step * Norma(grad) * Norma(grad))
        {
            step *= lamda;
            x_m = new Point2D()
            {
                X = x_k.X - grad.X * step,
                Y = x_k.Y - grad.Y * step,
            };
            f_m = Function(x_m, functionNumber);
        }

        while (Distance(x_m, x_k) > tol || (Abs(f_m - f_k) > tol))
        {
            step = alpha;
            x_k = x_m;
            x_m = new Point2D()
            {
                X = x_k.X - grad.X * step,
                Y = x_k.Y - grad.Y * step,
            };
            f_k = Function(x_k, functionNumber);
            f_m = Function(x_m, functionNumber);
            grad = Gradient(x_k, functionNumber);
            iterations++;
            while (f_m - f_k > -tol * step * Norma(grad) * Norma(grad))
            {
                step *= lamda;
                x_m = new Point2D()
                {
                    X = x_k.X - grad.X * step,
                    Y = x_k.Y - grad.Y * step,
                };
                f_m = Function(x_m, functionNumber);
            }
        }

        Console.WriteLine(iterations);
        return x_m;
    }
    #endregion

    #region Метод сопряженных градиентов
    public static Point2D ConjugateGradient(Point2D x0, int functionNumber, double lamda = 0.5, double alpha = 1.0, double eps = 10e-6)
    {
        double step = alpha;
        Point2D x_prev = x0;
        Point2D grad = Gradient(x_prev, functionNumber);
        Point2D p = new Point2D()
        {
            X = -grad.X,
            Y = -grad.Y
        };
        Point2D x_new = new Point2D()
        {
            X = x_prev.X + p.X * step,
            Y = x_prev.Y + p.Y * step
        };
        double f_k = Function(x_prev, functionNumber);
        double f_m = Function(x_new, functionNumber);
        int iterations = 1;
        while (f_m - f_k > -eps * step * Norma(grad) * Norma(grad))
        {
            step *= lamda;
            x_new = new Point2D()
            {
                X = x_prev.X + p.X * step,
                Y = x_prev.Y + p.Y * step
            };
            f_m = Function(x_new, functionNumber);
        }

        Point2D grad_next = Gradient(x_new, functionNumber);
        double beta = Norma(grad_next) / Norma(grad);
        p = new Point2D()
        {
            X = -grad_next.X + beta * grad.X,
            Y = -grad_next.Y + beta * grad.Y
        };
        grad = grad_next;

        double count = 0;

        while (Norma(grad_next) > eps && Distance(x_new, x_prev) > eps)
        {
            step = alpha;
            x_prev = x_new;
            x_new = new Point2D()
            {
                X = x_prev.X + p.X * step,
                Y = x_prev.Y + p.Y * step
            };
            f_k = Function(x_prev, functionNumber);
            f_m = Function(x_new, functionNumber);
            while (f_m - f_k > -eps * step * Norma(grad) * Norma(grad))
            {
                step *= lamda;
                x_new = new Point2D()
                {
                    X = x_prev.X + p.X * step,
                    Y = x_prev.Y + p.Y * step
                };
                f_m = Function(x_new, functionNumber);
            }

            grad_next = Gradient(x_new, functionNumber);
            beta = Norma(grad_next) / Norma(grad);
            p = new Point2D()
            {
                X = -grad_next.X + beta * grad.X,
                Y = -grad_next.Y + beta * grad.Y
            };
            grad = grad_next;
            count++;
            iterations++;
            if (count == 1)
            {
                p = new Point2D()
                {
                    X = -grad_next.X,
                    Y = -grad_next.Y
                };
                count = 0;
            }
        }

        Console.WriteLine(iterations);
        return x_new;
    }

    #endregion

    #region Метод Ньютона
    public static Point2D NewtonRun(Point2D x0, int functionNumber, double tol = 10e-6)
    {
        Point2D x_m = x0;
        Point2D x_k = new Point2D()
        {
            X = x_m.X * 2,
            Y = x_m.Y * 2
        };
        double f_m = Function(x_m, functionNumber);
        double f_k = Function(x_k, functionNumber);
        int iterations = 1;
        while (Distance(x_m, x_k) > tol || (Abs(f_m - f_k) > tol))
        {
            x_m = x_k;
            x_k = MatlabFunc(x_m, functionNumber);
            f_m = Function(x_m, functionNumber);
            f_k = Function(x_k, functionNumber);
            iterations++;
        }
        Console.WriteLine(iterations);
        return x_k;
    }
    #endregion
}