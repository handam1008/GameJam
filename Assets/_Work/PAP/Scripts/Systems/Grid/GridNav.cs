using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 AI용 길찾기 도구.
///
/// 핵심 아이디어: 적마다 각자 길찾기를 돌리지 않는다.
/// 플레이어 칸에서 바깥으로 BFS를 한 번 퍼뜨려 "거리장(distance field)"을 만들어 두고,
/// 모든 적이 그걸 공유해서 "내 이웃 칸 중 거리 숫자가 제일 작은 쪽"으로 한 칸씩 간다.
///
/// 플레이어가 칸을 옮겼을 때만 다시 계산하므로, 적이 50마리든 100마리든
/// 프레임당 BFS는 최대 한 번이다.
/// </summary>
public static class GridNav
{
    public static readonly Vector2Int[] Dirs4 =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
    };

    private static readonly Dictionary<Vector3Int, int> _dist = new Dictionary<Vector3Int, int>();
    private static readonly Queue<Vector3Int> _frontier = new Queue<Vector3Int>();

    private static Vector3Int _builtTarget;
    private static bool _hasField;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _dist.Clear();
        _frontier.Clear();
        _hasField = false;
    }

    /// <summary>타일맵을 런타임에 바꿨다면 호출해서 거리장을 강제로 다시 만들게 한다.</summary>
    public static void Invalidate() => _hasField = false;

    /// <summary>목표 칸이 바뀌었을 때만 거리장을 다시 만든다. 매 프레임 호출해도 안전.</summary>
    public static void BuildField(TilemapGrid map, Vector3Int target, int maxNodes)
    {
        if (map == null) return;
        if (_hasField && _builtTarget == target) return;

        _dist.Clear();
        _frontier.Clear();

        _dist[target] = 0;
        _frontier.Enqueue(target);

        int visited = 0;
        while (_frontier.Count > 0 && visited < maxNodes)
        {
            Vector3Int cur = _frontier.Dequeue();
            visited++;
            int next = _dist[cur] + 1;

            for (int i = 0; i < Dirs4.Length; i++)
            {
                Vector3Int n = cur + new Vector3Int(Dirs4[i].x, Dirs4[i].y, 0);
                if (_dist.ContainsKey(n)) continue;
                if (!map.IsWalkable(n)) continue;      // 벽은 여기서 걸러진다

                _dist[n] = next;
                _frontier.Enqueue(n);
            }
        }

        _builtTarget = target;
        _hasField = true;
    }

    /// <summary>이 칸이 거리장 안에 있으면 목표까지의 걸음 수를 돌려준다.</summary>
    public static bool TryGetDistance(Vector3Int cell, out int distance) => _dist.TryGetValue(cell, out distance);

    public static Vector3Int FieldTarget => _builtTarget;
    public static bool HasField => _hasField;

    // ---------------- 시야 ----------------

    /// <summary>a와 b 사이에 벽이 없는지 검사 (그리드 브레젠험 직선).</summary>
    public static bool HasLineOfSight(TilemapGrid map, Vector3Int a, Vector3Int b)
    {
        if (map == null) return false;

        int x0 = a.x, y0 = a.y;
        int x1 = b.x, y1 = b.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = -Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        // 무한 루프 방지용 상한
        int guard = dx - dy + 4;

        while (guard-- > 0)
        {
            if (x0 == x1 && y0 == y1) return true;

            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }

            // 도착 칸은 검사하지 않는다 (플레이어가 서 있는 칸이므로)
            if (x0 == x1 && y0 == y1) return true;
            if (!map.IsWalkable(new Vector3Int(x0, y0, a.z))) return false;
        }
        return false;
    }

    public static int Manhattan(Vector3Int a, Vector3Int b) =>
        Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
}