using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.BallSystem;
using _Main.Scripts.GamePlay.GridSystem;
using _Main.Scripts.Managers;
using _Main.Scripts.Signals;
using _Main.Scripts.Utilities.Singletons;
using DG.Tweening;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using GameState = _Main.Scripts.Managers.GameState;

namespace _Main.Scripts.GamePlay.InGame
{
    public class LinkControl : Singleton<LinkControl>
    {
        [InfoBox("Link Value For Booster Bomber", EInfoBoxType.Warning)] [SerializeField]
        private int matchedBasketsBombValue;

        [Space] [Space] [Space] [SerializeField]
        private LayerMask drawLayer;


        #region Privates

        private Ray _ray;
        private Camera _camera;
        private RaycastHit _hit;
        private Tile _firstLinkedTile;
        private Tile _lastLinkedTile;
        [ReadOnly] private List<Tile> _linkedTiles;
        private bool _canClick;
        private Coroutine _matchCoroutine;
        private Tile _lastBombTile;
        private Ball _bombBall;

        #endregion


        private void OnEnable()
        {
            _camera = Camera.main;
            _linkedTiles = new List<Tile>();
            _canClick = true;
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
            if (Input.GetMouseButtonDown(0))
            {
                FirstClickToTile();
            }

            if (Input.GetMouseButton(0))
            {
                if (UpdateDrawableTiles()) return;
                UpdateLineRenderers();
                UpdateLinkedGridsBall();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_firstLinkedTile is null) return;
                if (!_canClick) return;
                Debug.Log("hi");
                if (_matchCoroutine is null)
                    _matchCoroutine = StartCoroutine(CheckMatchCondition());
            }
        }


        private IEnumerator CheckMatchCondition()
        {
            _lastBombTile = null;
            _bombBall = null;
            if (_linkedTiles.Count > 0)
            {
                _canClick = false;
            }

            if (_linkedTiles.Count >= 3)
            {
                foreach (var tile in _linkedTiles)
                {
                    tile.UpdateLineRendererPosition(tile.ItemSnapPoint.position);
                }

                CheckMatchedBasketBallsIsGoal();
                foreach (var tile in _linkedTiles)
                {
                    tile.ActiveBall?.DisableWithScale();
                    tile.ActiveBall?.Release(Vector3.zero);
                    tile.SetActiveBall(null);
                    yield return new WaitForSeconds(0.1f);
                }


                if (_linkedTiles.Count >= matchedBasketsBombValue)
                {
                    _lastBombTile = _linkedTiles.Last();
                    _bombBall = GridManager.Instance.SpawnBombBall(_lastBombTile);

                    _linkedTiles.Remove(_lastBombTile);
                }

                DeleteLinkedTiles();
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
            DeleteLinkedTiles();
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

                // UIManager.Instance.SetMoveCountText();
            }
        }

        private void CheckMatchedBasketBallsIsGoal()
        {
            // TODO Buraya Goal Match Olunca olması gereken aksiyonlar yazilacak.!!!

            SetComboTexts(_linkedTiles.Count);
            bool haveMatched = false;
            var goals = GridManager.Instance.ActiveLevelData.goals;

            var selected =
                goals.FirstOrDefault(x => x.ballType == _linkedTiles.FirstOrDefault()!.ActiveBall.ballType)!;
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
                    if (!_lastLinkedTile && _lastLinkedTile == tile) return true;
                    if (_linkedTiles.Contains(tile))
                    {
                        ReverseLinkedTiles(tile);
                        return true;
                    }

                    if (!CheckIsSameBoxType(tile, _lastLinkedTile)) return true;
                    if (!CheckAreNeighbours(tile, _lastLinkedTile)) return true;
                    tile.ItemSelected();
                    _linkedTiles.Add(tile);
                    _lastLinkedTile = tile;
                }
            }

            return false;
        }

        private async void SetComboTexts(int index)
        {
            if (index < 5)
                return;
            var comboText = UIManager.Instance.ComboText;
            comboText.transform.localScale = Vector3.zero;
            comboText.gameObject.SetActive(true);
            if (index > 7)
                UIManager.Instance.SetComboText("PERFECT");
            else if (index > 5)
                UIManager.Instance.SetComboText("FASCINATING");
            comboText.transform.DOScale(Vector3.one, .5f);
            await DelayAsync(1000);
            comboText.transform.DOScale(Vector3.zero, .5f).OnComplete(() => comboText.gameObject.SetActive(false));
        }


        private void UpdateLinkedGridsBall()
        {
            int linkedTilesCount = _linkedTiles.Count;
            if (linkedTilesCount == 0) return;
            for (int i = 0; i < linkedTilesCount; i++)
            {
                _linkedTiles[i].ItemSelected();
            }
        }

        private void FirstClickToTile()
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(_ray, out _hit, 100f, drawLayer)) return;
            if (!_hit.collider.TryGetComponent(out Tile tile)) return;
            _firstLinkedTile = tile;
            _lastLinkedTile = tile;
            if (_linkedTiles.Contains(_firstLinkedTile)) return;
            _linkedTiles.Add(_firstLinkedTile);
            if (_firstLinkedTile.ActiveBall)
            {
                _firstLinkedTile.ItemSelected();
            }
        }

        private void DeleteLinkedTiles()
        {
            _firstLinkedTile = null;
            _lastLinkedTile = null;
            for (int i = 0; i < _linkedTiles.Count; i++)
            {
                _linkedTiles[i].UpdateLineRendererPosition(_linkedTiles[i].ItemSnapPoint.position);
                _linkedTiles[i].ActiveBall?.Release(Vector3.zero);
            }

            _linkedTiles.Clear();
        }

        private void ReverseLinkedTiles(Tile tile)
        {
            int heldTileIndex = _linkedTiles.IndexOf(tile);
            int linkedTilesCount = _linkedTiles.Count;
            int removeCount = (linkedTilesCount - 1) - heldTileIndex;
            for (int i = 0; i < removeCount; i++)
            {
                if (_linkedTiles.Count <= 0)
                    break;
                tile = _linkedTiles[^1];
                tile.ActiveBall.Release(Vector3.zero);
                tile.UpdateLineRendererPosition(tile.ItemSnapPoint.position);
                _linkedTiles.Remove(tile);
            }

            _lastLinkedTile = _linkedTiles[heldTileIndex];
            var drawPos = _linkedTiles[^1].ItemSnapPoint.position;
            var pos = new Vector3(drawPos.x, 0f, drawPos.z);
            _linkedTiles[^1].UpdateLineRendererPosition(pos);
        }

        private void UpdateLastLinkedTile(Tile tile, bool isWithVisualUpdate = true)
        {
            _lastLinkedTile = tile;

            if (isWithVisualUpdate)
            {
                _lastLinkedTile.ItemHoldingUpdate(true);
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
            int linkedTilesCount = _linkedTiles.Count;
            if (linkedTilesCount == 0) return;
            for (int i = 0; i < linkedTilesCount; i++)
            {
                if (i != linkedTilesCount - 1)
                {
                    var drawPos = _linkedTiles[i + 1].ItemSnapPoint.position;
                    var pos = new Vector3(drawPos.x, 0f, drawPos.z);
                    _linkedTiles[i].UpdateLineRendererPosition(pos);
                }
            }
        }
    }
}