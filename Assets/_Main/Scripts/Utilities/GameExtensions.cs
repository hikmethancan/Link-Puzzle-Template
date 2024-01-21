using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.Utilities
{
    public static class GameExtensions
    {
        public static T GetRandomElement<T>(this T[] array)
        {
            if (array == null || array.Length < 1) return default;
            return array[Random.Range(0, array.Length)];
        }

        public static T GetRandomElement<T>(this List<T> list)
        {
            if (list == null || list.Count < 1) return default;
            return list[Random.Range(0, list.Count)];
        }

        public static Vector3 SetX(this Vector3 vector, float xValue)
        {
            return new Vector3(xValue, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float yValue)
        {
            return new Vector3(vector.x, yValue, vector.z);
        }

        public static Vector3 SetZ(this Vector3 vector, float zValue)
        {
            return new Vector3(vector.x, vector.y, zValue);
        }

        public static Vector3 SetXY(this Vector3 vector, float xValue, float yValue)
        {
            return new Vector3(xValue, yValue, vector.z);
        }

        public static Vector3 SetXZ(this Vector3 vector, float xValue, float zValue)
        {
            return new Vector3(xValue, vector.y, zValue);
        }

        public static Vector3 SetYZ(this Vector3 vector, float yValue, float zValue)
        {
            return new Vector3(vector.x, yValue, zValue);
        }
    }
}