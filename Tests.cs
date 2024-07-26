using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntensitySegments;

[TestClass]
public class Tests
{
    [TestMethod]
    public void Test1()
    {
        var segments = new IntensitySegments();

        segments.ToString().Should().BeEquivalentTo("[]");
        segments.Add(10, 30, 1);
        segments.ToString().Should().BeEquivalentTo("[[10,1],[30,0]]");
        segments.Add(20, 40, 1);
        segments.ToString().Should().BeEquivalentTo("[[10,1],[20,2],[30,1],[40,0]]");
        segments.Add(10, 40, -2);
        segments.ToString().Should().BeEquivalentTo("[[10,-1],[20,0],[30,-1],[40,0]]");
    }

    [TestMethod]
    public void Test2()
    {
        var segments = new IntensitySegments();

        segments.ToString().Should().BeEquivalentTo("[]");
        segments.Add(10, 30, 1);
        segments.ToString().Should().BeEquivalentTo("[[10,1],[30,0]]");
        segments.Add(20, 40, 1);
        segments.ToString().Should().BeEquivalentTo("[[10,1],[20,2],[30,1],[40,0]]");
        segments.Add(10, 40, -1);
        segments.ToString().Should().BeEquivalentTo("[[20,1],[30,0]]");
        segments.Add(10, 40, -1);
        segments.ToString().Should().BeEquivalentTo("[[10,-1],[20,0],[30,-1],[40,0]]");
    }

    [TestMethod]
    public void TestDynamically()
    {
        const int interval = 100000;
        var contrast = new int[interval];

        var segments = new IntensitySegments();
        var rand = new Random();

        for (var i = 0; i < 10000; i++)
        {
            var opt = rand.Next(3);
            switch (opt)
            {
                case 0:
                    // Console.WriteLine($"{segments} - {DisplayArrayAsIntensitySegments(contrast)}");
                    segments.ToString().Should().Be(DisplayArrayAsIntensitySegments(contrast));
                    break;
                case 1:
                case 2:
                {
                    var from = rand.Next(0, interval);
                    var to = rand.Next(from, interval);
                    var amount = rand.Next(0, interval);
                    switch (opt)
                    {
                        case 1:
                            // Console.WriteLine($"Add {amount} from {from} to {to}");
                            segments.Add(from, to, amount);
                            for (var j = from; j < to; j++)
                            {
                                contrast[j] += amount;
                            }

                            break;
                        case 2:
                            // Console.WriteLine($"Set {amount} from {from} to {to}");
                            segments.Set(from, to, amount);
                            for (var j = from; j < to; j++)
                            {
                                contrast[j] = amount;
                            }

                            break;
                    }

                    break;
                }
            }
        }
    }

    private static string DisplayArrayAsIntensitySegments(int[] arr)
    {
        var sb = new StringBuilder();
        sb.Append('[');

        if (arr.Length > 0)
        {
            var prevIdx = 0;
            var prevValue = arr[prevIdx];
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] == prevValue)
                {
                    continue;
                }

                if (prevIdx != 0 || prevValue != 0)
                {
                    sb.Append($"[{prevIdx},{prevValue}],");
                }

                prevValue = arr[i];
                prevIdx = i;
            }

            if (prevIdx != 0 || prevValue != 0)
            {
                sb.Append($"[{prevIdx},{prevValue}]");
            }
        }

        var str = sb.ToString();
        return $"{str.Trim(',')}]";
    }
}