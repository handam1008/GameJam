using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 유니티 Tilemap을 읽어서 "이 칸으로 갈 수 있는가"를 판정한다.
/// Grid 오브젝트에 붙여서 사용. 콜라이더/물리를 전혀 쓰지 않는다.
///
/// 판정 순서 (하나라도 걸리면 이동 불가):
///   1) Ground Tilemap에 타일이 없다            -> 불가  (Ground를 비워두면 이 검사 생략)
///   2) Blocking Tilemaps 중 하나에 타일이 있다  -> 불가
///   3) Ground에 그려진 타일이 Blocking Tiles 목록에 있다 -> 불가
/// </summary>
[DefaultExecutionOrder(-50)]
public class TilemapGrid : MonoBehaviour
{
    public static TilemapGrid Instance { get; private set; }

    [Header("타일맵 방식 A — 바닥 / 벽 타일맵을 분리")]
    [Tooltip("여기에 타일이 그려진 칸만 이동 가능. 비워두면 바닥 검사를 하지 않는다.")]
    [SerializeField] private Tilemap groundTilemap;

    [Tooltip("여기에 타일이 하나라도 그려져 있으면 이동 불가. (벽, 장애물 타일맵)")]
    [SerializeField] private Tilemap[] blockingTilemaps;

    [Header("타일맵 방식 B — 타일맵 하나에 다 그리고 벽 타일만 지정")]
    [Tooltip("Ground Tilemap에 그린 타일 중 '벽'으로 취급할 타일 에셋들.")]
    [SerializeField] private TileBase[] blockingTiles;

    [Header("옵션")]
    [Tooltip("계산된 셀 중심에 더할 보정값. 타일 앵커가 기본이 아닐 때만 건드리면 된다.")]
    [SerializeField] private Vector3 worldOffset = Vector3.zero;

    [Tooltip("씬 뷰에서 이동 가능/불가 칸을 색으로 표시 (이 오브젝트를 선택했을 때).")]
    [SerializeField] private bool drawDebugCells = true;

    private Grid _grid;
    private HashSet<TileBase> _blockingSet;

    /// <summary>셀 <-> 월드 좌표 변환을 담당하는 Grid 컴포넌트.</summary>
    public Grid Layout
    {
        get
        {
            if (_grid == null) _grid = GetComponent<Grid>();
            if (_grid == null && groundTilemap != null) _grid = groundTilemap.layoutGrid;
            if (_grid == null && blockingTilemaps != null)
            {
                for (int i = 0; i < blockingTilemaps.Length; i++)
                {
                    if (blockingTilemaps[i] == null) continue;
                    _grid = blockingTilemaps[i].layoutGrid;
                    break;
                }
            }
            return _grid;
        }
    }

    private void Awake()
    {
        Instance = this;
        if (Layout == null)
            Debug.LogError("[TilemapGrid] Grid 컴포넌트를 찾을 수 없습니다. 이 스크립트를 Grid 오브젝트에 붙이세요.", this);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ---------------- 핵심 판정 ----------------

    public bool IsWalkable(Vector3Int cell)
    {
        // 1) 바닥이 있는가
        if (groundTilemap != null && !groundTilemap.HasTile(cell)) return false;

        // 2) 벽 타일맵에 걸리는가
        if (blockingTilemaps != null)
        {
            for (int i = 0; i < blockingTilemaps.Length; i++)
            {
                Tilemap tm = blockingTilemaps[i];
                if (tm != null && tm.HasTile(cell)) return false;
            }
        }

        // 3) 벽으로 지정된 타일 에셋인가
        if (blockingTiles != null && blockingTiles.Length > 0 && groundTilemap != null)
        {
            if (_blockingSet == null) RebuildBlockingSet();

            TileBase here = groundTilemap.GetTile(cell);
            if (here != null && _blockingSet.Contains(here)) return false;
        }

        return true;
    }

    private void OnValidate()
    {
        _blockingSet = null;   // 인스펙터에서 목록을 바꾸면 다음 조회 때 다시 만든다
        _grid = null;
    }

    private void RebuildBlockingSet()
    {
        _blockingSet = new HashSet<TileBase>();
        for (int i = 0; i < blockingTiles.Length; i++)
            if (blockingTiles[i] != null) _blockingSet.Add(blockingTiles[i]);
    }

    // ---------------- 좌표 변환 ----------------

    /// <summary>셀의 중심 월드 좌표. 셀 크기·아이소메트릭·헥사 레이아웃 모두 Grid가 알아서 처리한다.</summary>
    public Vector3 CellToWorld(Vector3Int cell)
    {
        Grid g = Layout;
        if (g == null) return new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f) + worldOffset;
        return g.GetCellCenterWorld(cell) + worldOffset;
    }

    public Vector3Int WorldToCell(Vector3 world)
    {
        Grid g = Layout;
        if (g == null) return new Vector3Int(Mathf.FloorToInt(world.x), Mathf.FloorToInt(world.y), 0);
        return g.WorldToCell(world - worldOffset);
    }

    /// <summary>origin이 벽이면 주변에서 가장 가까운 이동 가능 칸을 찾는다. (시작 위치 보정용)</summary>
    public bool TryFindNearestWalkable(Vector3Int origin, int maxRadius, out Vector3Int result)
    {
        result = origin;
        if (IsWalkable(origin)) return true;

        for (int r = 1; r <= maxRadius; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != r) continue;   // 링 테두리만
                    Vector3Int c = new Vector3Int(origin.x + dx, origin.y + dy, origin.z);
                    if (IsWalkable(c)) { result = c; return true; }
                }
            }
        }
        return false;
    }

    // ---------------- 씬 뷰 디버그 ----------------

    private void OnDrawGizmosSelected()
    {
        if (!drawDebugCells) return;

        Grid g = Layout;
        if (g == null) return;

        Tilemap reference = groundTilemap;
        if (reference == null && blockingTilemaps != null)
        {
            for (int i = 0; i < blockingTilemaps.Length; i++)
                if (blockingTilemaps[i] != null) { reference = blockingTilemaps[i]; break; }
        }
        if (reference == null) return;

        BoundsInt b = reference.cellBounds;
        if ((long)b.size.x * b.size.y > 20000) return;   // 맵이 너무 크면 기즈모 생략

        Vector3 cs = g.cellSize;
        Vector3 boxSize = new Vector3(cs.x * 0.82f, cs.y * 0.82f, 0.01f);

        for (int x = b.xMin; x < b.xMax; x++)
        {
            for (int y = b.yMin; y < b.yMax; y++)
            {
                Vector3Int c = new Vector3Int(x, y, 0);

                // 바닥 자체가 없는 칸(맵 바깥)은 표시하지 않는다
                if (groundTilemap != null && !groundTilemap.HasTile(c)) continue;

                Gizmos.color = IsWalkable(c)
                    ? new Color(0.2f, 1f, 0.45f, 0.22f)
                    : new Color(1f, 0.25f, 0.25f, 0.38f);

                Gizmos.DrawCube(CellToWorld(c), boxSize);
            }
        }
    }
}