using LinearRegression.Web.Models;

namespace LinearRegression.Web.Services;

// Teammate's engine can replace this class
public sealed class BasicRegressionEngine : IRegressionEngine
{
    public RegressionResult Compute(IReadOnlyList<DataPoint> points)
    {
        if (points.Count == 0)
        {
            return InvalidResult();
        }

        var meanX = points.Average(point => point.X);
        var meanY = points.Average(point => point.Y);
        var sumXY = points.Sum(point => (point.X - meanX) * (point.Y - meanY));
        var sumXX = points.Sum(point => Math.Pow(point.X - meanX, 2));

        if (points.Count < 2 || sumXX == 0 || !double.IsFinite(sumXX) || !double.IsFinite(sumXY))
        {
            return new RegressionResult(meanX, meanY, sumXY, sumXX, 0, 0, false, []);
        }

        var b = sumXY / sumXX;
        var a = meanY - b * meanX;
        if (!double.IsFinite(a) || !double.IsFinite(b))
        {
            return new RegressionResult(meanX, meanY, sumXY, sumXX, 0, 0, false, []);
        }

        var rows = points.Select(point =>
        {
            var predicted = a + b * point.X;
            return new PointRow(point.Id, predicted, point.Y - predicted);
        }).ToList();

        return new RegressionResult(meanX, meanY, sumXY, sumXX, b, a, true, rows);
    }

    private static RegressionResult InvalidResult() => new(0, 0, 0, 0, 0, 0, false, []);
}
