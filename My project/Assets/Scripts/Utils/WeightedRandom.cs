using UnityEngine;

public static class WeightedRandom
{
    public static T GetWeighted<T>(this T[] t) where T: SelectWeighted
    {
        float total = 0;
        for(int i = 0; i < t.Length; i ++)
        {
            total += t[i].GetSelectionRate();
        }

        float randomValue = Random.Range(0, total);

        total = 0;
        for(int i = 0; i < t.Length; i++)
        {
            if (randomValue < t[i].GetSelectionRate()) return t[i];
            total += t[i].GetSelectionRate();
        }

        return t[t.Length - 1];
    }
}