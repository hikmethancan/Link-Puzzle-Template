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
        public Vector2Int  GridIndex { get; set; }
        
        [Header("References")] [SerializeField]
        private ParticleSystem selectedParticle;

        [SerializeField] private GameObject model;
        [SerializeField] private Rigidbody rb;
        [SerializeField]
        private GameObject moveTrail;
        [SerializeField] private ParticleSystem matchedParticle;
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private GameObject ballCircle;
        [SerializeField] private Animator animator;
        private static readonly int Shake = Animator.StringToHash("Shake");

        private void OnEnable()
        {
            model.SetActive(true);
            model.transform.localScale = Vector3.one;
            ballCircle.SetActive(true);
            moveTrail.SetActive(false);
            
            rb.isKinematic = true;
        }

        public void Release(Vector3 position,Transform parent =null)
        {
            ballCircle.SetActive(false);
            animator.SetBool(Shake,false);
            selectedParticle.Stop();
        }

        public void DisableWithScale()
        {
            matchedParticle.Play();
            var sq= DOTween.Sequence();
            sq.Append(model.transform.DOScale(Vector3.zero, .4f).SetEase(Ease.InBack)).AppendInterval(.5f)
                .AppendCallback(() => gameObject.SetActive(false));

        }

        public void OpenPhysicAfterTheBasket()
        {
            rb.isKinematic = false;
            var forceWay = new Vector3(Random.Range(-0.05f,0.05f), -0.1f, -.09f);
            rb.AddForce(forceWay * 100f,ForceMode.Impulse);
            if(gameObject.activeInHierarchy)
                StartCoroutine(FallRoutine());
        }

        private IEnumerator FallRoutine()
        {
            yield return new WaitForSeconds(1.3f);
            rb.isKinematic = true;
            gameObject.SetActive(false);
        }

        public void ShelfSetup()
        {
            moveTrail.SetActive(false);
            ballCircle.SetActive(false);
            animator.SetBool(Shake,false);
        }
        public void PlayerSetup()
        {
            moveTrail.SetActive(true);
            ballCircle.SetActive(false);
            animator.SetBool(Shake,false);
        }

        public void MoveToTarget(Transform target)
        {
            
        }

        public void PlaySelectedParticle()
        {
            selectedParticle.Play();
        }

        private void MoveToGrid(GridTile gridTile)
        {
            transform.SetParent(gridTile.transform);
            transform.DOLocalMove(Vector3.zero, .5f);
            gridTile.SetActiveBall(this);
        }

        public void HoldingUpdate(bool isHolding)
        {
            ballCircle.SetActive(true);
            animator.SetBool(Shake,true);
        }

        public Color GetBallColor()
        {
            return mesh.material.GetColor("_MainColor1");
        }
    }
}