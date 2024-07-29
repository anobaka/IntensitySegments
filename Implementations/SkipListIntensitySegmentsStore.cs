using System.Collections.Generic;
using IntensitySegments.Abstractions;
using IntensitySegments.Utils;
using static System.Formats.Asn1.AsnWriter;

namespace IntensitySegments.Implementations;

public class SkipListIntensitySegmentsStore : ISegmentsStore
{
    private readonly SkipList _skipList = new();
    private readonly Dictionary<int, int> _amounts = new();

    public void Add(int from, int to, int amount) => _addOrSet(from, to, amount, true);

    public void Set(int from, int to, int amount) => _addOrSet(from, to, amount, false);

    /// <summary>
    /// 有必要时可以缓存
    /// </summary>
    public Segment[] Segments =>
        _skipList.GetAllIndexes().Select(index => new Segment(index, _amounts[index])).ToArray();

    /// <summary>
    /// 优化版本，中段无需遍历，仅需要检查两端（O(LogN)，N=线段端点数）
    /// </summary>
    private void _mergeEdges(int from, int to)
    {
        var beforeFrom = _skipList.GetBefore(from);
        var afterTo = _skipList.GetNext(to);

        // 和前面的值相等，或是第一个值并且amount为0，则移除from
        if (beforeFrom.HasValue && _amounts[beforeFrom.Value] == _amounts[from] ||
            !beforeFrom.HasValue && _amounts[from] == 0)
        {
            _skipList.Delete(from);
            _amounts.Remove(from);
        }

        // 和后面的值相等，则删除to
        if (afterTo.HasValue && _amounts[to] == _amounts[afterTo.Value])
        {
            _skipList.Delete(to);
            _amounts.Remove(to);
        }
        else
        {
            // 或者to是最后一个值并且和前一个值均为0，则删除to
            if (!afterTo.HasValue && _amounts[to] == 0)
            {
                var beforeTo = _skipList.GetBefore(to);
                if (beforeTo.HasValue && _amounts[beforeTo.Value] == 0)
                {
                    _skipList.Delete(to);
                    _amounts.Remove(to);
                }
            }
        }
    }

    private void _addOrSet(int from, int to, int amount, bool isAdd)
    {
        // 插入
        _skipList.Insert(from);
        _skipList.Insert(to);

        // 设置本次修改前的Amount值
        if (!_amounts.ContainsKey(from))
        {
            var before = _skipList.GetBefore(from);
            _amounts[from] = before.HasValue ? _amounts[before.Value] : 0;
        }

        if (!_amounts.ContainsKey(to))
        {
            var before = _skipList.GetBefore(to);
            _amounts[to] = before.HasValue ? _amounts[before.Value] : 0;
        }

        // 更新区间Amount
        // todo: 根据实际数据分布情况，该步骤时间复杂度可能会劣化成O(N)。N<=线段数量x2。
        // todo: 可以参考线段树的方案，为非底层跳表预留一个Lazy字段，用于在ToString时再计算Amount，Add和Set时不计算，但需要向上调整链路的Lazy值，修改后此步耗时为O(logN)。
        var indexesBetween = _skipList.GetBetween(from, to);
        for (var i = 0; i < indexesBetween.Length - 1; i++)
        {
            var index = indexesBetween[i];
            if (isAdd)
            {
                _amounts[index] += amount;
            }
            else
            {
                _amounts[index] = amount;
                // Set时中间节点均可删除
                if (i > 0)
                {
                    _skipList.Delete(index);
                    _amounts.Remove(index);
                }
            }
        }

        // 和左右相同Amount的节点合并
        _mergeEdges(from, to);
    }
}