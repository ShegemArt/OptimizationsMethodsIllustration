using Points;

namespace Optimizations;

public class OptimizationResult
{
    public required Point2D MinPoint {  get; init; }

    public required int IterationCount { get; init; }

    public required double MinFunctionValue { get; init; }
}
