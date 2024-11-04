namespace Osmium.Language.Types;

public class Range
{
    public int? StartIndex { get; set; }
    public int? EndIndex { get; set; }

    public Range(int? startIndex, int? endIndex)
    {
        if (startIndex.HasValue && endIndex.HasValue && startIndex > endIndex)
            throw new ArgumentException("Start index cannot be greater than end index!");

        if (startIndex is null && endIndex is null)
            throw new ArgumentException("Start index and End index cannot both be null!");


        StartIndex = startIndex;
        EndIndex = endIndex;
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
