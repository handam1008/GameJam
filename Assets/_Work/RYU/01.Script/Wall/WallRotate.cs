using UnityEngine;
using UnityEngine.InputSystem;

public class WallRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 360f;

    [SerializeField] private float smoothTime = 0.08f;

    [SerializeField] private float mouseSensitivity = 0.5f;

    [SerializeField] private Transform rotateTarget;

    /// <summary>모든 WallRotate에 공통으로 곱해지는 감도 배율. PauseMenu 설정에서 조절한다.</summary>
    public static float SensitivityMultiplier { get; set; }

    public const string SensitivityPrefKey = "MapSensitivity";

    /// <summary>부호 있는 각속도(도/초). 양수면 반시계, 음수면 시계 방향으로 돈다.</summary>
    public float AngularSpeed { get; private set; }

    private float angleVelocity;
    private Rigidbody2D rb;
    private Vector2 previousMouseScreenPos;

    private void Awake()
    {
        rb = rotateTarget.GetComponent<Rigidbody2D>();
        previousMouseScreenPos = Mouse.current.position.ReadValue();
        SensitivityMultiplier = PlayerPrefs.GetFloat(SensitivityPrefKey, 1f);
    }

    private void FixedUpdate()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        float deltaX = mouseScreenPos.x - previousMouseScreenPos.x;
        previousMouseScreenPos = mouseScreenPos;

        // 마우스가 오른쪽으로 움직인 만큼 시계 방향(-), 왼쪽으로 움직인 만큼 반시계 방향(+)으로 회전한다.
        // 마우스가 반대로 움직이면 deltaX 부호가 뒤집혀 자연스럽게 반대편으로 돌아온다.
        float instantAngularSpeed = Mathf.Clamp(-deltaX * mouseSensitivity * SensitivityMultiplier / Time.fixedDeltaTime, -rotateSpeed, rotateSpeed);
        AngularSpeed = Mathf.SmoothDamp(AngularSpeed, instantAngularSpeed, ref angleVelocity, smoothTime);

        float angle = rotateTarget.eulerAngles.z + AngularSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(Quaternion.Euler(0f, 0f, angle));
    }

    /// <summary>
    /// 이 판이 회전하면서 world 좌표 point 지점이 순간적으로 갖게 되는 접선 속도(월드 단위/초).
    /// 총알이 부딪힌 지점이 회전축에서 멀수록, 회전이 빠를수록 커진다.
    /// </summary>
    public Vector2 GetPointVelocity(Vector2 worldPoint)
    {
        Vector2 pivot = rotateTarget != null ? (Vector2)rotateTarget.position : (Vector2)transform.position;
        Vector2 r = worldPoint - pivot;
        float angularSpeedRad = AngularSpeed * Mathf.Deg2Rad;

        // v = ω × r. 2D에서는 r을 90도 회전시킨 방향으로 |ω||r|만큼의 접선 속도가 생긴다.
        return angularSpeedRad * new Vector2(-r.y, r.x);
    }
}
