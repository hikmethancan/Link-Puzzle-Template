using _Main.Scripts.GamePlay.GridSystem;
using UnityEngine;

namespace _Main.Scripts.GamePlay.InGame
{
    public class BombControl : MonoBehaviour
    {
        public void ActivateBombMatch( Tile[,] gridTiles,Vector2Int tileIndex)
        {
            var index = tileIndex;
            var startX = Mathf.Clamp(index.x - 1, 0, gridTiles.GetLength(0));
            var startY = Mathf.Clamp(index.y - 1, 0, gridTiles.GetLength(1));

            var endX = startX switch
            {
                0 => Mathf.Clamp(index.x + 2, 0, gridTiles.GetLength(0)),
                6=> gridTiles.GetLength(0),
                _ => Mathf.Clamp(startX + 3, 0, gridTiles.GetLength(0))
            };

            var endY = startY switch
            {
                0 => Mathf.Clamp(index.y + 2, 0, gridTiles.GetLength(1)),
                6 => 6,
                _ => Mathf.Clamp(startY + 3, 0, gridTiles.GetLength(1))
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