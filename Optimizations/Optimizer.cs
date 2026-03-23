using Points;
using static System.Math;

namespace Optimizations;

/// <summary>
/// Объект решеающий оптимизайионную задачу
/// </summary>
public class Optimizer
{
    private readonly Func<Point2D, double> _f;
    private readonly Func<Point2D, Point2D> _grad;
    private readonly Func<Point2D, Point2D>? _newtonStep;

    public Optimizer(
        Func<Point2D, double> function,
        Func<Point2D, Point2D> gradient,
        Func<Point2D, Point2D>? newtonStep = null)
    {
        _f = function;
        _grad = gradient;
        _newtonStep = newtonStep;
    }

    /// <summary>
    /// Выдает расстояние между двумя точками
    /// </summary>
    /// <param name="x">Точка 1</param>
    /// <param name="y">Точка 2</param>
    /// <returns></returns>
    private static double GetDistance(Point2D x, Point2D y)
    {
        return (x - y).Norma();
    }

    #region Метод градиентного спуска
    /// <summary>
    /// Метод градиентного спуска с адаптивным подбором шага (backtracking).
    /// Начинает с точки x0 и движется в направлении антиградиента.
    /// Шаг уменьшается, пока не выполнится условие убывания функции.
    /// Остановка происходит, когда изменение точки и значения функции становится меньше tol.
    /// </summary>
    /// <param name="x0">Начальная точка</param>
    /// <param name="alpha">Начальный шаг</param>
    /// <param name="lamda">Коэффициент уменьшения шага (0 < lamda < 1)</param>
    /// <param name="tol">Точность (критерий остановки)</param>
    /// <param name="c">Коэффициент Армихо</param>
    /// <returns>Точка минимума функции</returns>
    public Point2D GradientDescent(Point2D x0, double alpha, 
        double lamda = 0.5, double c = 10e-4, double tol = 10e-6)
    {
        double step = alpha;
        Point2D x_k = x0;
        Point2D grad = _grad(x_k);
        Point2D x_m = new Point2D()
        {
            X = x_k.X - grad.X * step,
            Y = x_k.Y - grad.Y * step,
        };
        double f_k = _f(x_k);
        double f_m = _f(x_m);
        int iterations = 1;
        while (f_m - f_k > - c * step * Pow(grad.Norma(), 2))
        {
            step *= lamda;
            x_m = new Point2D()
            {
                X = x_k.X - grad.X * step,
                Y = x_k.Y - grad.Y * step,
            };
            f_m = _f(x_m);
        }

        while (GetDistance(x_m, x_k) > tol || (Abs(f_m - f_k) > tol) || grad.Norma() > tol)
        {
            step = alpha;
            x_k = x_m;
            grad = _grad(x_k);
            x_m = new Point2D()
            {
                X = x_k.X - grad.X * step,
                Y = x_k.Y - grad.Y * step,
            };
            f_k = _f(x_k);
            f_m = _f(x_m);
            iterations++;
            while (f_m - f_k > - c * step * Pow(grad.Norma(), 2))
            {
                step *= lamda;
                x_m = new Point2D()
                {
                    X = x_k.X - grad.X * step,
                    Y = x_k.Y - grad.Y * step,
                };
                f_m = _f(x_m);
            }
        }

        Console.WriteLine(iterations);
        return x_m;
    }
    #endregion

    #region Метод сопряженных градиентов
    /// <summary>
    /// Метод сопряжённых градиентов для минимизации функции двух переменных.
    /// Использует направления, сопряжённые относительно гессиана,
    /// что ускоряет сходимость по сравнению с обычным градиентным спуском.
    /// Включает backtracking для подбора шага.
    /// Периодически сбрасывает направление к антиградиенту.
    /// </summary>
    /// <param name="x0">Начальная точка</param>
    /// <param name="lamda">Коэффициент уменьшения шага</param>
    /// <param name="alpha">Начальный шаг</param>
    /// <param name="eps">Точность (критерий остановки)</param>
    /// <returns>Точка минимума функции</returns>
    public Point2D ConjugateGradient(Point2D x0, double lamda = 0.5, 
        double alpha = 1.0, double eps = 10e-6)
    {
        double step = alpha;
        Point2D x_prev = x0;
        Point2D grad = _grad(x_prev);
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
        double f_k = _f(x_prev);
        double f_m = _f(x_new);
        int iterations = 1;
        while (f_m - f_k > -eps * step * Pow(grad.Norma(), 2))
        {
            step *= lamda;
            x_new = new Point2D()
            {
                X = x_prev.X + p.X * step,
                Y = x_prev.Y + p.Y * step
            };
            f_m = _f(x_new);
        }

        Point2D grad_next = _grad(x_new);
        double beta = grad_next.Dot(grad_next - grad) / Pow(grad.Norma(), 2);
        p = new Point2D()
        {
            X = -grad_next.X + beta * grad.X,
            Y = -grad_next.Y + beta * grad.Y
        };
        grad = grad_next;

        double count = 0;

        while (grad_next.Norma() > eps && GetDistance(x_new, x_prev) > eps)
        {
            step = alpha;
            x_prev = x_new;
            x_new = new Point2D()
            {
                X = x_prev.X + p.X * step,
                Y = x_prev.Y + p.Y * step
            };
            f_k = _f(x_prev);
            f_m = _f(x_new);
            while (f_m - f_k > -eps * step * Pow(grad.Norma(), 2))
            {
                step *= lamda;
                x_new = new Point2D()
                {
                    X = x_prev.X + p.X * step,
                    Y = x_prev.Y + p.Y * step
                };
                f_m = _f(x_new);
            }

            grad_next = _grad(x_new);
            beta = grad_next.Dot(grad_next - grad) / Pow(grad.Norma(), 2);
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
    /// <summary>
    /// Метод Ньютона для поиска минимума функции.
    /// Использует информацию о второй производной (гессиан),
    /// что обеспечивает быструю (квадратичную) сходимость при хорошей начальной точке.
    /// Следующая точка вычисляется через MatlabFunc (предположительно решение системы).
    /// Остановка происходит при малом изменении точки и значения функции.
    /// </summary>
    /// <param name="x0">Начальная точка</param>
    /// <param name="tol">Точность (критерий остановки)</param>
    /// <returns>Точка минимума функции</returns>
    public Point2D Newton(Point2D x0, double tol = 10e-6)
    {
        Point2D x_m = x0;
        Point2D x_k = new Point2D()
        {
            X = x_m.X * 2,
            Y = x_m.Y * 2
        };
        double f_m = _f(x_m);
        double f_k = _f(x_k);
        int iterations = 1;
        while (GetDistance(x_m, x_k) > tol || (Abs(f_m - f_k) > tol))
        {
            x_m = x_k;
            x_k = _newtonStep!(x_m);
            f_m = _f(x_m);
            f_k = _f(x_k);
            iterations++;
        }
        Console.WriteLine(iterations);
        return x_k;
    }
    #endregion
}

