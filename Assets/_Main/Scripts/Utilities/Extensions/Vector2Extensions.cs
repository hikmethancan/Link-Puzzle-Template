using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.Utilities.Extensions
{
	public static class Vector2Extensions
	{
		/// <summary>
		/// Sets X value of this vector
		/// </summary>
		/// <param name="x">X value</param>
		public static void SetX(ref this Vector2 vector, float x)
		{
			vector.Set(x, vector.y);
		}

		/// <summary>
		/// Sets Y value of this vector
		/// </summary>
		/// <param name="y">Y value</param>
		public static void SetY(ref this Vector2 vector, float y)
		{
			vector.Set(vector.x, y);
		}

		/// <summary>
		/// Converts a Vector2 array to Vector3 array
		/// </summary>
		/// <returns>Vector3 array</returns>
		public static Vector3[] ToVector3(this Vector2[] vector)
		{
			var v3 = new Vector3[vector.Length];
			for (int i = 0; i < vector.Length; i++)
				v3[i] = new Vector3(vector[i].x, vector[i].y);
			return v3;
		}

		/// <summary>
		/// Converts a Vector2 list to Vector3 list
		/// </summary>
		/// <returns>Vector3 list</returns>
		public static List<Vector3> ToVector3(this List<Vector2> vector)
		{
			var v3 = new List<Vector3>(vector.Count);
			for (int i = 0; i < vector.Count; i++)
				v3[i] = new Vector3(vector[i].x, vector[i].y);
			return v3;
		}
	}
}