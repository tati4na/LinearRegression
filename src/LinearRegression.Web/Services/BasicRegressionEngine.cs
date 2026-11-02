using LinearRegression.Core.Table;
using LinearRegression.Web.Models;

namespace LinearRegression.Web.Services;

// Teammate's engine can replace this class
public sealed class BasicRegressionEngine : IRegressionEngine
{
    public RegressionResult Compute(IReadOnlyList<DataPoint> points)
    {
        var table = RegressionTable.Calculate(
            points.Select(point => new RegressionTableInput(point.Id, point.X, point.Y)).ToArray());

        if (points.Count == 0)
        {
            return InvalidResult();
        }

        if (table.Slope is not double slope || table.Intercept is not double intercept)
        {
            return new RegressionResult(table.MeanX, table.MeanY, table.SumDeviationProduct, table.SumSquaredXDeviation, 0, 0, false, []);
        }

        var rows = table.Rows
            .Select(row => new PointRow(row.Id, row.PredictedY!.Value, row.Y - row.PredictedY.Value))
            .ToList();

        return new RegressionResult(table.MeanX, table.MeanY, table.SumDeviationProduct, table.SumSquaredXDeviation, slope, intercept, true, rows);
    }

    private static RegressionResult InvalidResult() => new(0, 0, 0, 0, 0, 0, false, []);
}
