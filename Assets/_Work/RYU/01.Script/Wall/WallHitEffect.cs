using UnityEngine;
using UnityEngine.InputSystem;

public class WallHitEffect : MonoBehaviour
{
    // 켜두면 마우스 왼클릭한 자리에 효과가 뜬다. 탄 나오면 끄면 됨
    [SerializeField] private bool debugClick = true;

    // WallReveal 머티리얼이 붙은 스프라이트 (Barrier)를 여기에 넣는다
    [SerializeField] private SpriteRenderer barrier;

    // 다 사라지는 데 걸리는 시간(초)
    [SerializeField] private float fadeTime = 0.6f;

    private Material mat;
    private float strength;

    private void Awake()
    {
        // 머티리얼을 복제해서 쓴다. 안 그러면 벽 여러 개가 같은 효과를 공유한다
        mat = barrier.material;
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
    }

    // 반사 성공한 위치를 넘기면 그 주위가 밝아진다. 반사 코드에서 이걸 불러주면 된다
    public void ShowHit(Vector2 worldPos)
    {
        strength = 1f;
        mat.SetVector("_HitPos", worldPos);
        mat.SetFloat("_Strength", 1f);
    }

    // 탄이 벽 콜라이더에 부딪히면 자동으로도 뜬다
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ShowHit(collision.GetContact(0).point);
    }
}
