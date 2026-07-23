using UnityEngine;

/// <summary>
/// 칸 위에 잠깐 떴다가 서서히 사라지는 사각형 표식.
/// 적의 공격 예고(텔레그래프)에 쓴다. 스프라이트 에셋을 따로 준비할 필요 없이
/// 1x1 흰색 텍스처를 런타임에 만들어 재사용한다.
/// </summary>
public class GridMarker : MonoBehaviour
{
    private static Sprite _sharedSprite;

    private SpriteRenderer _sr;
    private Color _startColor;
    private float _life;
    private float _age;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => _sharedSprite = null;

    public static GridMarker Show(Vector3 worldPos, Vector2 size, Color color, float life,
                                  int sortingOrder = 100, string sortingLayer = null)
    {
        if (_sharedSprite == null)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();

            // pixelsPerUnit = 1 이라 localScale이 곧 월드 크기가 된다
            _sharedSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            _sharedSprite.name = "GridMarkerSprite";
        }

        GameObject go = new GameObject("GridMarker");
        go.transform.position = new Vector3(worldPos.x, worldPos.y, worldPos.z);
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = _sharedSprite;
        sr.color = color;
        sr.sortingOrder = sortingOrder;
        if (!string.IsNullOrEmpty(sortingLayer)) sr.sortingLayerName = sortingLayer;

        GridMarker m = go.AddComponent<GridMarker>();
        m._sr = sr;
        m._startColor = color;
        m._life = Mathf.Max(0.01f, life);
        return m;
    }

    private void Update()
    {
        _age += Time.deltaTime;
        float t = Mathf.Clamp01(_age / _life);

        if (_sr != null)
        {
            Color c = _startColor;
            c.a = _startColor.a * (1f - t * t);   // 끝에서 빠르게 사라짐
            _sr.color = c;
        }

        if (t >= 1f) Destroy(gameObject);
    }
}