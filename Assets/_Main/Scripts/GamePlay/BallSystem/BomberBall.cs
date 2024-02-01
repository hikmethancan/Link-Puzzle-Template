using System.Collections;
using _Main.Scripts.Datas;
using _Main.Scripts.GamePlay.BallSystem.Abstract;
using _Main.Scripts.GamePlay.GridSystem;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.GamePlay.BallSystem
{
    public class BomberBall : BallBase
    {
        [SerializeField] private BombData bombData;

        public IEnumerator ExecuteBomb(Tile[,] gridTiles, Vector2Int tileIndex, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            var tempScale = transform.localScale * 1.5f;
            transform.DOScale(tempScale, bombData.explosionWaitDuration).SetLoops(3, LoopType.Yoyo);
            yield return new WaitForSeconds(bombData.explosionWaitDuration);
            ActivateBombMatch(gridTiles, tileIndex);
        }

        private void ActivateBombMatch(Tile[,] gridTiles, Vector2Int tileIndex)
        {
            var index = tileIndex;
            var startX = Mathf.Clamp(index.x - 1, 0, gridTiles.GetLength(0));
            var startY = Mathf.Clamp(index.y - 1, 0, gridTiles.GetLength(1));

            var endX = startX switch
            {
                0 => Mathf.Clamp(index.x + 2, 0, gridTiles.GetLength(0)),
                _ => startX == gridTiles.GetLength(0)
                    ? gridTiles.GetLength(0)
                    : Mathf.Clamp(startX + 3, 0, gridTiles.GetLength(0))
            };

            var endY = startY switch
            {
                0 => Mathf.Clamp(index.y + 2, 0, gridTiles.GetLength(1)),
                _ => startY == gridTiles.GetLength(1)
                    ? gridTiles.GetLength(1)
                    : Mathf.Clamp(startY + 3, 0, gridTiles.GetLength(1))
            };

            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    gridTiles[i, j].ActiveBall?.DisableWithScale();
                    gridTiles[i, j].ActiveBall?.Release(Vector3.zero);
                    gridTiles[i, j].SetActiveBall(null);
                }
            }
        }
    }
}