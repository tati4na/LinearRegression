using LinearRegression.Web.Models;

namespace LinearRegression.Web.Services;

public interface IRegressionEngine
{
    RegressionResult Compute(IReadOnlyList<DataPoint> points);
}
