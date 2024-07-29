using System.Text;
using IntensitySegments.Abstractions;

namespace IntensitySegments;

public class IntensitySegments(ISegmentsStore store)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to">不包含该位置</param>
    /// <param name="amount"></param>
    public void Add(int from, int to, int amount) => store.Add(from, to, amount);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to">不包含该位置</param>
    /// <param name="amount"></param>
    public void Set(int from, int to, int amount) => store.Set(from, to, amount);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');

        var segments = store.Segments;
        sb.Append(string.Join(',', segments.Select(s => s.ToString())));

        sb.Append(']');
        var str = sb.ToString();
        return str;
    }
}