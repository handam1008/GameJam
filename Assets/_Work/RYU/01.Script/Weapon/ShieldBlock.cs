using _Work.PAP.Scripts.Agent;
using UnityEngine;
using UnityEngine.InputSystem;

// 좌클릭을 누르는 동안 방패를 든다. 마우스 방향 플레이어 앞에 방패가 서고,
// 방패 정면 총알을 튕긴다. 든 동안 이동속도가 느려진다.
public class ShieldBlock : MonoBehaviour
{
    [Header("막기")]
    // 방패 감지 반경
    [SerializeField] private float blockRadius = 1.5f;
    // 정면 각도(도). 180이면 앞쪽 절반 다 막는다
    [SerializeField] private float blockAngle = 180f;

    [Header("게이지")]
    // 총 사용 가능 시간(초). 누르면 줄고 0이 되면 방패가 사라진다
    [SerializeField] private float maxGauge = 6f;
    // 내린 뒤 다시 들기까지 쿨타임(초)
    [SerializeField] private float recoolTime = 1f;

    [Header("속도")]
    // 방패 든 동안 곱해질 이동속도 배율 (0.5 = 절반)
    [SerializeField] private float slowFactor = 0.5f;

    [Header("비주얼")]
    // 방패 네모 스프라이트. 든 동안 켜져 마우스 방향 앞에 선다
    [SerializeField] private GameObject shieldVisual;
    // 플레이어에서 방패까지 거리
    [SerializeField] private float shieldDistance = 1f;
    // 스프라이트가 원래 바라보는 각도 보정. 그림이 위(세로)를 보고 있으면 -90
    [SerializeField] private float spriteAngleOffset = -90f;

    public bool IsBlocking { get; private set; }

    // 방패 무기가 장착됐는지. 장착 안 됐으면 좌클릭해도 방패 안 나온다
    private bool equipped;

    private Vector2 facingDir = Vector2.right;
    private AgentMovement movement;

    private float gauge;      // 남은 사용 시간
    private float cooldown;   // 남은 재사용 쿨타임

    private void Awake()
    {
        movement = GetComponent<AgentMovement>();
        gauge = maxGauge;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    // 방패 무기 장착/해제. WeaponHolder가 부른다
    public void SetEquipped(bool value)
    {
        equipped = value;
        gauge = maxGauge;   // 장착할 때 게이지 채움

        // 장착 해제 시 방패 들고 있었으면 내린다
        if (!equipped && IsBlocking)
            EndBlock();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (cooldown > 0f)
            cooldown -= Time.deltaTime;

        // 방패 무기가 장착됐고, 게이지 남고, 쿨타임 아닐 때만 들 수 있다
        bool canBlock = equipped && gauge > 0f && cooldown <= 0f;
        bool held = Mouse.current.leftButton.isPressed && canBlock;

        // 누르기 시작 / 떼기
        if (held && !IsBlocking)
            StartBlock();
        else if (!held && IsBlocking)
            EndBlock();

        if (IsBlocking)
        {
            // 든 동안 게이지가 줄고, 다 쓰면 강제로 내린다
            gauge -= Time.deltaTime;
            if (gauge <= 0f)
            {
                gauge = 0f;
                EndBlock();
                return;
            }

            UpdateShield();
            BlockBullets();
        }
    }

    private void StartBlock()
    {
        IsBlocking = true;

        if (movement != null)
            movement.SetSpeedMultiplier(slowFactor);

        if (shieldVisual != null)
            shieldVisual.SetActive(true);
    }

    private void EndBlock()
    {
        IsBlocking = false;
        cooldown = recoolTime;   // 내리면 재사용 쿨타임 시작

        if (movement != null)
            movement.SetSpeedMultiplier(1f);

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    // 마우스 방향을 정면으로, 방패를 플레이어 앞에 그 방향으로 세운다
    private void UpdateShield()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        facingDir = ((Vector2)(mouse - transform.position)).normalized;

        if (shieldVisual == null)
            return;

        // 위치: 플레이어 앞 (마우스 방향)
        shieldVisual.transform.position = transform.position + (Vector3)(facingDir * shieldDistance);

        // 회전: 마우스 방향을 바라봄
        // 스프라이트 기본 방향만큼 보정해서 마우스 쪽을 바라보게 한다
        float deg = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;
        shieldVisual.transform.rotation = Quaternion.Euler(0f, 0f, deg + spriteAngleOffset);
    }

    private void BlockBullets()
    {
        // 반경 안 콜라이더를 다 훑고 Bullet만 골라 튕긴다 (반사 포탑과 같은 방식)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, blockRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out Bullet bullet))
                continue;

            // 총알이 정면 각도 안에 있을 때만 튕긴다
            Vector2 toBullet = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(facingDir, toBullet);

            if (angle > blockAngle * 0.5f)
                continue;

            bullet.Reflect();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, blockRadius);
    }
}
