using System;
using _Work.PAP.Scripts.Systems;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements.Experimental;

namespace _Work.PAP.Scripts.Agent
{
    public class AgentMovement : MonoBehaviour
    {
        [field:SerializeField] public TilemapGrid map {get; private set;}
        [SerializeField] private float moveDuration;
        [SerializeField] private Ease moveEase;

        public bool _isMoving { get; private set; }
        private bool _initialized;
        private Vector2Int _moveDir;
        public Vector2Int _facing {get; private set;}
        public Vector3Int _currentCell { get; private set; }

        public event Action BlockEvent;

        private void Awake()
        {
            GameManager.Instance.OnMoveEvent += TryStep;
        }

        private void Start()
        {
            _currentCell = map.WorldToCell(transform.position);
            if (!map.IsWalkable(_currentCell))
            {
                if (map.TryFindNearestWalkable(_currentCell, 8, out Vector3Int fixedCell))
                {
                    _currentCell = fixedCell;
                }
            }
            transform.position = map.CellToWorld(_currentCell);
            GridOccupancy.Reserve(this,_currentCell);
            _initialized = true;
        }
        
        private void OnEnable()
        {
            if (_initialized && map != null) GridOccupancy.Reserve(this, _currentCell);
        }

        private void OnDisable() => GridOccupancy.ReleaseAll(this);
        private void OnDestroy()
        {
            GridOccupancy.ReleaseAll(this);
            GameManager.Instance.OnMoveEvent -= TryStep;
        }

        public void TryStep()
        {
            if (_isMoving || _moveDir == Vector2Int.zero || map == null) return;
            if (_moveDir.x != 0 && _moveDir.y != 0) return;

            _facing = _moveDir;
            //플립시키기

            Vector3Int target = _currentCell + new Vector3Int(_moveDir.x, _moveDir.y, 0);

            if (!map.IsWalkable(target)) return;

            if (!GridOccupancy.Reserve(this, target))
            {
                BlockEvent?.Invoke();
                return;
            }

            Vector3Int prevCell = _currentCell;
            _currentCell = target;

            transform.DOMove(map.CellToWorld(target), moveDuration).OnComplete(() =>
            {
                _isMoving = false;
                _moveDir = Vector2Int.zero;
            }).SetEase(moveEase);
            GridOccupancy.Release(prevCell, this);
            _isMoving = true;
            return;
        }

        public void SetMoveDirection(Vector2Int dir)
        {
            _moveDir = dir;
        }
    }
}