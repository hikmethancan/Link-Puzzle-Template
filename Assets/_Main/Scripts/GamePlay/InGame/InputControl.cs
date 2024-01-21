using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.GridSystem;
using _Main.Scripts.Managers;
using _Main.Scripts.Utilities;
using _Main.Scripts.Utilities.Singletons;
using DG.Tweening;
using JetBrains.Annotations;
using TapticPlugin;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Main.Scripts.GamePlay.InGame
{
    public class InputControl : Singleton<InputControl>
    {
        [Header("Values")] [SerializeField] private int matchedBasketsBombValue;

        [Space] [Space] [Space] [SerializeField]
        private LayerMask drawLayer;

        [SerializeField] private Image fillImage;
        [SerializeField] private List<BallShelfController> shelfs = new List<BallShelfController>();
        [SerializeField] private GameObject combotextsParent;
        [SerializeField] private List<GameObject> comboTexts = new List<GameObject>();

        private Ray _ray;
        private Camera _camera;
        private RaycastHit _hit;
        private GridTile _firstDrawedGridTile;
        private GridTile _lastDrawedGridTile;
        public List<GridTile> _drawedTiles;

        private bool _canClick;

        private void OnEnable()
        {
            _camera = Camera.main;
            _drawedTiles = new List<GridTile>();
            fillImage.fillAmount = 0;
            _canClick = true;
            combotextsParent.SetActive(false);
            _matchCoroutine = null;
        }

        private void OnDisable()
        {
            if (_matchCoroutine is not null)
                _matchCoroutine = null;
        }

        private void Update()
        {
            Drawing();
        }

        private void Drawing()
        {
            FillSelectedImage();

            if (!_canClick) return;
            if (Input.GetMouseButtonDown(0))
            {
                FirstClickToTile();
            }

            if (Input.GetMouseButton(0))
            {
                if (UpdateDrawableTiles()) return;
                UpdateLineRenderers();
                UpdateDrawedGridsBall();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if(_firstDrawedGridTile is null) return;
                if (!_canClick) return;
                Debug.Log("hi");
                if (_matchCoroutine is null)
                    _matchCoroutine = StartCoroutine(CheckMatchCondition());
            }
        }

        private Coroutine _matchCoroutine;
        private GridTile _lastBombTile;
        private BasketBall _bombBall;

        private IEnumerator CheckMatchCondition()
        {
            _lastBombTile = null;
            _bombBall = null;
            if (_drawedTiles.Count > 0)
            {
                _canClick = false;
            }
            if (_drawedTiles.Count >= 3)
            {
                foreach (var tile in _drawedTiles)
                {
                    tile.UpdateLineRendererPosition(tile.ItemSnapPoint.position);
                }

                CheckMatchedBasketBallsIsGoal();
                foreach (var tile in _drawedTiles)
                {
                    tile.ActiveBall?.DisableWithScale();
                    tile.ActiveBall?.Release(Vector3.zero);
                    tile.SetActiveBall(null);
                    yield return new WaitForSeconds(0.1f);
                }


                if (_drawedTiles.Count >= matchedBasketsBombValue)
                {
                    _lastBombTile = _drawedTiles.Last();
                    _bombBall = GridManager.Instance.SpawnBombBall(_lastBombTile);
                    var tempScale = _bombBall.transform.localScale * 1.5f;
                    _bombBall.transform.DOScale(tempScale, .4f).SetLoops(3, LoopType.Yoyo).OnComplete(() => { });
                    _drawedTiles.Remove(_lastBombTile);
                }

                DeleteDrawedTiles();
                yield return new WaitForSeconds(0.4f);
                GridManager.Instance.SpawnNewBallsAfterTheDeleting();
            }

            yield return new WaitForSeconds(0.4f);

            GridManager.Instance.SpawnNewBallsAfterTheDeleting();
            yield return new WaitForSeconds(0.4f);
            if (_lastBombTile)
            {
                GridManager.Instance.ActivateBombMatch(_bombBall.GridIndex);
                var explosion = GridManager.Instance.BombExplosionParticle();
                explosion.transform.position = _bombBall.transform.position + Vector3.up * 2;
                explosion.Play();
                yield return new WaitForSeconds(0.4f);
                GridManager.Instance.SpawnNewBallsAfterTheDeleting();
            }

            _canClick = true;
            DeleteDrawedTiles();
            _matchCoroutine = null;

            _matchCoroutine = null;
        }

        private void DecreaseMoveCount(bool isHaveMatched)
        {
            if (!isHaveMatched)
            {
                GridManager.Instance.ActiveLevelGridData.moveCount--;
                if (GridManager.Instance.ActiveLevelGridData.moveCount == 0)
                {
                    if (StateManager.Instance.CurrentState != GameState.OnWin &&
                        StateManager.Instance.CurrentState != GameState.OnLose)
                        GameManager.OnGameLose?.Invoke();
                }

                UIManager.Instance.SetMoveCountText();
            }
        }

        private void CheckMatchedBasketBallsIsGoal()
        {
            SetComboTexts(_drawedTiles.Count);
            bool haveMatched = false;
            var goals = GridManager.Instance.ActiveLevelGridData.goals;

            var selected =
                goals.FirstOrDefault(x => x.basketBallType == _drawedTiles.FirstOrDefault()!.ActiveBall.ballType)!;
            if (selected is not null)
            {
                if (selected.count > 0)
                {
                    haveMatched = true;
                    var rnd = Random.Range(0, 2);
                    var shelf = shelfs[rnd];
                    shelf.AddBalls(_drawedTiles.FirstOrDefault()!.ActiveBall.ballType, _drawedTiles.Count, rnd);
                }
                else
                {
                    if (GridManager.Instance.ActiveLevelGridData.isAllColorInclude)
                    {
                        var allSelected =
                            goals.FirstOrDefault(x => x.basketBallType == BasketBallType.All)!;
                        if (allSelected is not null)
                        {
                            if (allSelected.count > 0)
                            {
                                haveMatched = true;
                                var rnd = Random.Range(0, 2);
                                var shelf = shelfs[rnd];
                                shelf.AddBalls(_drawedTiles.FirstOrDefault()!.ActiveBall.ballType, _drawedTiles.Count,
                                    rnd);
                            }
                        }
                    }
                }
            }

            else if (GridManager.Instance.ActiveLevelGridData.isAllColorInclude)
            {
                var allSelected =
                    goals.FirstOrDefault(x => x.basketBallType == BasketBallType.All)!;
                if (allSelected is not null)
                {
                    if (allSelected.count > 0)
                    {
                        haveMatched = true;
                        var rnd = Random.Range(0, 2);
                        var shelf = shelfs[rnd];
                        shelf.AddBalls(_drawedTiles.FirstOrDefault()!.ActiveBall.ballType, _drawedTiles.Count, rnd);
                    }
                }
            }

            DecreaseMoveCount(haveMatched);
        }

        private async Task DelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }

        private bool UpdateDrawableTiles()
        {
            // if (!_canClick) return false;
            _ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, 100f, drawLayer))
            {
                if (_hit.collider.TryGetComponent(out GridTile tile))
                {
                    if (!_lastDrawedGridTile && _lastDrawedGridTile == tile) return true;
                    if (_drawedTiles.Contains(tile))
                    {
                        ReverseDrawedTiles(tile);
                        return true;
                    }

                    if (!CheckIsSameBoxType(tile, _lastDrawedGridTile)) return true;
                    if (!CheckAreNeighbours(tile, _lastDrawedGridTile)) return true;
                    tile.ActiveBall.PlaySelectedParticle();
                    _drawedTiles.Add(tile);
                    VibrationManager.SingleImpact(ImpactFeedback.Light);
                    _lastDrawedGridTile = tile;
                }
            }

            return false;
        }

        private async void SetComboTexts(int index)
        {
            combotextsParent.SetActive(true);

            for (int i = 0; i < comboTexts.Count; i++)
            {
                comboTexts[i].SetActive(false);
            }

            switch (index)
            {
                case 5:
                    comboTexts[0].SetActive(true);
                    break;
                case 6:
                    comboTexts[1].SetActive(true);
                    break;
                case 7:
                    comboTexts[2].SetActive(true);
                    break;
                case 10:
                    comboTexts[3].SetActive(true);
                    break;
            }

            await DelayAsync(1000);
            combotextsParent.SetActive(false);
            for (int i = 0; i < comboTexts.Count; i++)
            {
                comboTexts[i].SetActive(false);
            }
        }

        private void FillSelectedImage()
        {
            var fillValue = Mathf.InverseLerp(0, 10, _drawedTiles.Count);
            var smoothFill = Mathf.Lerp(fillImage.fillAmount, fillValue, 0.1f);
            fillImage.fillAmount = smoothFill;
        }

        private void UpdateDrawedGridsBall()
        {
            int drawedCount = _drawedTiles.Count;
            if (drawedCount == 0) return;
            for (int i = 0; i < drawedCount; i++)
            {
                _drawedTiles[i].ItemHoldingUpdate(true);
            }
        }

        private void FirstClickToTile()
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, 100f, drawLayer))
            {
                if (_hit.collider.TryGetComponent(out GridTile tile))
                {
                    _firstDrawedGridTile = tile;
                    _lastDrawedGridTile = tile;
                    if (!_drawedTiles.Contains(_firstDrawedGridTile))
                    {
                        VibrationManager.SingleImpact(ImpactFeedback.Light);
                        _drawedTiles.Add(_firstDrawedGridTile);
                        if (_firstDrawedGridTile.ActiveBall)
                        {
                            fillImage.color = _firstDrawedGridTile.ActiveBall.GetBallColor();
                            _firstDrawedGridTile.ItemHoldingUpdate(true);
                        }
                    }
                }
            }
        }

        private void DeleteDrawedTiles()
        {
            _firstDrawedGridTile = null;
            _lastDrawedGridTile = null;
            for (int i = 0; i < _drawedTiles.Count; i++)
            {
                _drawedTiles[i].UpdateLineRendererPosition(_drawedTiles[i].ItemSnapPoint.position);
                // _drawedTiles[i].ActiveBall?.DisableWithScale();
                _drawedTiles[i].ActiveBall?.Release(Vector3.zero);
                // _drawedTiles[i].SetActiveBall(null);
            }

            _drawedTiles.Clear();
        }

        private void ReverseDrawedTiles(GridTile gridTile)
        {
            int heldTileIndex = _drawedTiles.IndexOf(gridTile);
            int drawedTileCount = _drawedTiles.Count;
            int removeCount = (drawedTileCount - 1) - heldTileIndex;
            for (int i = 0; i < removeCount; i++)
            {
                if (_drawedTiles.Count <= 0)
                    break;
                GridTile tile = _drawedTiles[^1];
                tile.ActiveBall.Release(Vector3.zero);
                tile.UpdateLineRendererPosition(tile.ItemSnapPoint.position);
                _drawedTiles.Remove(tile);
            }

            _lastDrawedGridTile = _drawedTiles[heldTileIndex];
            var drawPos = _drawedTiles[^1].ItemSnapPoint.position;
            var pos = new Vector3(drawPos.x, 0f, drawPos.z);
            _drawedTiles[^1].UpdateLineRendererPosition(pos);
        }

        private void UpdateLastDrawedTile(GridTile gridTile, bool isWithVisualUpdate = true)
        {
            _lastDrawedGridTile = gridTile;

            if (isWithVisualUpdate)
            {
                _lastDrawedGridTile.ItemHoldingUpdate(true);
            }
        }

        private bool CheckIsSameBoxType([CanBeNull] GridTile tile1, [CanBeNull] GridTile tile2)
        {
            if (tile1 is null || tile2 is null) return false;
            if (tile1.ActiveBall == null || tile2.ActiveBall == null)
                return false;
            return tile1.ActiveBall?.ballType == tile2.ActiveBall?.ballType;
        }

        private bool CheckAreNeighbours(GridTile gridTile, GridTile neighbourCheckTile)
        {
            return gridTile.NeigbourTiles.Contains(neighbourCheckTile);
        }

        private void UpdateLineRenderers()
        {
            int drawedCount = _drawedTiles.Count;
            if (drawedCount == 0) return;
            for (int i = 0; i < drawedCount; i++)
            {
                if (i != drawedCount - 1)
                {
                    var drawPos = _drawedTiles[i + 1].ItemSnapPoint.position;
                    var pos = new Vector3(drawPos.x, 0f, drawPos.z);
                    _drawedTiles[i].UpdateLineRendererPosition(pos);
                }
            }
        }
    }
}