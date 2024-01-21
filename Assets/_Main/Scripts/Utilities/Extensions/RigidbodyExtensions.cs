using UnityEngine;

namespace _Main.Scripts.Utilities.Extensions
{
	public static class RigidbodyExtensions
	{
		/// <summary>
		/// Launches the rigidbody to the given position.
		/// </summary>
		/// <param name="targetPosition">The position which the object is to be launched</param>
		/// <param name="maxHeight">the maximum height that the object will reach</param>
		/// <param name="showGizmos">Whether you want to see the gizmos</param>
		public static void Throw(this Rigidbody rb, Vector3 targetPosition, float maxHeight, bool showGizmos = false)
		{
			var rbPos = rb.position;
			float g = Physics.gravity.y;
			float displacementY = targetPosition.y - rbPos.y;
			var displacementXZ = new Vector3(targetPosition.x - rbPos.x, 0, targetPosition.z - rbPos.z);
			float time = Mathf.Sqrt(-2 * maxHeight / g) + Mathf.Sqrt(2 * (displacementY - maxHeight) / g);
			var velocityY = Mathf.Sqrt(-2 * g * maxHeight) * Vector3.up;
			var velocityXZ = displacementXZ / time;
			rb.velocity = velocityY + velocityXZ;

			if (showGizmos)
			{
				ShowThrowGizmos(rbPos, rb.velocity, time, true);
			}
		}

		/// <summary>
		/// Launches the 2d rigidbody to the given position.
		/// </summary>
		/// <param name="targetPosition">The position which the object is to be launched</param>
		/// <param name="maxHeight">the maximum height that the object will reach</param>
		/// <param name="showGizmos">Whether you want to see the gizmos</param>
		public static void Throw(this Rigidbody2D rb, Vector2 targetPosition, float maxHeight, bool showGizmos = false)
		{
			var rbPos = rb.position;
			float g = Physics2D.gravity.y;
			float displacementY = targetPosition.y - rbPos.y;
			float displacementX = targetPosition.x - rbPos.x;
			float time = Mathf.Sqrt(-2 * maxHeight / g) + Mathf.Sqrt(2 * (displacementY - maxHeight) / g);
			var velocityY = Mathf.Sqrt(-2 * g * maxHeight) * Vector2.up;
			var velocityX = (displacementX / time) * Vector2.right;
			rb.velocity = velocityY + velocityX;

			if (showGizmos)
			{
				ShowThrowGizmos(rbPos, rb.velocity, time, false);
			}
		}

		private static void ShowThrowGizmos(Vector3 position, Vector3 velocity, float time, bool is3D)
		{
			Vector3 g = is3D ? Physics.gravity : Physics2D.gravity;
			var prevPos = position;
			const int resolution = 20;
			const float duration = 5f;
			for (int i = 0; i <= resolution; i++)
			{
				float simTime = i / (float)resolution * time;
				var displacement = (velocity * simTime) + simTime * simTime * g / 2f;
				var drawPoint = position + displacement;
				Debug.DrawLine(prevPos, drawPoint, Color.green, duration);
				prevPos = drawPoint;
			}
		}

		/// <summary>
		///   <para>Applies a force to a 2d rigidbody that simulates explosion effects.</para>
		/// </summary>
		/// <param name="explosionForce">The force of the explosion (which may be modified by distance).</param>
		/// <param name="explosionPosition">The centre of the circle within which the explosion has its effect.</param>
		/// <param name="explosionRadius">The radius of the circle within which the explosion has its effect.</param>
		/// <param name="upwardsModifier">Adjustment to the apparent position of the explosion to make it seem to lift objects.</param>
		/// <param name="mode">The method used to apply the force to its targets.</param>
		public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0f,ForceMode2D mode = ForceMode2D.Force)
		{
			var explosionDir = rb.position - explosionPosition;
			var explosionDistance = explosionDir.magnitude / explosionRadius;

			// Normalize without computing magnitude again
			if (upwardsModifier == 0)
				explosionDir /= explosionDistance;
			else
			{
				// From Rigidbody.AddExplosionForce doc:
				// If you pass a non-zero value for the upwardsModifier parameter, the direction
				// will be modified by subtracting that value from the Y component of the centre point.
				explosionDir.y += upwardsModifier;
				explosionDir.Normalize();
			}

			rb.AddForce(Mathf.Lerp(0, explosionForce, 1 - explosionDistance) * explosionDir, mode);
		}
	}
}