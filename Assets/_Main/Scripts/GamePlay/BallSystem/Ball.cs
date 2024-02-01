using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.BallSystem.Abstract;
using _Main.Scripts.GamePlay.GridSystem;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.GamePlay.BallSystem
{
    public class Ball : BallBase
    {
        public BallType ballType;
        public Vector2Int GridIndex { get; set; }


        [SerializeField] private GameObject model;
        [SerializeField] private TrailRenderer moveTrail;
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private GameObject ballCircle;


        private Sequence _scaleAnimationSequence;
        private Vector3 _baseScale;

        private void OnEnable()
        {
            _baseScale = transform.localScale;
            model.SetActive(true);
            model.transform.localScale = Vector3.one;
            ballCircle.SetActive(true);
            moveTrail.gameObject.SetActive(false);
            SetMoveTrailColor();
        }

        private void SetMoveTrailColor()
        {
            moveTrail.endColor = GetBallColor();
        }

        public void Release(Vector3 position, Transform parent = null)
        {
            _scaleAnimationSequence.Kill();
            ballCircle.SetActive(false);
        }

        public void DisableWithScale()
        {
            var sq = DOTween.Sequence();
            sq.Append(model.transform.DOScale(Vector3.zero, .4f).SetEase(Ease.InBack)).AppendInterval(.5f)
                .AppendCallback(() => gameObject.SetActive(false));
        }

        public void MoveToTarget(Transform target)
        {
        }

        // You can use the selected particle when selected the ball object.
        private void PlaySelectedParticle()
        {
        }

        private void MoveToGrid(Tile tile)
        {
            transform.SetParent(tile.transform);
            transform.DOLocalMove(Vector3.zero, .5f);
            tile.SetActiveBall(this);
        }

        public void Selected()
        {
            PlaySelectedParticle();
            ballCircle.SetActive(true);
            _scaleAnimationSequence = DOTween.Sequence();
            _scaleAnimationSequence.Append(transform.DOScale(_baseScale * 1.2f, .3f));
            _scaleAnimationSequence.Append(transform.DOScale(_baseScale, .3f));
            _scaleAnimationSequence.SetLoops(-1, LoopType.Yoyo);
        }


        // If you want to use something should always run when ball selected.
        public void HoldingUpdate(bool isHolding)
        {
        }

        public Color GetBallColor()
        {
            return mesh.material.color;
        }
    }
}