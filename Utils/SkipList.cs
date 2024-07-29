using System.Reflection.PortableExecutable;

namespace IntensitySegments.Utils;

public class SkipList
{
    /// <summary>
    /// 作为内部类，减少外界干扰，不对外暴露
    /// </summary>
    private record SkipListNode
    {
        public SkipListNode(int level, int index)
        {
            Index = index;
            Forward = new SkipListNode[level + 1];
        }

        /// <summary>
        /// 跳表索引值，所有查找、插入、删除均以该值为基准。
        /// </summary>
        public int Index { get; }

        public SkipListNode?[] Forward { get; }
    }

    private const double SkipPossibility = 0.5;
    private readonly SkipListNode _header;

    /// <summary>
    /// 跳表层数
    /// </summary>
    private readonly int _maxLevel;

    private readonly Random _random = new Random();

    public SkipList()
    {
        _maxLevel = 31;
        // 哨兵节点
        _header = new SkipListNode(_maxLevel, int.MinValue);
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
    /// <param name="index"></param>
    /// <returns></returns>
    public void Insert(int index)
    {
        var update = new SkipListNode[_maxLevel + 1];
        var current = _header;

        for (var i = _maxLevel - 1; i >= 0; i--)
        {
            while (current!.Forward[i]?.Index < index)
            {
                current = current.Forward[i];
            }

            update[i] = current;
        }

        var next = current.Forward[0];

        if (next?.Index != index)
        {
            var level = _getRandomLevel();
            var newNode = new SkipListNode(level, index);

            for (var i = 0; i <= level; i++)
            {
                newNode.Forward[i] = update[i].Forward[i];
                update[i].Forward[i] = newNode;
            }

        }
    }

    private SkipListNode _findLastLessThanOrEquals(int index)
    {
        var current = _header;

        for (var i = _maxLevel - 1; i >= 0; i--)
        {
            while (current!.Forward[i]?.Index <= index)
            {
                current = current.Forward[i];
            }
        }

        return current;
    }

    public int? GetBefore(int index)
    {
        var current = _header;

        for (var i = _maxLevel - 1; i >= 0; i--)
        {
            while (current!.Forward[i]?.Index < index)
            {
                current = current.Forward[i];
            }
        }

        return current == _header ? null : current.Index;
    }

    public int? GetNext(int index)
    {
        var current = _header;

        for (var i = _maxLevel - 1; i >= 0; i--)
        {
            while (current!.Forward[i]?.Index <= index)
            {
                current = current.Forward[i];
            }
        }

        return current.Forward[0]?.Index;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end">Included</param>
    /// <returns></returns>
    public int[] GetBetween(int start, int end)
    {
        var startNode = _findLastLessThanOrEquals(start);
        var endNode = _findLastLessThanOrEquals(end);

        var indexes = new List<int>();
        while (startNode != endNode)
        {
            if (startNode != _header)
            {
                indexes.Add(startNode.Index);
            }

            startNode = startNode.Forward[0]!;
        }

        indexes.Add(end);
        return indexes.ToArray();
    }

    /// <summary>
    /// 跳表删除
    /// </summary>
    /// <param name="index"></param>
    public void Delete(int index)
    {
        var update = new SkipListNode[_maxLevel + 1];
        var current = _header;

        for (var i = _maxLevel - 1; i >= 0; i--)
        {
            while (current?.Forward[i]?.Index < index)
            {
                current = current.Forward[i];
            }

            update[i] = current!;
        }

        current = current?.Forward[0];

        if (current?.Index == index)
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

    public int[] GetAllIndexes()
    {
        var indexes = new List<int>();
        var current = _header.Forward[0];
        while (current != null)
        {
            indexes.Add(current.Index);
            current = current.Forward[0];
        }

        return indexes.ToArray();
    }
}