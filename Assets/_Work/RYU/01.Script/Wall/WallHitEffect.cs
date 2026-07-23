using UnityEngine;
using UnityEngine.InputSystem;

public class WallHitEffect : MonoBehaviour
{
    // 켜두면 마우스 왼클릭한 방향의 벽에 효과가 뜬다. 탄 나오면 끄면 됨
    [SerializeField] private bool debugClick = true;

    // WallReveal 머티리얼이 붙은 스프라이트 (Barrier)
    [SerializeField] private SpriteRenderer barrier;

    // 회색 바닥 사각형. 이 네 모서리를 기준으로 벽을 계산한다
    [SerializeField] private Transform floor;

    // 바닥에서 천장까지의 화면상 높이
    [SerializeField] private float wallHeight = 2f;

    // 다 사라지는 데 걸리는 시간(초)
    [SerializeField] private float fadeTime = 0.6f;

    private Material mat;
    private float strength;

    // 맞은 자리를 "몇 번 벽의 어디쯤(u, v)"으로 기억한다. 바닥이 돌면 효과도 같이 돈다
    private int hitEdge;
    private float hitU;
    private float hitV;

    private readonly Vector2[] corners = new Vector2[4];

    private void Awake()
    {
        mat = barrier.material;

        if (floor == null)
            floor = transform;
    }

    private void Update()
    {
        if (debugClick && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            ShowHit(mouse);
        }

        if (strength <= 0f)
            return;

        // 시간이 지날수록 서서히 걷힌다
        strength -= Time.deltaTime / fadeTime;
        mat.SetFloat("_Strength", Mathf.Max(0f, strength));

        // 바닥이 도는 중이어도 효과가 그 벽 그 자리에 붙어 있도록 매번 다시 계산한다
        ApplyToMaterial();
    }

    // 반사 성공한 위치를 넘기면 그 방향의 벽 위가 밝아진다. 반사 코드에서 이걸 불러주면 된다
    public void ShowHit(Vector2 worldPos)
    {
        FindWallPoint(worldPos, out hitEdge, out hitU, out hitV);
        strength = 1f;
        mat.SetFloat("_Strength", 1f);
        ApplyToMaterial();
    }

    // 기억해 둔 (벽 번호, u, v)를 지금 바닥 위치 기준의 월드 좌표로 바꿔 셰이더에 넘긴다
    private void ApplyToMaterial()
    {
        ReadCorners();

        Vector2 p1 = corners[hitEdge];
        Vector2 edge = corners[(hitEdge + 1) % 4] - p1;
        Vector2 up = Vector2.up * wallHeight;

        Vector2 point = p1 + edge * hitU + up * hitV;

        mat.SetVector("_HitPos", point);
        mat.SetVector("_WallDir", edge.normalized);
    }

    // 마우스가 가리키는 곳에서 가장 가까운 벽면과 그 위의 (u, v)를 찾는다
    // 벽면: 바닥 모서리 p1→p2 를 위로 wallHeight 만큼 올린 평행사변형
    // 점 m 을 m = p1 + u*(p2-p1) + v*up 으로 풀고, u v 를 0~1로 잘라 벽 안에 가둔다
    private void FindWallPoint(Vector2 m, out int edge, out float u, out float v)
    {
        ReadCorners();

        Vector2 up = Vector2.up * wallHeight;
        float best = float.MaxValue;
        edge = 0;
        u = 0f;
        v = 0f;

        for (int i = 0; i < 4; i++)
        {
            Vector2 p1 = corners[i];
            Vector2 e = corners[(i + 1) % 4] - p1;

            // 2x2 연립방정식. det가 0이면 벽이 화면에서 선으로 보이는 각도라 건너뛴다
            float det = e.x * up.y - e.y * up.x;
            if (Mathf.Abs(det) < 0.0001f)
                continue;

            Vector2 d = m - p1;
            float cu = Mathf.Clamp01((d.x * up.y - d.y * up.x) / det);
            float cv = Mathf.Clamp01((e.x * d.y - e.y * d.x) / det);

            // 잘라낸 지점이 마우스와 가장 가까운 벽을 고른다
            Vector2 p = p1 + e * cu + up * cv;
            float dist = (p - m).sqrMagnitude;

            if (dist < best)
            {
                best = dist;
                edge = i;
                u = cu;
                v = cv;
            }
        }
    }

    // 바닥 스프라이트(1x1 기준)의 네 모서리를 월드 좌표로 읽는다. 회전과 크기가 반영된다
    private void ReadCorners()
    {
        corners[0] = floor.TransformPoint(new Vector3(-0.5f, -0.5f, 0f));
        corners[1] = floor.TransformPoint(new Vector3(0.5f, -0.5f, 0f));
        corners[2] = floor.TransformPoint(new Vector3(0.5f, 0.5f, 0f));
        corners[3] = floor.TransformPoint(new Vector3(-0.5f, 0.5f, 0f));
    }
}
