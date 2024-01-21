using _Main.Scripts.Utilities.Singletons;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace _Main.Scripts.UI
{
	public class LoadingPanelController : PersistenceSingleton<LoadingPanelController>
	{
		[Header("General Variables")]
		[SerializeField] private float minLoadingDuration = 4f;
		[SerializeField] private float maxLoadingDuration = 5f;
		[SerializeField] private Ease loadingEase;

		[Header("References")]
		[SerializeField] private Image imgFillBar;
		[SerializeField] private GameObject loadingPanelParent;
		[Space]
		[SerializeField] private Image imgBackground;
		[SerializeField] private Image imgLoadingScreen;
		[SerializeField] private Image imgLoadingScreenTitle;

		public event UnityAction OnLoadingFinished;

		private void Start()
		{
			imgFillBar.fillAmount = 0f;
			loadingPanelParent.SetActive(true);

			float _duration = Random.Range(minLoadingDuration, maxLoadingDuration);

			imgFillBar.DOFillAmount(1f, _duration).SetEase(loadingEase).SetLink(gameObject).SetTarget(gameObject).OnComplete(() =>
			{
				loadingPanelParent.SetActive(false);
				OnLoadingFinished?.Invoke();
			});
		}

		public void SetLoadingScreen(Sprite background, Sprite loadingScreen, Sprite loadingScreenTitle)
		{
			imgBackground.sprite = background;
			imgLoadingScreen.sprite = loadingScreen;
			imgLoadingScreenTitle.sprite = loadingScreenTitle;
		}
	}
}