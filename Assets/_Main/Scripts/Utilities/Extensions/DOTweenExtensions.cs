using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace _Main.Scripts.Utilities.Extensions
{
	public static class DOTweenExtensions
	{
		#region BlendShape

		/// <summary>
		/// Tweens a BlendShape weight to the given value from current weight
		/// </summary>
		/// <param name="index">Index of the BlendShape in the SkinnedMeshRenderer</param>
		/// <param name="target">Value of the weight will be tweened to. Between 0-100</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>The tween</returns>
		public static TweenerCore<float, float, FloatOptions> DOBlendShape(this SkinnedMeshRenderer renderer, int index, float target, float duration)
		{
			float value = renderer.GetBlendShapeWeight(index);
			return DOTween.To(() => value, x =>
			{
				value = x;
				renderer.SetBlendShapeWeight(index, value);
			}, target, duration).SetTarget(renderer);
		}

		/// <summary>
		/// Tweens a BlendShape weight to the given value from a given value
		/// </summary>
		/// <param name="index">Index of the BlendShape in the SkinnedMeshRenderer</param>
		/// <param name="from">Value of the weight will be tweened from. Between 0-100</param>
		/// <param name="target">Value of the weight will be tweened to. Between 0-100</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>The tween</returns>
		public static TweenerCore<float, float, FloatOptions> DOBlendShape(this SkinnedMeshRenderer renderer, int index, float from, float target, float duration)
		{
			renderer.SetBlendShapeWeight(index, from);
			return renderer.DOBlendShape(index, target, duration);
		}

		#endregion

		#region Camera

		/// <summary>
		/// Tweens the Cinemachine Transposer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraOffSet(this CinemachineTransposer vCam, Vector3 endValue, float duration)
		{
			var value = vCam.m_FollowOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_FollowOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		/// <summary>
		/// Tweens the Cinemachine Framing Transposer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraOffSet(this CinemachineFramingTransposer vCam, Vector3 endValue, float duration)
		{
			var value = vCam.m_TrackedObjectOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_TrackedObjectOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		/// <summary>
		/// Tweens the Cinemachine Orbital Transposer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraOffSet(this CinemachineOrbitalTransposer vCam, Vector3 endValue, float duration)
		{
			var value = vCam.m_FollowOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_FollowOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		/// <summary>
		/// Tweens the Cinemachine aim Composer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraAimOffSet(this CinemachineComposer vCam, Vector3 endValue, float duration)
		{
			var value = vCam.m_TrackedObjectOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_TrackedObjectOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		/// <summary>
		/// Tweens the Cinemachine aim Group Composer offset to the given Vector3
		/// </summary>
		/// <param name="endValue">Vector of the offset will be tweened to</param>
		/// <param name="duration">Tween duration</param>
		/// <returns>The tween</returns>
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOCameraAimOffSet(this CinemachineGroupComposer vCam, Vector3 endValue, float duration)
		{
			var value = vCam.m_TrackedObjectOffset;
			return DOTween.To(() => value, x =>
			{
				value = x;
				vCam.m_TrackedObjectOffset = value;
			}, endValue, duration).SetTarget(vCam);
		}

		#endregion

		#region LineRenderer

		/// <summary>
		/// Tweens the color of the LineRenderer
		/// </summary>
		/// <param name="endValue">Color</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>Tween</returns>
		public static TweenerCore<Color, Color, ColorOptions> DOColor(this LineRenderer line, Color endValue, float duration)
		{
			return DOTween.To(() => line.startColor, line.SetColor, endValue, duration).SetTarget(line);
		}

		/// <summary>
		/// Tweens the start width of the LineRenderer
		/// </summary>
		/// <param name="endValue">Final value of the width</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>Tween</returns>
		public static TweenerCore<float, float, FloatOptions> DOStartWidth(this LineRenderer line, float endValue, float duration)
		{
			return DOTween.To(() => line.startWidth, x => line.startWidth = x, endValue, duration).SetTarget(line);
		}

		/// <summary>
		/// Tweens the end width of the LineRenderer
		/// </summary>
		/// <param name="endValue">Final value of the width</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>Tween</returns>
		public static TweenerCore<float, float, FloatOptions> DOEndWidth(this LineRenderer line, float endValue, float duration)
		{
			return DOTween.To(() => line.endWidth, x => line.endWidth = x, endValue, duration).SetTarget(line);
		}

		/// <summary>
		/// Tweens the  width of the LineRenderer
		/// </summary>
		/// <param name="endValue">Final value of the width</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>Tween</returns>
		public static TweenerCore<float, float, FloatOptions> DOWidth(this LineRenderer line, float endValue, float duration)
		{
			return DOTween.To(() => line.startWidth, x =>
			{
				line.endWidth = x;
				line.startWidth = x;
			}, endValue, duration).SetTarget(line);
		}

		/// <summary>
		/// Tweens the count of the LineRenderer
		/// </summary>
		/// <param name="endValue">Final value of the count</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>Tween</returns>
		public static TweenerCore<int, int, NoOptions> DOCount(this LineRenderer line, int endValue, float duration)
		{
			return DOTween.To(() => line.positionCount, x => line.positionCount = x, endValue, duration).SetTarget(line);
		}

		#endregion

		#region Light

		/// <summary>
		/// Tweens the intensity of the light
		/// </summary>
		/// <param name="finalIntensity">Final value of the intensity</param>
		/// <param name="duration">Duration of the tween</param>
		/// <returns>Tween</returns>
		public static TweenerCore<float, float, FloatOptions> DOIntensity(this Light light, float finalIntensity, float duration)
		{
			return DOTween.To(() => light.intensity, x => light.intensity = x, finalIntensity, duration).SetTarget(light);
		}

		#endregion
	}
}