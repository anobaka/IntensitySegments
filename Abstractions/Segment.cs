namespace IntensitySegments.Abstractions;

public record Segment(int From, int Amount)
{
    public override string ToString()
    {
        return $"[{From},{Amount}]";
    }
}