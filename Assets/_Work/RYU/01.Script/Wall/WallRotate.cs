using UnityEngine;
using UnityEngine.InputSystem;

public class WallRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 360f;

    [SerializeField] private float smoothTime = 0.08f;

    [SerializeField] private Transform rotateTarget;

    /// <summary>부호 있는 각속도(도/초). 양수면 반시계, 음수면 시계 방향으로 돈다.</summary>
    public float AngularSpeed { get; private set; }

    private Camera cam;
    private float angleVelocity;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 mouse = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector2 dir = mouse - transform.position;
        float target = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float angle = Mathf.SmoothDampAngle(rotateTarget.eulerAngles.z, target, ref angleVelocity, smoothTime, rotateSpeed);
        AngularSpeed = angleVelocity;
        rotateTarget.rotation = Quaternion.Euler(0f, 0f, angle);
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
