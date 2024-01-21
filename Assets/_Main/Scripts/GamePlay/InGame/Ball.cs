using System.Collections;
using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.GridSystem;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.GamePlay.InGame
{
    public class Ball : MonoBehaviour
    {
        public BallType ballType;
        public Vector2Int GridIndex { get; set; }

        [Header("References")] [SerializeField]
        private ParticleSystem selectedParticle;

        [SerializeField] private GameObject model;
        [SerializeField] private GameObject moveTrail;
        [SerializeField] private ParticleSystem matchedParticle;
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private GameObject ballCircle;

        private void OnEnable()
        {
            model.SetActive(true);
            model.transform.localScale = Vector3.one;
            ballCircle.SetActive(true);
            moveTrail.SetActive(false);
        }

        public void Release(Vector3 position, Transform parent = null)
        {
            ballCircle.SetActive(false);
            // Animator Release
            selectedParticle.Stop();
        }

        public void DisableWithScale()
        {
            matchedParticle.Play();
            var sq = DOTween.Sequence();
            sq.Append(model.transform.DOScale(Vector3.zero, .4f).SetEase(Ease.InBack)).AppendInterval(.5f)
                .AppendCallback(() => gameObject.SetActive(false));
        }

        public void ShelfSetup()
        {
            moveTrail.SetActive(false);
            ballCircle.SetActive(false);
        }

        public void PlayerSetup()
        {
            moveTrail.SetActive(true);
            ballCircle.SetActive(false);
        }

        public void MoveToTarget(Transform target)
        {
        }

        public void PlaySelectedParticle()
        {
            selectedParticle.Play();
        }

        private void MoveToGrid(Tile tile)
        {
            transform.SetParent(tile.transform);
            transform.DOLocalMove(Vector3.zero, .5f);
            tile.SetActiveBall(this);
        }

        public void HoldingUpdate(bool isHolding)
        {
            ballCircle.SetActive(true);
            //TODO Scale Up Down Twenn
        }

        public Color GetBallColor()
        {
            return mesh.material.GetColor("_MainColor1");
        }
    }
}