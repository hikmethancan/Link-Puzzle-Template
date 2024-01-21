using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.Datas;
using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.GridSystem;
using _Main.Scripts.GamePlay.InGame;
using _Main.Scripts.Utilities;
using _Main.Scripts.Utilities.Singletons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Managers
{
    public class GridManager : Singleton<GridManager>
    {
        [SerializeField] private HoopController hoop;
        [SerializeField] private ParticleSystem bombParticle;
        [SerializeField] private BasketBall bombBall;
        [SerializeField] private GridTile gridPrefab;
        [SerializeField] private GoalController goalController;
        public LevelGridData ActiveLevelGridData => _activeLevelGridData;

        [Header("Privates")] public GridTile[,] GridTiles;
        private LevelGridData _activeLevelGridData;
        private const float GRID_OFFSET_VALUE = 2.5f;


        public void Setup()
        {
            SpawnGrid();
            AttachNeighbours();
        }

        public void SetLevelData(LevelGridData levelGridData)
        {
            _activeLevelGridData = levelGridData;
            UIManager.Instance.SetMoveCountText();
            if (levelGridData.isAllColorInclude)
            {
                Goal newGoal = new Goal();
                newGoal.basketBallType = BasketBallType.All;
                newGoal.count = _activeLevelGridData.allColorCount;
                _activeLevelGridData.goals.Add(newGoal);
                goalController.IsAllColorInclude = true;
            }
            else
            {
                goalController.IsAllColorInclude = false;
            }

            goalController.SetGoals(_activeLevelGridData.goals);
        }

        public void SetGoalUIValues(BasketBallType type, int count)
        {
            if (_activeLevelGridData.isAllColorInclude)
            {
                // type = BasketBallType.All;
                // count = _activeLevelGridData.goals.FirstOrDefault()!.count;
            }

            goalController.SetCount(type, count);
        }

        private void SpawnGrid()
        {
            GridTiles = new GridTile[6, 6];

            float gridHalfSize = 3f * 0.5f;
            float columnSpawnOffsetX = ((3f * -6) * 0.5f) + gridHalfSize;

            for (int x = 0; x < 6; x++)
            {
                float columnSpawnOffsetZ = ((3f * -6f) * 0.5f) + gridHalfSize;

                for (int y = 0; y < 6; y++)
                {
                    GridTile gridTile = Instantiate(gridPrefab, transform);
                    gridTile.gameObject.name += $"{x}_{y}";
                    Vector3 spawnPoint = new Vector3(columnSpawnOffsetX, 0f, columnSpawnOffsetZ);
                    gridTile.Initialize(spawnPoint, new Vector2Int(x, y), GetRandomBallForInitializingGrid());
                    GridTiles[x, y] = gridTile;
                    columnSpawnOffsetZ += GRID_OFFSET_VALUE;
                }

                columnSpawnOffsetX += GRID_OFFSET_VALUE;
            }
        }

        private void AttachNeighbours()
        {
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    List<GridTile> neighbourTiles = new List<GridTile>(4);

                    Vector2Int leftNeighbourIndex = new Vector2Int(x - 1, y);
                    Vector2Int leftUpNeighbourIndex = new Vector2Int(x - 1, y + 1);
                    Vector2Int rightNeighbourIndex = new Vector2Int(x + 1, y);
                    Vector2Int rightUpNeighbourIndex = new Vector2Int(x + 1, y + 1);
                    Vector2Int upNeighbourIndex = new Vector2Int(x, y + 1);
                    Vector2Int downNeighbourIndex = new Vector2Int(x, y - 1);
                    Vector2Int leftDownNeighbourIndex = new Vector2Int(x - 1, y - 1);
                    Vector2Int rightDownNeighbourIndex = new Vector2Int(x + 1, y - 1);

                    Vector2Int[] neighbourIndexes = new Vector2Int[8]
                    {
                        leftNeighbourIndex,
                        rightNeighbourIndex,
                        upNeighbourIndex,
                        downNeighbourIndex,
                        leftUpNeighbourIndex,
                        rightUpNeighbourIndex,
                        leftDownNeighbourIndex,
                        rightDownNeighbourIndex
                    };

                    for (int i = 0; i < neighbourIndexes.Length; i++)
                    {
                        if ((neighbourIndexes[i].x >= 0f && neighbourIndexes[i].x < 6)
                            && (neighbourIndexes[i].y >= 0f && neighbourIndexes[i].y < 6))
                        {
                            neighbourTiles.Add(GridTiles[neighbourIndexes[i].x, neighbourIndexes[i].y]);
                        }
                    }

                    GridTiles[x, y].SetNeighbours(neighbourTiles);
                }
            }
        }


        [ContextMenu("Fill")]
        public void SpawnNewBallsAfterTheDeleting()
        {
            MoveAllTilesToEmptySpaces();
            FillAllEmptySpaces();
        }

        public BasketBall SpawnBombBall(GridTile tile)
        {
            var bomb = Instantiate(bombBall);
            tile.Initialize(tile.ItemSnapPoint.position, new Vector2Int(tile.Index.x, tile.Index.y), bomb);
            return bomb;
        }

        public ParticleSystem BombExplosionParticle()
        {
            var bomb = Instantiate(bombParticle);
            return bomb;
        }

        public void ActivateBombMatch(Vector2Int tileIndex)
        {
            var index = tileIndex;
            var startX = Mathf.Clamp(index.x - 1, 0, 6);
            var startY = Mathf.Clamp(index.y - 1, 0, 6);

            var endX = Mathf.Clamp(startX + 3, 0, 6);
            if (startX == 0)
            {
                endX = Mathf.Clamp(index.x + 2, 0, 6);
            }
            else if (startX == 6)
            {
                endX = 6;
            }

            var endY = Mathf.Clamp(startY + 3, 0, 6);
            if (startY == 0)
            {
                endY = Mathf.Clamp(index.y + 2, 0, 6);
            }
            else if (startY == 6)
            {
                endY = 6;
            }

            
            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    hoop.CheckMatchedBasketBallsIsGoal(GridTiles[i,j].ActiveBall.ballType);
                    GridTiles[i, j].ActiveBall?.DisableWithScale();
                    GridTiles[i, j].ActiveBall?.Release(Vector3.zero);
                    GridTiles[i, j].SetActiveBall(null);
                }
            }
        }

        private void FillAllEmptySpaces()
        {
            bool anyEmptySpace = true;

            while (anyEmptySpace)
            {
                anyEmptySpace = false;

                for (int x = 5; x >= 0; x--)
                {
                    for (int y = 5; y >= 0; y--)
                    {
                        if (y == 5)
                        {
                            if (!GridTiles[x, y].ActiveBall)
                            {
                                var spawnedBall = GetRandomBallForInitializingGrid();
                                GridTiles[x, y].Initialize(GridTiles[x, y].transform.localPosition,
                                    new Vector2Int(x, y),
                                    spawnedBall, true);
                                MoveBallToEmptySpaceBelow(x, y);
                                anyEmptySpace = true;
                            }
                            else
                            {
                                if (!GridTiles[x, y - 1].ActiveBall)
                                {
                                    MoveBallToEmptySpaceBelow(x, y);
                                    anyEmptySpace = true;
                                }
                            }
                        }
                        else if (GridTiles[x, y].ActiveBall)
                        {
                            if (y != 0 && !GridTiles[x, y - 1].ActiveBall)
                            {
                                MoveBallToEmptySpaceBelow(x, y);
                                anyEmptySpace = true;
                            }
                        }
                    }
                }
            }
        }



        private void MoveAllTilesToEmptySpaces()
        {
            for (int x = 5; x >= 0; x--)
            {
                for (int y = 5; y >= 0; y--)
                {
                    if (y == 5)
                    {
                        if (!GridTiles[x, y].ActiveBall)
                        {
                            GridTiles[x, y].Initialize(GridTiles[x, y].transform.localPosition, new Vector2Int(x, y),
                                GetRandomBallForInitializingGrid());

                            // Move the ball to the empty space below
                            MoveBallToEmptySpaceBelow(x, y);
                        }
                    }
                    else if (GridTiles[x, y].ActiveBall)
                    {
                        if (y != 0)
                        {
                            if (!GridTiles[x, y - 1].ActiveBall)
                            {
                                // Move the ball to the empty space below
                                MoveBallToEmptySpaceBelow(x, y);
                            }
                        }
                    }
                }
            }
        }

        private void MoveBallToEmptySpaceBelow(int x, int y)
        {
            // Check if the current grid has an active ball
            if (GridTiles[x, y].ActiveBall)
            {
                // Check if it's not on the bottom row and the cell below is empty
                if (y > 0 && !GridTiles[x, y - 1].ActiveBall)
                {
                    // Find the first empty space below and move the ball
                    for (int i = y - 1; i >= 0; i--)
                    {
                        if (!GridTiles[x, i].ActiveBall)
                        {
                            // Move the ball to the empty space below
                            GridTiles[x, i].InitializeItem(GridTiles[x, y].ActiveBall, true);
                            GridTiles[x, i].SetActiveBall(GridTiles[x, y].ActiveBall);
                            GridTiles[x, y].SetActiveBall(null);
                            break;
                        }
                        else if (GridTiles[x, i].ActiveBall && i == 0)
                        {
                            // Check if there's an active ball at the top grid
                            if (GridTiles[x, y].ActiveBall)
                            {
                                // Move the active ball to the top row
                                MoveBallToEmptySpaceBelow(x, y);
                                return;
                            }
                        }
                    }
                }
                else if (y == 0)
                {
                    // Handle the case where the bottom row has an active ball
                    // This might involve spawning a new ball at the top row
                    // or any other logic based on your game requirements

                    // Check if there's space above to move the ball
                    if (!GridTiles[x, y].ActiveBall)
                    {
                        // Move the ball to the top row
                        GridTiles[x, y].InitializeItem(GridTiles[x, y].ActiveBall, true);
                        GridTiles[x, y].SetActiveBall(GridTiles[x, y].ActiveBall);
                        GridTiles[x, y].SetActiveBall(null);
                    }
                    else
                    {
                        // If there's no space above, spawn a new ball at the top row
                        var newBall = GetRandomBallForInitializingGrid();
                        GridTiles[x, 0].Initialize(GridTiles[x, 0].transform.localPosition,
                            new Vector2Int(x, 0), newBall, true);
                    }
                }
            }
        }


        public BasketBall GetRandomBallForInitializingGrid()
        {
            return PoolManager.Instance.GetBasketBall(GetBallIndex());
        }

        private int GetBallIndex()
        {
            int index = 0;
            int rndValue = Random.Range(1, 101);

            List<int> spawnableIndexes =
                _activeLevelGridData.colorSpawnDatas
                    .Where(x => x.chancePercent >= rndValue)
                    .Select(x => (int)x.basketballType)
                    .ToList();

            index = spawnableIndexes.GetRandomElement();

            return index;
        }
    }
}