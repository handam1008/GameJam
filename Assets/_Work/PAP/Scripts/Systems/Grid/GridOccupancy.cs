using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 런타임에 "어느 칸을 누가 쓰고 있는지" 기록하는 아주 단순한 예약 장부.
/// 타일맵(벽)은 TilemapGrid가, 움직이는 대상끼리의 겹침은 여기가 담당한다.
///
/// 이동을 "시작"할 때 목적지를 미리 예약(Reserve)하고, 도착한 뒤에 출발지를 해제(Release)한다.
/// 그래서 이동 중인 칸에 다른 적이 끼어드는 일이 생기지 않는다.
/// </summary>
public static class GridOccupancy
{
    private static readonly Dictionary<Vector3Int, Component> _map = new Dictionary<Vector3Int, Component>();

    // 씬 재시작 / 도메인 리로드 비활성화 상황에서 장부가 남아있지 않도록 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => _map.Clear();

    public static bool IsOccupied(Vector3Int cell)
    {
        if (!_map.TryGetValue(cell, out Component c)) return false;
        if (c == null) { _map.Remove(cell); return false; }   // 파괴된 오브젝트가 남긴 찌꺼기 정리
        return true;
    }

    public static Component GetOwner(Vector3Int cell)
    {
        _map.TryGetValue(cell, out Component c);
        return c;
    }

    /// <summary>칸을 예약한다. 이미 자기 자신이 예약한 칸이면 true.</summary>
    public static bool Reserve(Component owner, Vector3Int cell)
    {
        if (_map.TryGetValue(cell, out Component cur))
        {
            if (cur == null) { _map[cell] = owner; return true; }
            return cur == owner;
        }
        _map[cell] = owner;
        return true;
    }

    public static void Release(Vector3Int cell, Component owner)
    {
        if (_map.TryGetValue(cell, out Component cur) && (cur == owner || cur == null))
            _map.Remove(cell);
    }

    public static void ReleaseAll(Component owner)
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach (KeyValuePair<Vector3Int, Component> kv in _map)
        {
            if (kv.Value == owner || kv.Value == null) toRemove.Add(kv.Key);
        }
        for (int i = 0; i < toRemove.Count; i++) _map.Remove(toRemove[i]);
    }

    public static void Clear() => _map.Clear();
}