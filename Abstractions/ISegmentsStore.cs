namespace IntensitySegments.Abstractions;

public interface ISegmentsStore
{
    void Add(int from, int to, int amount);
    void Set(int from, int to, int amount);
    Segment[] Segments { get; }
}