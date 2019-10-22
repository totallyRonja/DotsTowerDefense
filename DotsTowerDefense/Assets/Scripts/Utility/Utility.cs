using System.Collections.Generic;
using Unity.Mathematics;

public static class Utility
{
    //counts from first to last number, not including the last
    public static IEnumerable<int> CountArray(int from, int to)
    {
        int diff = to - from;
        int length = math.abs(diff);
        int dir = sign(diff);
        for (int i = 0, num = from; i < length; i++, num += dir)
        {
            yield return num;
        }
    }

    public static int sign(int num)
    {
        return (num >> 31) * 2 + 1;
    }
}