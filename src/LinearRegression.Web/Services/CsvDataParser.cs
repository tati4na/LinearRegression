using System.Globalization;

namespace LinearRegression.Web.Services;

public static class CsvDataParser
{
	public static bool TryParse(
		string content,
		out List<(double X, double Y)> coordinates,
		out string? error)
	{
		coordinates = [];
		error = null;

		var lines = content.TrimStart('\uFEFF').Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
		var firstLine = lines.FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));
		if (firstLine is null)
		{
			error = "Enter or upload data containing X and Y coordinate pairs.";
			return false;
		}

		var delimiter = DetectDelimiter(firstLine);
		var firstRecord = true;
		var lineNumber = 0;
		foreach (var line in lines)
		{
			lineNumber++;
			if (string.IsNullOrWhiteSpace(line))
			{
				continue;
			}

			var fields = line.Split(delimiter);
			if (firstRecord && IsHeader(fields))
			{
				firstRecord = false;
				continue;
			}

			firstRecord = false;
			if (fields.Length < 2 ||
				!TryParseCoordinate(fields[0], out var x) ||
				!TryParseCoordinate(fields[1], out var y))
			{
				error = $"Row {lineNumber} must contain two valid numeric coordinates separated by a comma or tab.";
				coordinates.Clear();
				return false;
			}

			coordinates.Add((x, y));
		}

		if (coordinates.Count == 0)
		{
			error = "No coordinate pairs were found in the data.";
			return false;
		}

		return true;
	}

	private static char DetectDelimiter(string line)
	{
		var commas = line.Count(character => character == ',');
		var tabs = line.Count(character => character == '\t');
		return tabs > commas ? '\t' : ',';
	}

	private static bool IsHeader(string[] fields) =>
		fields.Length >= 2 &&
		((NormalizeField(fields[0]).Equals("x", StringComparison.OrdinalIgnoreCase) &&
			NormalizeField(fields[1]).Equals("y", StringComparison.OrdinalIgnoreCase)) ||
			(!TryParseCoordinate(fields[0], out _) && !TryParseCoordinate(fields[1], out _)));

	private static bool TryParseCoordinate(string field, out double value) =>
		double.TryParse(
			NormalizeField(field),
			NumberStyles.Float,
			CultureInfo.InvariantCulture,
			out value) &&
		double.IsFinite(value);

	private static string NormalizeField(string field)
	{
		var value = field.Trim();
		if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
		{
			value = value[1..^1].Replace("\"\"", "\"", StringComparison.Ordinal);
		}

		return value.Trim();
	}
}
