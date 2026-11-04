namespace LinearRegression.Core.Table;

public sealed record RegressionTableInput(int Id, double X, double Y);

public sealed record RegressionTableRow(
    int Id,
    double X,
    double Y,
    double XDeviation,
    double YDeviation,
    double DeviationProduct,
    double SquaredXDeviation,
    double? PredictedY);

public sealed record RegressionTableResult(
    double MeanX,
    double MeanY,
    double SumX,
    double SumY,
    double SumXDeviation,
    double SumYDeviation,
    double SumDeviationProduct,
    double SumSquaredXDeviation,
    double? SumPredictedY,
    double? Slope,
    double? Intercept,
    IReadOnlyList<RegressionTableRow> Rows);

public static class RegressionTable
{
    public static RegressionTableResult Calculate(IReadOnlyList<RegressionTableInput> points)
    {
        var meanX = points.Count == 0 ? 0 : points.Average(point => point.X);
        var meanY = points.Count == 0 ? 0 : points.Average(point => point.Y);

        var rows = points.Select(point =>
        {
            var xDeviation = point.X - meanX;
            var yDeviation = point.Y - meanY;

            return new RegressionTableRow(
                point.Id,
                point.X,
                point.Y,
                xDeviation,
                yDeviation,
                xDeviation * yDeviation,
                xDeviation * xDeviation,
                null);
        }).ToArray();

        var sumXY = rows.Sum(row => row.DeviationProduct);
        var sumXX = rows.Sum(row => row.SquaredXDeviation);
        double? slope = null;
        double? intercept = null;

        if (points.Count >= 2 && sumXX != 0 && double.IsFinite(sumXX) && double.IsFinite(sumXY))
        {
            var calculatedSlope = sumXY / sumXX;
            var calculatedIntercept = meanY - calculatedSlope * meanX;
            if (double.IsFinite(calculatedSlope) && double.IsFinite(calculatedIntercept))
            {
                slope = calculatedSlope;
                intercept = calculatedIntercept;
            }
        }

        if (slope is double b && intercept is double a)
        {
            for (var index = 0; index < rows.Length; index++)
            {
                rows[index] = rows[index] with { PredictedY = a + b * rows[index].X };
            }
        }

        return new RegressionTableResult(
            meanX,
            meanY,
            rows.Sum(row => row.X),
            rows.Sum(row => row.Y),
            rows.Sum(row => row.XDeviation),
            rows.Sum(row => row.YDeviation),
            sumXY,
            sumXX,
            slope is null ? null : rows.Sum(row => row.PredictedY!.Value),
            slope,
            intercept,
            rows);
    }
}