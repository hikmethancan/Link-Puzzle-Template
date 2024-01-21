using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.Utilities.Extensions
{
	public static class TransformExtensions
	{
		/// <summary>
		/// It tells you whether this transform is inside a camera.
		/// </summary>
		/// <param name="camera">Camera</param>
		/// <returns>Is inside or not</returns>
		public static bool IsInsideCamera(this Transform t, Camera camera = null)
		{
			return Functions.IsPositionInsideCamera(t.position, camera);
		}

		/// <summary>
		/// Destroy all children of this parent
		/// </summary>
		public static void DestroyChildren(this Transform parent)
		{
			foreach (Transform t in parent)
				UnityEngine.Object.Destroy(t.gameObject);
		}

		/// <summary>
		/// DestroyImmediate all children of this parent
		/// </summary>
		public static void DestroyImmediateChildren(this Transform parent)
		{
			foreach (Transform t in parent)
				UnityEngine.Object.DestroyImmediate(t.gameObject);
		}

		/// <summary>
		/// Disable all children of this parent
		/// </summary>
		public static void DisableChildren(this Transform parent)
		{
			foreach (Transform t in parent)
				t.gameObject.SetActive(false);
		}

		/// <summary>
		/// Set parent's and all children's layer to given layer
		/// <br/><i>Note that this code is intensive because it's recursive</i>
		/// </summary>
		/// <param name="layer">The layer you want to set</param>
		public static void SetChildrenLayer(this Transform parent, int layer)
		{
			parent.gameObject.layer = layer;
			foreach (Transform t in parent)
				SetChildrenLayer(t, layer);
		}

		public static Transform FindDeepChild(this Transform parent, string childName, GraphSearchType type = GraphSearchType.BreadthFirstSearch)
		{
			if (type == GraphSearchType.BreadthFirstSearch)
			{
				var queue = new Queue<Transform>();
				queue.Enqueue(parent);
				while (queue.Count > 0)
				{
					var c = queue.Dequeue();
					if (c.name.Equals(childName))
						return c;
					foreach (Transform t in c)
						queue.Enqueue(t);
				}

				return null;
			}
			else if (type == GraphSearchType.DepthFirstSearch)
			{
				foreach (Transform child in parent)
				{
					if (child.name.Equals(childName))
						return child;
					var result = child.FindDeepChild(childName, GraphSearchType.DepthFirstSearch);
					if (result)
						return result;
				}

				return null;
			}

			return null;
		}
	}

	public enum GraphSearchType
	{
		BreadthFirstSearch,
		DepthFirstSearch
	}
}