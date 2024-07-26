using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntensitySegments;

public class Node(int level, int startPos)
{
    public int Amount { get; set; }
    public int StartPos { get; set; } = startPos;
    public Node?[] Forward { get; set; } = new Node[level + 1];
}

/// <summary>
/// 结合跳表实现
/// </summary>
public class IntensitySegments
{
    private const double SkipPossibility = 0.5;

    private readonly Node _header;

    /// <summary>
    /// 跳表层数
    /// </summary>
    private readonly int _maxLevel;

    private readonly Random _random = new Random();

    public IntensitySegments()
    {
        _maxLevel = 31;
        // 哨兵节点
        _header = new Node(_maxLevel, int.MinValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to">不包含该位置</param>
    /// <param name="amount"></param>
    public void Add(int from, int to, int amount) => _addOrSet(from, to, amount, true);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to">不包含该位置</param>
    /// <param name="amount"></param>
    public void Set(int from, int to, int amount) => _addOrSet(from, to, amount, false);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        var data = _header.Forward[0];

        while (data?.Amount == 0)
        {
            var temp = data;
            data = data.Forward[0];
            _delete(temp.StartPos);
        }

        if (data != null)
        {
            _merge(data, null);

            while (data != null)
            {
                sb.Append($"[{data.StartPos},{(data.Forward[0] == null ? 0 : data.Amount)}]");
                data = data.Forward[0];
                if (data != null)
                {
                    sb.Append(',');
                }
            }
        }

        sb.Append(']');
        var str = sb.ToString();
        return str;
    }

    /// <summary>
    /// 随机选择跳表索引层数
    /// </summary>
    /// <returns></returns>
    private int _getRandomLevel()
    {
        var level = 0;
        while (_random.NextDouble() < SkipPossibility && level < _maxLevel)
        {
            level++;
        }

        return level;
    }

    /// <summary>
    /// 跳表插入
    /// </summary>
    /// <param name="startPos"></param>
    /// <returns></returns>
    private Node _insert(int startPos)
    {
        var update = new Node[_maxLevel + 1];
        var current = _header;

        for (var i = _maxLevel - 1; i >= 0; i--)
        {
            while (current!.Forward[i]?.StartPos < startPos)
            {
                current = current.Forward[i];
            }

            update[i] = current;
        }

        var next = current.Forward[0];

        if (next?.StartPos != startPos)
        {
            var level = _getRandomLevel();
            var newNode = new Node(level, startPos) {Amount = current.Amount};

            for (var i = 0; i <= level; i++)
            {
                newNode.Forward[i] = update[i].Forward[i];
                update[i].Forward[i] = newNode;
            }

            return newNode;
        }

        return next;
    }

    /// <summary>
    /// 跳表删除
    /// </summary>
    /// <param name="startPos"></param>
    private void _delete(int startPos)
    {
        var update = new Node[_maxLevel + 1];
        var current = _header;

        for (var i = _maxLevel - 1; i >= 0; i--)
        {
            while (current?.Forward[i]?.StartPos < startPos)
            {
                current = current.Forward[i];
            }

            update[i] = current!;
        }

        current = current?.Forward[0];

        if (current?.StartPos == startPos)
        {
            for (var i = 0; i < current.Forward.Length; i++)
            {
                if (update[i].Forward[i] != current)
                {
                    break;
                }

                update[i].Forward[i] = current.Forward[i];
            }
        }
    }

    /// <summary>
    /// 合并相同Amount的连续节点
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private void _merge(Node from, Node? to)
    {
        var current = from;
        while (true)
        {
            var next = current.Forward[0];
            while (next != null && next.Amount == current.Amount)
            {
                _delete(next.StartPos);
                if (next == to)
                {
                    break;
                }

                next = next.Forward[0];
            }

            if (next == null || next == to)
            {
                break;
            }

            current = next;
        }
    }

    private void _addOrSet(int from, int to, int amount, bool isAdd)
    {
        // 插入 & 定位基链节点
        var left = _insert(from);
        var right = _insert(to);

        var tmpLeft = left;

        // 更新区间Amount
        while (tmpLeft != right)
        {
            if (isAdd)
            {
                tmpLeft!.Amount += amount;
            }
            else
            {
                tmpLeft!.Amount = amount;
            }

            tmpLeft = tmpLeft.Forward[0];
        }

        // 合并相同Amount的连续节点
        _merge(left, right);
    }
}