using Points;
using static System.Math;

namespace Optimizations;

/// <summary>
/// Объект, решающий оптимизационную задачу
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

    #region Одномерная оптимизация
    private double FindOptimalStep(Point2D x_prev,
    Point2D grad, double c, double alpha, double lambda)
    {
        double step = alpha;
        Point2D x_find = new Point2D()
        {
            X = x_prev.X - grad.X * step,
            Y = x_prev.Y - grad.Y * step
        };

        double f_k = _f(x_prev);
        double f_m = _f(x_find);

        while (f_m - f_k > -c * step * Pow(grad.Norma(), 2))
        {
            step *= lambda;
            x_find = new Point2D()
            {
                X = x_prev.X - grad.X * step,
                Y = x_prev.Y - grad.Y * step,
            };
            f_m = _f(x_find);
        }

        return step;
    }
    #endregion

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
    /// <returns>Точка минимума функции, минимальное значение функции, кол-во итераций</returns>
    public OptimizationResult GradientDescent(Point2D x0, double alpha, 
        double lamda = 0.5, double c = 10e-4, double tol = 10e-6)
    {
        Point2D x_prev;
        double f_prev;

        Point2D x_next = x0;
        double f_next = _f(x_next);
        Point2D grad = _grad(x_next);

        int iterationCount = 0;

        do
        {
            x_prev = x_next;
            f_prev = f_next;
            double step = FindOptimalStep(x_prev, grad, c, alpha, lamda);

            x_next = x_prev - step * grad;
            f_next = _f(x_next);
            grad = _grad(x_next);
            iterationCount++;
        } while (GetDistance(x_next, x_prev) > tol || Abs(f_prev - f_next) > tol || grad.Norma() > tol);

        return new OptimizationResult()
        {
            MinFunctionValue = f_next,
            MinPoint = x_next,
            IterationCount = iterationCount
        };
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
    /// <returns>Точка минимума функции, минимальное значение функции, кол-во итераций</returns>
    public OptimizationResult ConjugateGradient(Point2D x0, double lamda = 0.5, 
        double alpha = 1.0, double c = 10e-4, double eps = 10e-6)
    {
        Point2D x_prev;
        double f_prev;

        Point2D x_next = x0;
        double f_next = _f(x_next);
        Point2D grad = _grad(x_next);
        Point2D p = -grad;

        int iterationCount = 0;

        do
        { 
            x_prev = x_next;
            f_prev = f_next;   
            double step = FindOptimalStep(x_prev, -p, c, alpha, lamda);

            x_next = x_prev + p * step;
            f_next = _f(x_next);
            
            Point2D grad_next = _grad(x_next);
            double beta = grad_next.Dot(grad_next - grad) / Pow(grad.Norma(), 2);
            p = -grad_next + beta * p;
            grad = grad_next;

            iterationCount++;
            if (iterationCount % 2 == 0)
            {
                p = -grad;
            }
        } while (grad.Norma() > eps || GetDistance(x_next, x_prev) > eps || Abs(f_next - f_prev) > eps) ;

        return new OptimizationResult()
        {
            MinFunctionValue = f_next,
            MinPoint = x_next,
            IterationCount = iterationCount
        };
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
    /// <returns>Точка минимума функции, минимальное значение функции, кол-во итераций</returns>
    public OptimizationResult Newton(Point2D x0, double tol = 10e-6)
    {
        if (_newtonStep == null)
            throw new InvalidOperationException("Newton step is not provided");

        Point2D x_m = x0;
        Point2D x_k = new Point2D()
        {
            X = x_m.X * 2,
            Y = x_m.Y * 2
        };

        double f_m = _f(x_m);
        double f_k = _f(x_k);
        int iterationCount = 1;
        while (GetDistance(x_m, x_k) > tol || (Abs(f_m - f_k) > tol) || _grad(x_k).Norma() > tol)
        {
            x_m = x_k;
            x_k = _newtonStep!(x_m);
            f_m = _f(x_m);
            f_k = _f(x_k);
            iterationCount++;
        }

        return new OptimizationResult()
        {
            MinFunctionValue = f_k,
            MinPoint = x_k,
            IterationCount = iterationCount
        };
    }
    #endregion
}