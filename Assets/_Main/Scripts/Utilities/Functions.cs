using System.Globalization;
using UnityEngine;

namespace _Main.Scripts.Utilities
{
	public static class Functions
	{
		private static Camera mainCamera;
		public static Camera MainCamera => mainCamera;

		static Functions()
		{
			mainCamera = Camera.main;
		}

		/// <summary>
		/// Remaps a number from one range to another.
		/// </summary>
		/// <param name="value">The incoming value to be converted</param>
		/// <param name="valueMin">Lower bound of the value's current range</param>
		/// <param name="valueMax">upper bound of the value's current range</param>
		/// <param name="min">lower bound of the value's target range</param>
		/// <param name="max">upper bound of the value's target range</param>
		/// <returns>Equivalent of value between min and max</returns>
		public static float Map(float value, float valueMin, float valueMax, float min, float max)
		{
			return min + (value - valueMin) * (max - min) / (valueMax - valueMin);
		}

		/// <summary>
		/// 3 point lerp
		/// </summary>
		/// <param name="a">1st point</param>
		/// <param name="b">2nd point</param>
		/// <param name="c">3rd point</param>
		/// <param name="t">Value used to interpolate between points</param>
		public static Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
		{
			var ab = Vector3.Lerp(a, b, t);
			var bc = Vector3.Lerp(b, c, t);

			return Vector3.Lerp(ab, bc, t);
		}

		/// <summary>
		/// 4 point lerp
		/// </summary>
		/// <param name="a">1st point</param>
		/// <param name="b">2nd point</param>
		/// <param name="c">3rd point</param>
		/// <param name="d">4th point</param>
		/// <param name="t">Value used to interpolate between points</param>
		public static Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
		{
			var ab_bc = QuadraticLerp(a, b, c, t);
			var bc_cd = QuadraticLerp(b, c, d, t);

			return Vector3.Lerp(ab_bc, bc_cd, t);
		}

		/// <summary>
		/// It tells you whether a position is inside a camera frustum.
		/// </summary>
		/// <param name="position">Given position point</param>
		/// <param name="camera">Camera</param>
		/// <returns>Is inside or not</returns>
		public static bool IsPositionInsideCamera(Vector3 position, Camera camera = null)
		{
			camera = camera ? camera : MainCamera;
			var viewport = camera.WorldToViewportPoint(position);
			bool inCameraFrustum = viewport.x.Is01() && viewport.y.Is01();
			bool inFrontOfCamera = viewport.z > 0;

			return inCameraFrustum && inFrontOfCamera;
		}

		/// <summary>
		/// Is a float value between 0 and 1
		/// </summary>
		/// <param name="value">Float value</param>
		/// <returns>Is between 0 - 1 or not</returns>
		public static bool Is01(this float value)
		{
			return value is > 0f and < 1f;
		}

		/// <summary>
		/// Formats a big number to more readable manner
		/// <br/> 1000000 to 1M etc.
		/// </summary>
		/// <param name="number">Big number</param>
		/// <returns>String value of formatted number</returns>
		public static string FormatBigNumber(long number)
		{
			return number switch
			{
				> 999999999999999999 => number.ToString("0,,,,,,.##Q", CultureInfo.InvariantCulture),
				> 999999999999999 => number.ToString("0,,,,,.##q", CultureInfo.InvariantCulture),
				> 999999999999 => number.ToString("0,,,,.##T", CultureInfo.InvariantCulture),
				> 999999999 => number.ToString("0,,,.##B", CultureInfo.InvariantCulture),
				> 999999 => number.ToString("0,,.##M", CultureInfo.InvariantCulture),
				> 999 => number.ToString("0,.##K", CultureInfo.InvariantCulture),
				_ => number.ToString(CultureInfo.InvariantCulture)
			};
		}
	}
}