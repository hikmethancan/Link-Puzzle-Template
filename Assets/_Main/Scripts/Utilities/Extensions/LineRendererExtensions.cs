using System;
using System.Linq;
using UnityEngine;

namespace _Main.Scripts.Utilities.Extensions
{
	public static class LineRendererExtensions
	{
		/// <summary>
		/// Get the positions of all points in the LineRenderer
		/// </summary>
		/// <returns>The array of the positions</returns>
		public static Vector3[] Positions(this LineRenderer line)
		{
			var pos = new Vector3[line.positionCount];

			line.GetPositions(pos);
			return pos;
		}

		/// <summary>
		/// Adds a new point at the end of the LineRenderer
		/// </summary>
		/// <param name="position">Point's position</param>
		public static void Queue(this LineRenderer line, Vector3 position)
		{
			++line.positionCount;
			line.SetPosition(line.positionCount - 1, position);
		}

		/// <summary>
		/// Deletes first position of the LineRenderer
		/// </summary>
		public static void Dequeue(this LineRenderer line)
		{
			if (line.positionCount.Equals(0)) return;
			var pos = line.Positions();
			--line.positionCount;
			line.SetPositions(pos[1..]);
		}

		/// <summary>
		/// Deletes last position of the LineRenderer
		/// </summary>
		public static void Pop(this LineRenderer line)
		{
			if (line.positionCount.Equals(0)) return;

			line.positionCount--;
		}

		/// <summary>
		/// Deletes all the positions of the LineRenderer
		/// </summary>
		public static void Clear(this LineRenderer line)
		{
			line.positionCount = 0;
		}

		private static readonly float[] _kernel = new float[] { 0.05f, 0.9f, 0.05f };
		private static readonly float[] _strongKernel = new float[] { 0.3f, 0.4f, 0.3f };
		private const int _pad = 1;

		/// <summary>
		/// Smooths the whole LineRenderer's positions
		/// </summary>
		public static void Convolve(this LineRenderer line)
		{
			for (int i = _pad; i < line.positionCount - _pad; i++)
			{
				var r = Vector3.zero;
				for (int j = 0; j < 3; j++)
					r += _strongKernel[j] * line.GetPosition(i - _pad + j);
				line.SetPosition(i, r);
			}
		}

		/// <summary>
		/// Smooths just the last LineRenderer's position
		/// </summary>
		public static void ConvolveLast(this LineRenderer line)
		{
			int count = line.positionCount;
			if (count <= 3) return;
			int i = count - 3;
			var r = Vector3.zero;
			for (int j = 0; j < 3; j++)
				r += _strongKernel[j] * line.GetPosition(i + j);
			line.SetPosition(i, r);
		}

		/// <summary>
		/// Smooths just the last three LineRenderer's positions
		/// </summary>
		public static void ConvolveLast3(this LineRenderer line)
		{
			int count = line.positionCount;
			if (count <= 6) return;
			for (int i = count - 1; i >= count - 3; i--)
			{
				var r = Vector3.zero;
				for (int j = 0; j < 3; j++)
					r += _strongKernel[j] * line.GetPosition(i - j);
				line.SetPosition(i, r);
			}
		}

		public static void ConvolveMerge(this LineRenderer line, LineRenderer toMerge)
		{
			if (line.positionCount.Equals(0)) return;

			var combined = new Vector3[line.positionCount + toMerge.positionCount - 1];
			Array.Copy(line.Positions(), combined, line.positionCount);
			Array.Copy(toMerge.Positions()[1..], 0, combined, line.positionCount, toMerge.positionCount - 1);

			for (int i = _pad; i < combined.Length - _pad; i++)
			{
				Vector3 r = Vector2.zero;
				for (int j = 0; j < 3; j++)
					r += _strongKernel[j] * combined[i - _pad + j];
				combined[i] = r;
			}

			line.SetPositions(combined[..line.positionCount]);
			toMerge.SetPositions(combined[(line.positionCount - 1)..]);
		}

		/// <summary>
		/// Returns the last position of the LineRenderer
		/// </summary>
		/// <returns>Last Position</returns>
		public static Vector2 Peek(this LineRenderer line)
		{
			return line.PeekAt(line.positionCount - 1);
		}

		/// <summary>
		/// Returns the position of the given index
		/// </summary>
		/// <param name="index">Index of position</param>
		/// <returns>Position at index</returns>
		public static Vector2 PeekAt(this LineRenderer line, int index)
		{
			if (line.positionCount.Equals(0)) return Vector2.zero;

			return line.GetPosition(index);
		}

		public static void Subdivide(this LineRenderer line)
		{
			var pos = line.Positions();
			var newPos = new Vector3[pos.Length * 2 - 1];

			for (int i = 0; i < pos.Length - 1; i++)
			{
				newPos[i * 2] = pos[i];
				newPos[i * 2 + 1] = (pos[i] + pos[i + 1]) / 2;
			}

			newPos[^1] = pos[^1];
			newPos[^2] = (pos[^1] + pos[^2]) / 2;

			line.positionCount = pos.Length * 2 - 1;
			line.SetPositions(newPos);
		}

		/// <summary>
		/// Sets the color of start and end of the LineRenderer
		/// </summary>
		/// <param name="color">Color</param>
		public static void SetColor(this LineRenderer line, Color color)
		{
			line.startColor = color;
			line.endColor = color;
		}

		/// <summary>
		/// Adds a collider to the LineRenderer
		/// <br/><i><b>NOTE:</b> Only works in 2D</i> 
		/// </summary>
		/// <param name="physicsMaterial">Physics material of the collider</param>
		public static void AddCollider(this LineRenderer line, PhysicsMaterial2D physicsMaterial = null)
		{
			if (!line.gameObject.TryGetComponent(out EdgeCollider2D col))
				col = line.gameObject.AddComponent<EdgeCollider2D>();

			var points = line.Positions().ToVector2().ToList();

			col.edgeRadius = line.widthMultiplier / 2f;
			col.sharedMaterial = physicsMaterial;
			col.SetPoints(points);
		}

		/// <summary>
		/// Gets the total length of the LineRenderer
		/// </summary>
		/// <returns>Length</returns>
		public static float GetLength(this LineRenderer line)
		{
			float length = 0;
			var points = line.Positions();
			int count = points.Length;
			for (int i = 1; i < count; i++)
				length += Vector3.Distance(points[i], points[i - 1]);

			return length;
		}
	}
}