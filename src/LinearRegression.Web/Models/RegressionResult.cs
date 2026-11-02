namespace LinearRegression.Web.Models;

public sealed record PointRow(int Id, double Predicted, double Residual);

public sealed record RegressionResult(
    double MeanX,
    double MeanY,
    double SumXY,
    double SumXX,
    double B,
    double A,
    bool IsValid,
    List<PointRow> Rows);
