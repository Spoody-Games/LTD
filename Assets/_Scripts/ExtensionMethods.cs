using UnityEngine;
using System.Collections.Generic;
static class ExtensionMethods
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    public static T[,] ConvertMatrix<T>(this T[] arr, int m, int n)
    {
        T[,] result = new T[m, n];
        for (int i = 0; i < arr.Length; i++)
        {
            result[i % m, i / m] = arr[i];
        }
        return result;
    }
    public static Slot GetSlotByIndex(this Slot[,] mx, Vector2Int indexVec)
    {
        return mx[indexVec.x, indexVec.y];
    }
    public static Vector2Int FindSlotIndexInMatrix(this Slot[,] mx, Slot ob, int width = 12, int length = 14)
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (mx[j, i] == ob)
                {
                    return new Vector2Int(j, i);
                }
            }
        }
        Debug.LogError("Not Found");
        return new Vector2Int(999, 999);
    }
    public static Transform GetClosestObject(this List<Transform> Objects, Transform self)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = self.position;
        foreach (Transform potentialTarget in Objects)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }
    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

}