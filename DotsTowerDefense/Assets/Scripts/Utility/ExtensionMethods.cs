using Unity.Mathematics;
using Random = UnityEngine.Random;

public static class ExtensionMethods
{
    public static void Randomize<T>(this T[] arr)
    {
        for(var i=0; i<arr.Length; i++)
        {
            arr.Swap(i, Random.Range(0, arr.Length));
        }
    }

    public static void Swap<T>(this T[] arr, int index1, int index2)
    {
        T temp = arr[index1];
        arr[index1] = arr[index2];
        arr[index2] = temp;
    }
}