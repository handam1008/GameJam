using System;
using System.Collections.Generic;
using _Work.PAP.Scripts.Agent;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.PAP.Scripts.Enemy
{
    public abstract class AbstractEnemy : MonoBehaviour
    {
        [field:SerializeField] public AgentMovement target { get; private set; }
        [Header("Detect")]
        [SerializeField] private float detectRange = 6f;
        [field:SerializeField] public float loseRange{ get; private set; } = 10f;
        [field:SerializeField] public float memoryDuration { get; private set; }= 2.5f;
        [SerializeField] private bool requireLineOfSight = true;
        [Header("Patrol")]
        [field:SerializeField] public bool patrolWhenIdle { get; private set; } = true;
        [field:SerializeField] public float patrolInterval { get; private set; } = 1.5f;
        [SerializeField] private float patrolStraightness = 0.7f;
        [Header("PathFinder")]
        [field:SerializeField] public int maxPathNodes { get; private set; } = 2000;
        [field:SerializeField] public List<AttackPatternSO> AttackPatterns { get; private set; }

        public AgentMovement AgentMovement { get; private set; }
        public bool CanStateChangeable { get; private set; } = true;
        public bool IsDead { get; private set; }
        public bool CanAttack { get; private set; } = true;

        protected virtual void Awake()
        {
            AgentMovement = GetComponentInChildren<AgentMovement>();
        }
        
        public bool CanSeePlayer()
        {
            if (target == null) return false;

            int dist = GridNav.Manhattan(AgentMovement._currentCell, target._currentCell);
            if (dist > detectRange) return false;
            if (!requireLineOfSight) return true;

            return GridNav.HasLineOfSight(AgentMovement.map, AgentMovement._currentCell, target._currentCell);
        }
        
        public bool TryPickChaseStep(out Vector2Int best)
        {
            best = Vector2Int.zero;

            if (!GridNav.TryGetDistance(AgentMovement._currentCell, out int myDist)) return false;   // 거리장 범위 밖

            int bestDist = myDist;
            for (int i = 0; i < GridNav.Dirs4.Length; i++)
            {
                Vector2Int d = GridNav.Dirs4[i];
                Vector3Int c = AgentMovement._currentCell + new Vector3Int(d.x, d.y, 0);

                if (!CanEnter(c)) continue;                                    // 벽 / 다른 적 / 플레이어
                if (!GridNav.TryGetDistance(c, out int nd)) continue;
                if (nd >= bestDist) continue;                                  // 제자리걸음·후퇴 방지

                bestDist = nd;
                best = d;
            }
            return best != Vector2Int.zero;
        }
        
        private bool CanEnter(Vector3Int cell)
        {
            if (!AgentMovement.map.IsWalkable(cell)) return false;
            if (target != null && cell == target._currentCell) return false;   // 플레이어를 통과하지 않는다
            if (GridOccupancy.IsOccupied(cell)) return false;
            return true;
        }
        
        public void Patrol()
        {
            Vector3Int aheadCell = AgentMovement._currentCell + new Vector3Int(AgentMovement._facing.x, AgentMovement._facing.y, 0);
            if (AgentMovement._facing != Vector2Int.zero && Random.value < patrolStraightness && CanEnter(aheadCell))
            {
                AgentMovement.SetMoveDirection(AgentMovement._facing);
                return;
            }

            int start = Random.Range(0, GridNav.Dirs4.Length);
            for (int i = 0; i < GridNav.Dirs4.Length; i++)
            {
                Vector2Int d = GridNav.Dirs4[(start + i) % GridNav.Dirs4.Length];
                Vector3Int c = AgentMovement._currentCell + new Vector3Int(d.x, d.y, 0);
                if (!CanEnter(c)) continue;

                AgentMovement.SetMoveDirection(d);
                return;
            }

            AgentMovement.SetMoveDirection(Vector2Int.zero);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position,loseRange);
        }
    }
}