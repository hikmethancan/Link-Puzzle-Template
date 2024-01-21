using System.Collections.Generic;
using _Main.Scripts.GamePlay.InGame;
using _Main.Scripts.Utilities.Extensions;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.GamePlay.GridSystem
{
    public class Tile : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform itemsSnapPoint;

        [SerializeField] private LineRenderer drawLine;
        [Header("Privates")] private Ball _activeBall;
        private Vector2Int _index;
        private List<Tile> _neighbourTiles;
        private const float BasketBallsMoveDuration = .2f;
        
        public Ball ActiveBall => _activeBall;
        public List<Tile> NeigbourTiles => _neighbourTiles;
        public Transform ItemSnapPoint => itemsSnapPoint;
        public Vector2Int Index => _index;


        public void SetDrawLineColor(Color color)
        {
            drawLine.SetColor(color);
        }
        public void Initialize(Vector3 position,Vector2Int index,Ball initializeItem,bool isFillingItem = false)
        {
            if(!isFillingItem)
                transform.localPosition = position;
            _index = index;
            initializeItem.GridIndex = index;
            if(isFillingItem)
                initializeItem.transform.position = new Vector3(position.x,0f,position.z)+ Vector3.forward * 5f;
            InitializeItem(initializeItem,isFillingItem);
        }

        public void SetActiveBall(Ball item)
        {
            _activeBall = item;
        }

        public void SetNeighbours(List<Tile> neighbourTiles)
        {
            _neighbourTiles = neighbourTiles;
        }

        public void UpdateLineRendererPosition(Vector3 pos)
        {
            var position = itemsSnapPoint.position;
            var startPos = new Vector3(position.x, 0f, position.z);
            drawLine.SetPosition(0,startPos);
            drawLine.SetPosition(1,pos);
            
        }
        public void InitializeItem(Ball initializeItem,bool shouldMoveWithDotween = false)
        {
            if(!initializeItem) return;
            _activeBall = initializeItem;
            Transform transform1;
            initializeItem.GridIndex = _index;
            var transformPosition = transform.position;
            transformPosition.y = 0;
            transform.position = transformPosition;

            (transform1 = _activeBall.transform).SetParent(transform);
            if (shouldMoveWithDotween)
                transform1.DOLocalMove(Vector3.zero, BasketBallsMoveDuration);
            else
                transform1.localPosition = Vector3.zero;
            SetDrawLineColor(initializeItem.GetBallColor());
            if(!_activeBall)
                return;
            _activeBall.Release(ItemSnapPoint.position,ItemSnapPoint);
        }

        public void MoveBallToGrid(Ball moveBall,Transform parent)
        {
            moveBall.transform.SetParent(parent);
            moveBall.transform.DOLocalMove(Vector3.zero, BasketBallsMoveDuration);
            SetActiveBall(moveBall);
        }

        public void ItemHoldingUpdate(bool isHolding)
        {
            if (!_activeBall) return;
            
            _activeBall.HoldingUpdate(isHolding);
        }

        public void SendItemToTarget(Transform target)
        {
            if(!_activeBall) return;
            if (!target)
            {
                Debug.LogWarning("Target is null");
                return;
            }
            _activeBall.MoveToTarget(target);
            _activeBall = null;
        }

        public void DisableItemWithScale()
        {
            if(!_activeBall) return;
            _activeBall.DisableWithScale();
            _activeBall = null;
        }
    }
}
