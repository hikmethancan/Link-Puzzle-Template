using System;
using _Main.Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text guestScore;
        [SerializeField] private TMP_Text homeScore;
        [SerializeField] private TMP_Text timer;
        [SerializeField] private float countdownDuration = 100.0f; // Geri sayım süresi
        
        private float _currentTime = 0.0f;
        private bool _isTimerRunning = false;
        private int _tempHomeScore;

        private void OnEnable()
        {
            EventManager.OnIncreaseHomeScore += IncreaseHomeScore;
        }

        private void OnDisable()
        {
            EventManager.OnIncreaseHomeScore -= IncreaseHomeScore;
        }

        private void Start()
        {
            _tempHomeScore = 0;
            SetHomeScore();
            RandomGuestScore();
            StartCountdown();
        }

       
        

        private void Update()
        {
            if (_isTimerRunning)
            {
                _currentTime -= Time.deltaTime;

                // Zamanı güncelle
                UpdateCountdownUI();

                // Zaman bittiğinde yapılacak işlemler
                if (_currentTime <= 0.0f)
                {
                    FinishCountdown();
                }
            }
        }

        void StartCountdown()
        {
            _currentTime = countdownDuration;
            _isTimerRunning = true;
            UpdateCountdownUI();
        }

        void FinishCountdown()
        {
            _isTimerRunning = false;
            _currentTime = 0.0f;
            // Zaman bittiğinde yapılacak işlemler burada yapılabilir.
            Debug.Log("Zaman bitti!");
        }

        void UpdateCountdownUI()
        {
            // Zamanı metin olarak göster
            timer.text = Mathf.CeilToInt(_currentTime).ToString();
        }

        private void RandomGuestScore()
        {
            var rnd = Random.Range(10, 40);
            guestScore.SetText(rnd.ToString());
        }

        private void SetHomeScore()
        {
            homeScore.SetText($"{_tempHomeScore}");
            homeScore.transform.DOComplete();
            homeScore.transform.DOPunchScale(Vector3.one * 20f, .2f, 2, 1f).SetEase(Ease.OutBack)
                .OnComplete(() => homeScore.transform.DORewind());
        }

        public void IncreaseHomeScore()
        {
            _tempHomeScore++;
            SetHomeScore();
        }
    }
}