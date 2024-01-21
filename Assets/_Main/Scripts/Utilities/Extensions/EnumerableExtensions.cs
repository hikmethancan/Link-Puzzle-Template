using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Utilities.Extensions
{
	public static class EnumerableExtensions
	{
		#region Array

		/// <summary>
		/// Adds an item to the end of a new array
		/// </summary>
		/// <param name="item">Item to be added</param>
		/// <returns>Creates a new array of your array and added item</returns>
		public static T[] Add<T>(this T[] array, T item)
		{
			Array.Resize(ref array, array.Length + 1);
			array[^1] = item;
			return array;
		}

		/// <summary>
		/// Adds an array to the end of a new array
		/// </summary>
		/// <param name="collection">Array to be added</param>
		/// <returns>Creates a new array of your array and added array</returns>
		public static T[] AddRange<T>(this T[] array, T[] collection)
		{
			int startingSize = array.Length;
			int newSize = startingSize + collection.Length;
			Array.Resize(ref array, newSize);
			Array.Copy(collection, 0, array, startingSize, collection.Length);
			return array;
		}

		/// <summary>
		/// Picks a random item from the array.
		/// </summary>
		/// <returns>A random item</returns>
		public static T RandomItem<T>(this T[] array)
		{
			if (array.Length.Equals(0))
				return default;

			var rnd = new System.Random(Random.Range(0, int.MaxValue));
			var index = rnd.Next(0, array.Length);
			return array[index];
		}

		/// <summary>
		/// Picks a random item from the array according to probability table
		/// </summary>
		/// <param name="array">Your array</param>
		/// <param name="weights">Weights of the array items corresponding to items index</param>
		/// <returns>A random item</returns>
		public static T WeightedRandom<T>(this T[] array, int[] weights)
		{
			int totalPriority = 0;
			int itemCount = array.Length;
			var weightsOriginal = new List<int>(weights);

			for (int i = 0; i < itemCount; i++)
			{
				weightsOriginal[i] += totalPriority;
				totalPriority += weights[i];
			}

			int randomPriority = Random.Range(0, totalPriority);

			for (int i = 0; i < itemCount; i++)
			{
				if (weightsOriginal[i] > randomPriority)
					return array[i];
			}

			return array[0];
		}

		/// <summary>
		/// Shuffles the array
		/// </summary>
		public static void Shuffle<T>(this T[] array)
		{
			var rng = new System.Random(Random.Range(0, int.MaxValue));
			var n = array.Length;
			while (n > 1)
			{
				n--;
				var k = rng.Next(n + 1);
				(array[k], array[n]) = (array[n], array[k]);
			}
		}

		#endregion

		#region List

		/// <summary>
		/// Picks a random item from the list.
		/// </summary>
		/// <returns>A random item</returns>
		public static T RandomItem<T>(this IList<T> list)
		{
			if (list.Count.Equals(0))
				return default;

			var rnd = new System.Random(Random.Range(0, int.MaxValue));
			var index = rnd.Next(0, list.Count);
			return list[index];
		}

		/// <summary>
		/// Picks a random item and removes it from the list.
		/// </summary>
		/// <returns>A random item</returns>
		public static T PickRandomItem<T>(this List<T> list)
		{
			if (list.Count.Equals(0))
				return default;

			int pickIndex = Random.Range(0, list.Count);
			var picked = list[pickIndex];
			list.Remove(picked);
			return picked;
		}

		/// <summary>
		/// Picks a random item from the list according to probability table
		/// </summary>
		/// <param name="list">Your list</param>
		/// <param name="weights">Weights of the list items corresponding to items index</param>
		/// <returns>A random item</returns>
		public static T WeightedRandom<T>(this List<T> list, List<int> weights)
		{
			int itemCount = list.Count;
			var weightsOriginal = new List<int>(weights);
			int totalPriority = WeightedRandom(ref weightsOriginal, weights, itemCount);

			int randomPriority = Random.Range(0, totalPriority);

			for (int i = 0; i < itemCount; i++)
			{
				if (weightsOriginal[i] > randomPriority)
					return list[i];
			}

			return list[0];
		}

		/// <summary>
		/// Picks a random item from the list according to probability table and removes it from the list
		/// </summary>
		/// <param name="list">Your list</param>
		/// <param name="weights">Weights of the list items corresponding to items index</param>
		/// <returns>A random item</returns>
		public static T PickWeightedRandom<T>(this List<T> list, ref List<int> weights)
		{
			int itemCount = list.Count;
			var weightsOriginal = new List<int>(weights);
			var totalPriority = WeightedRandom(ref weightsOriginal, weights, itemCount);

			int randomPriority = Random.Range(0, totalPriority);

			for (int i = 0; i < itemCount; i++)
			{
				if (weightsOriginal[i] > randomPriority)
				{
					var item = list[i];
					list.RemoveAt(i);
					weights.RemoveAt(i);
					return item;
				}
			}

			var item0 = list[0];
			list.RemoveAt(0);
			weights.RemoveAt(0);
			return item0;
		}

		private static int WeightedRandom(ref List<int> weightsOriginal, IReadOnlyList<int> weights, int itemCount)
		{
			int totalPriority = 0;
			for (int i = 0; i < itemCount; i++)
			{
				weightsOriginal[i] += totalPriority;
				totalPriority += weights[i];
			}

			return totalPriority;
		}

		/// <summary>
		/// Shuffles the list
		/// </summary>
		public static void Shuffle<T>(this IList<T> list)
		{
			var rng = new System.Random(Random.Range(0, int.MaxValue));
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = rng.Next(n + 1);
				(list[k], list[n]) = (list[n], list[k]);
			}
		}

		#endregion
	}
}