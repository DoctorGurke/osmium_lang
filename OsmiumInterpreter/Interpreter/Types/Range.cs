namespace Osmium.Interpreter.Types;

public class Range
{
    public int? StartIndex { get; set; }
    public int? EndIndex { get; set; }

    public Range(int? startIndex, int? endIndex)
    {
        if (startIndex.HasValue && endIndex.HasValue && startIndex > endIndex)
        {
            throw new ArgumentException("Start index cannot be greater than end index.");
        }

        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    public static Range FromStartIndex(int startIndex)
    {
        return new Range(startIndex, null);
    }

    public static Range ToEndIndex(int endIndex)
    {
        return new Range(null, endIndex);
    }

    public static Range Between(int startIndex, int endIndex)
    {
        return new Range(startIndex, endIndex);
    }

    public static Range Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input string cannot be null or empty.");
        }

        string[] parts = input.Split("..");

        if (parts.Length == 1)
        {
            if (int.TryParse(parts[0], out int endIndex))
            {
                return ToEndIndex(endIndex);
            }
            else
            {
                throw new FormatException($"Invalid input string: {input}");
            }
        }
        else if (parts.Length == 2)
        {
            int? startIndex = null;
            int? endIndex = null;

            if (!string.IsNullOrEmpty(parts[0]))
            {
                if (!int.TryParse(parts[0], out int start))
                {
                    throw new FormatException($"Invalid input string: {input}");
                }
                startIndex = start;
            }

            if (!string.IsNullOrEmpty(parts[1]))
            {
                if (!int.TryParse(parts[1], out int end))
                {
                    throw new FormatException($"Invalid input string: {input}");
                }
                endIndex = end;
            }

            return new Range(startIndex, endIndex);
        }
        else
        {
            throw new FormatException($"Invalid input string: {input}");
        }
    }

    public override string ToString()
    {
        if (StartIndex.HasValue && EndIndex.HasValue)
        {
            return $"{StartIndex}..{EndIndex}";
        }
        else if (StartIndex.HasValue)
        {
            return $"{StartIndex}..";
        }
        else if (EndIndex.HasValue)
        {
            return $"..{EndIndex}";
        }

        return base.ToString();
    }
}
