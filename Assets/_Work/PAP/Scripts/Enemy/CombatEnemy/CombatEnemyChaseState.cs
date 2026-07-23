using System.Collections.Generic;
using _Work.PAP.Scripts.Enemy.Enemies;
using _Work.PAP.Scripts.Enemy.FSM;
using _Work.PAP.Scripts.Systems;
using Enemies.FSM;
using UnityEngine;

namespace _Work.PAP.Scripts.Enemy.CombatEnemy
{
    public class CombatEnemyChaseState : EnemyState
    {
        private static readonly Color TelegraphColor = new Color(1f, 0.85f, 0.2f, 0.5f);
        private const float StrikeFlashDuration = 0.15f;

        private float _lastSeenTime;
        private float _lastAttackTime;
        private AttackPatternSO currentAttack;

        private readonly List<Vector3Int> _pendingCells = new List<Vector3Int>();
        private bool _hasPendingAttack;

        public CombatEnemyChaseState(AbstractEnemy enemy, EnemyStateMachine stateMachine, int animationHash) : base(enemy, stateMachine, animationHash)
        {
        }

        public override void Enter()
        {
            currentAttack = PickRandomAttack();
            _hasPendingAttack = false;
            _pendingCells.Clear();
            GameManager.Instance.OnMoveEvent += OnBeat;
        }

        public override void Exit()
        {
            GameManager.Instance.OnMoveEvent -= OnBeat;
        }

        public override void UpdateState()
        {
            if (_hasPendingAttack)
            {
                // 이미 시전(예고)된 공격은 플레이어가 도망가거나 시야를 벗어나도 끝까지 이어간다.
                // 실제 타격 판정은 OnBeat()에서 처리하고, 여기서는 제자리에 멈춰서 기다린다.
                _enemy.AgentMovement.SetMoveDirection(Vector2Int.zero);
                return;
            }

            if (_enemy.target == null) { _stateMachine.ChangeState((int)CombatEnemyState.Idle); return; }

            bool visible = _enemy.CanSeePlayer();
            if (visible) _lastSeenTime = Time.time;

            int dist = GridNav.Manhattan(_enemy.AgentMovement._currentCell, _enemy.target._currentCell);

            // 놓친 지 오래됐거나 너무 멀어지면 포기
            bool forgotten = Time.time - _lastSeenTime > _enemy.memoryDuration;
            if (forgotten || (!visible && dist > _enemy.loseRange))
            {
                _stateMachine.ChangeState((int)CombatEnemyState.Idle);
                return;
            }

            Vector3Int cellDirection = _enemy.target._currentCell - _enemy.AgentMovement._currentCell;
            bool aligned = cellDirection.x == 0 || cellDirection.y == 0;
            bool inAttackStance = currentAttack != null && dist <= currentAttack.range && aligned;

            if (inAttackStance)
            {
                // 공격 실행은 OnBeat()에서 GameManager 박자에 맞춰 처리한다. 여기서는 제자리에 멈추기만 한다.
                _enemy.AgentMovement.SetMoveDirection(Vector2Int.zero);
                return;
            }

            GridNav.BuildField(_enemy.AgentMovement.map, _enemy.target._currentCell, _enemy.maxPathNodes);

            if (_enemy.TryPickChaseStep(out Vector2Int dir)) _enemy.AgentMovement.SetMoveDirection(dir);
            else _enemy.AgentMovement.SetMoveDirection(Vector2Int.zero);
        }

        /// <summary>
        /// GameManager의 박자(OnMoveEvent)마다 한 단계씩 진행한다.
        /// 한 틱: 공격 범위를 예고(텔레그래프)만 하고, 다음 틱: 그 범위를 그대로 타격한다.
        /// </summary>
        private void OnBeat()
        {
            if (_hasPendingAttack)
            {
                Strike();
                return;
            }

            if (currentAttack == null || _enemy.target == null) return;
            if (_enemy.AgentMovement._isMoving) return;
            if (Time.time < _lastAttackTime) return;

            int dist = GridNav.Manhattan(_enemy.AgentMovement._currentCell, _enemy.target._currentCell);
            Vector3Int cellDirection = _enemy.target._currentCell - _enemy.AgentMovement._currentCell;
            bool aligned = cellDirection.x == 0 || cellDirection.y == 0;
            if (dist > currentAttack.range || !aligned) return;

            Telegraph(cellDirection);
        }

        /// <summary>공격 범위를 계산해 예고 마커로 보여주고, 다음 틱에 그대로 타격할 셀들을 확정한다.</summary>
        private void Telegraph(Vector3Int cellDirection)
        {
            if (currentAttack.patternSize % 2 == 0) Debug.LogError("currentAttack 범위가 홀수여야합니다!!");

            Vector2Int facing = ToCardinal(cellDirection);
            Vector3Int enemyCell = _enemy.AgentMovement._currentCell;
            Vector3Int targetCell = _enemy.target._currentCell;

            _pendingCells.Clear();
            for (int i = 0; i < currentAttack.patternSize; i++)
            {
                for (int j = 0; j < currentAttack.patternSize; j++)
                {
                    if (!currentAttack.GetCell(i, j)) continue;

                    Vector2Int offset = currentAttack.GetRotatedOffset(i, j, facing);
                    Vector3Int cell = currentAttack.primary == primaryTarget.owner
                        ? enemyCell + new Vector3Int(offset.x, offset.y, 0)
                        : targetCell + new Vector3Int(offset.x, offset.y, 0);

                    _pendingCells.Add(cell);
                    GridMarker.Show(_enemy.AgentMovement.map.CellToWorld(cell), new Vector2(0.85f, 0.85f), TelegraphColor, currentAttack.duration, 100);
                }
            }

            _hasPendingAttack = true;
        }

        /// <summary>예고했던 셀들을 그대로 타격한다. 다음 공격까지의 쿨다운은 여기서부터 계산된다.</summary>
        private void Strike()
        {
            _hasPendingAttack = false;
            _lastAttackTime = Time.time + currentAttack.postDelay;

            for (int i = 0; i < _pendingCells.Count; i++)
            {
                Vector3 worldPos = _enemy.AgentMovement.map.CellToWorld(_pendingCells[i]);
                GridMarker.Show(worldPos, new Vector2(0.85f, 0.85f), Color.red, StrikeFlashDuration, 100);
            }

            currentAttack = PickRandomAttack();
        }

        private AttackPatternSO PickRandomAttack()
        {
            return _enemy.AttackPatterns is { Count: > 0 }
                ? _enemy.AttackPatterns[Random.Range(0, _enemy.AttackPatterns.Count)]
                : null;
        }

        /// <summary>정렬된(같은 행/열) 셀 방향 벡터를 4방향 단위 벡터로 바꾼다.</summary>
        private static Vector2Int ToCardinal(Vector3Int cellDirection)
        {
            int x = cellDirection.x == 0 ? 0 : (cellDirection.x > 0 ? 1 : -1);
            int y = cellDirection.y == 0 ? 0 : (cellDirection.y > 0 ? 1 : -1);
            return x != 0 ? new Vector2Int(x, 0) : new Vector2Int(0, y == 0 ? 1 : y);
        }
    }
}