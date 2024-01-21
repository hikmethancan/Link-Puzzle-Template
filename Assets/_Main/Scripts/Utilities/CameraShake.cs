using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Utilities
{
	/// <summary>
	/// Handles the camera shake.
	/// <br/> Needs to be attached to a Cinemachine Virtual Camera.
	/// Works best with 6D Shake Noise Profile
	/// </summary>
	[RequireComponent(typeof(CinemachineVirtualCamera))]
	public class CameraShake : MonoBehaviour
	{
		[NoiseSettingsProperty]
		[SerializeField] private NoiseSettings noiseProfile;

		private CinemachineBasicMultiChannelPerlin perlin;

		private float startingIntensity;
		private float shakeTimer;
		private float shakeTimerTotal;

		public event UnityAction OnComplete;

		private Coroutine shakeCoroutine;

		private void Awake()
		{
			var vcam = GetComponent<CinemachineVirtualCamera>();
			perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			if (!perlin)
				perlin = vcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

			perlin.m_NoiseProfile = noiseProfile;
			perlin.m_AmplitudeGain = 0;
		}

		private void OnDisable()
		{
			StopShaking();
		}

		/// <summary>
		/// Shakes the camera
		/// </summary>
		/// <param name="intensity">How much shake will be applied to camera</param>
		/// <param name="duration">How long the camera shake will take</param>
		/// <param name="isSmooth">Will the shake slow down gradually?</param>
		public void Shake(float intensity, float duration, bool isSmooth = true)
		{
			perlin.m_AmplitudeGain = intensity;

			startingIntensity = intensity;
			shakeTimer = duration;
			shakeTimerTotal = duration;

			if (shakeCoroutine is not null)
				StopCoroutine(shakeCoroutine);
			shakeCoroutine = StartCoroutine(ShakeCoroutine(isSmooth));
		}

		/// <summary>
		/// Shakes the camera
		/// </summary>
		/// <param name="intensity">How much shake will be applied to camera</param>
		/// <param name="frequency">How rapidly will the shake applied</param>
		/// <param name="duration">How long the camera shake will take</param>
		/// <param name="isSmooth">Will the shake slow down gradually?</param>
		public void Shake(float intensity, float frequency, float duration, bool isSmooth = true)
		{
			perlin.m_FrequencyGain = frequency;

			Shake(intensity, duration, isSmooth);
		}

		public void StopShaking()
		{
			shakeTimer = 0;
			perlin.m_AmplitudeGain = 0;
			if (shakeCoroutine is not null)
				StopCoroutine(shakeCoroutine);
		}

		private IEnumerator ShakeCoroutine(bool isSmooth)
		{
			while (shakeTimer > 0)
			{
				shakeTimer -= Time.deltaTime;

				if (isSmooth)
					perlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0, 1 - (shakeTimer / shakeTimerTotal));
				else
				{
					if (shakeTimer <= 0)
						perlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0, 1 - (shakeTimer / shakeTimerTotal));
				}

				yield return new WaitForSeconds(Time.deltaTime);
			}

			perlin.m_AmplitudeGain = 0;
			perlin.m_FrequencyGain = 1;

			OnComplete?.Invoke();
		}
	}
}