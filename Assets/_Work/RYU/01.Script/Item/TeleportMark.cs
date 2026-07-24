using UnityEngine;

// 순간이동 표식. 플레이어가 남긴 자리에 월드 고정으로 서 있다가,
// 쿨타임이 지나면 플레이어를 이 자리로 강제 귀환시키고 사라진다.
public class TeleportMark : MonoBehaviour
{
    private GameObject target;   // 되돌릴 플레이어
    private float timer;
    private float totalCooldown;
    private bool started;   // Init 전엔 카운트도 순간이동도 안 함

    // 쿨타임 진행도 0~1. 0이면 방금 남김, 1이면 곧 순간이동. UI가 읽는다
    public float Progress => totalCooldown > 0f ? 1f - Mathf.Clamp01(timer / totalCooldown) : 0f;

    // 아이템이 표식을 만들 때 호출. 대상과 쿨타임을 넘긴다
    public void Init(GameObject player, float cooldown)
    {
        target = player;
        timer = cooldown;
        totalCooldown = cooldown;
        started = true;
    }

    private void Update()
    {
        // Init이 안 됐으면 아무것도 안 함 (즉시 사라지는 것 방지)
        if (!started)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f)
            return;

        Teleport();
    }

    private void Teleport()
    {
        if (target != null)
        {
            target.transform.position = transform.position;

            // 이동 관성 제거 (미끄러지지 않게)
            if (target.TryGetComponent(out Rigidbody2D rb))
                rb.linearVelocity = Vector2.zero;
        }

        Destroy(gameObject);
    }
}
