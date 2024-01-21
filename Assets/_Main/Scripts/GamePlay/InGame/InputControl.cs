using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.GridSystem;
using _Main.Scripts.Managers;
using _Main.Scripts.Signals;
using _Main.Scripts.Utilities.Singletons;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using GameState = _Main.Scripts.Managers.GameState;

namespace _Main.Scripts.GamePlay.InGame
{
    public class InputControl : Singleton<InputControl>
    {
        [Header("Values")] [SerializeField] private int matchedBasketsBombValue;

        [Space] [Space] [Space] [SerializeField]
        private LayerMask drawLayer;

        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject combotextsParent;
        [SerializeField] private List<GameObject> comboTexts = new List<GameObject>();

        private Ray _ray;
        private Camera _camera;
        private RaycastHit _hit;
        private Tile _firstDrawedTile;
        private Tile _lastDrawedTile;
        public List<Tile> _drawedTiles;

        private bool _canClick;

        private void OnEnable()
        {
            _camera = Camera.main;
            _drawedTiles = new List<Tile>();
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
                if (_firstDrawedTile is null) return;
                if (!_canClick) return;
                Debug.Log("hi");
                if (_matchCoroutine is null)
                    _matchCoroutine = StartCoroutine(CheckMatchCondition());
            }
        }

        private Coroutine _matchCoroutine;
        private Tile _lastBombTile;
        private Ball _bombBall;

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
                GridManager.Instance.ActiveLevelData.moveCount--;
                if (GridManager.Instance.ActiveLevelData.moveCount == 0)
                {
                    if (GameManager.Instance.CurrentState != GameState.Win &&
                        GameManager.Instance.CurrentState != GameState.Lose)
                        GameEvents.onLose?.Invoke();
                }

                UIManager.Instance.SetMoveCountText();
            }
        }

        private void CheckMatchedBasketBallsIsGoal()
        {
            // TODO Buraya Goal Match Olunca olması gereken aksiyonlar yazilacak.!!!

            SetComboTexts(_drawedTiles.Count);
            bool haveMatched = false;
            var goals = GridManager.Instance.ActiveLevelData.goals;

            var selected =
                goals.FirstOrDefault(x => x.ballType == _drawedTiles.FirstOrDefault()!.ActiveBall.ballType)!;
            if (selected is not null)
            {
                if (selected.count > 0)
                {
                }
                else
                {
                    if (GridManager.Instance.ActiveLevelData.isAllColorInclude)
                    {
                        var allSelected =
                            goals.FirstOrDefault(x => x.ballType == BallType.All)!;
                        if (allSelected is not null)
                        {
                            if (allSelected.count > 0)
                            {
                            }
                        }
                    }
                }
            }

            else if (GridManager.Instance.ActiveLevelData.isAllColorInclude)
            {
                var allSelected =
                    goals.FirstOrDefault(x => x.ballType == BallType.All)!;
                if (allSelected is not null)
                {
                    if (allSelected.count > 0)
                    {
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
                if (_hit.collider.TryGetComponent(out Tile tile))
                {
                    if (!_lastDrawedTile && _lastDrawedTile == tile) return true;
                    if (_drawedTiles.Contains(tile))
                    {
                        ReverseDrawedTiles(tile);
                        return true;
                    }

                    if (!CheckIsSameBoxType(tile, _lastDrawedTile)) return true;
                    if (!CheckAreNeighbours(tile, _lastDrawedTile)) return true;
                    tile.ActiveBall.PlaySelectedParticle();
                    _drawedTiles.Add(tile);
                    _lastDrawedTile = tile;
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
                if (_hit.collider.TryGetComponent(out Tile tile))
                {
                    _firstDrawedTile = tile;
                    _lastDrawedTile = tile;
                    if (!_drawedTiles.Contains(_firstDrawedTile))
                    {
                        _drawedTiles.Add(_firstDrawedTile);
                        if (_firstDrawedTile.ActiveBall)
                        {
                            fillImage.color = _firstDrawedTile.ActiveBall.GetBallColor();
                            _firstDrawedTile.ItemHoldingUpdate(true);
                        }
                    }
                }
            }
        }

        private void DeleteDrawedTiles()
        {
            _firstDrawedTile = null;
            _lastDrawedTile = null;
            for (int i = 0; i < _drawedTiles.Count; i++)
            {
                _drawedTiles[i].UpdateLineRendererPosition(_drawedTiles[i].ItemSnapPoint.position);
                _drawedTiles[i].ActiveBall?.Release(Vector3.zero);
            }

            _drawedTiles.Clear();
        }

        private void ReverseDrawedTiles(Tile tile)
        {
            int heldTileIndex = _drawedTiles.IndexOf(tile);
            int drawedTileCount = _drawedTiles.Count;
            int removeCount = (drawedTileCount - 1) - heldTileIndex;
            for (int i = 0; i < removeCount; i++)
            {
                if (_drawedTiles.Count <= 0)
                    break;
                tile = _drawedTiles[^1];
                tile.ActiveBall.Release(Vector3.zero);
                tile.UpdateLineRendererPosition(tile.ItemSnapPoint.position);
                _drawedTiles.Remove(tile);
            }

            _lastDrawedTile = _drawedTiles[heldTileIndex];
            var drawPos = _drawedTiles[^1].ItemSnapPoint.position;
            var pos = new Vector3(drawPos.x, 0f, drawPos.z);
            _drawedTiles[^1].UpdateLineRendererPosition(pos);
        }

        private void UpdateLastDrawedTile(Tile tile, bool isWithVisualUpdate = true)
        {
            _lastDrawedTile = tile;

            if (isWithVisualUpdate)
            {
                _lastDrawedTile.ItemHoldingUpdate(true);
            }
        }

        private bool CheckIsSameBoxType([CanBeNull] Tile tile1, [CanBeNull] Tile tile2)
        {
            if (tile1 is null || tile2 is null) return false;
            if (tile1.ActiveBall == null || tile2.ActiveBall == null)
                return false;
            return tile1.ActiveBall?.ballType == tile2.ActiveBall?.ballType;
        }

        private bool CheckAreNeighbours(Tile tile, Tile neighbourCheckTile)
        {
            return tile.NeigbourTiles.Contains(neighbourCheckTile);
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