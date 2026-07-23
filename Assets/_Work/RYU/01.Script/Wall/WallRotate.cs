using UnityEngine;
using UnityEngine.InputSystem;

public class WallRotate : MonoBehaviour
{
    // 초당 최대 회전 각도. 낮출수록 벽이 무겁게 따라온다.
    [SerializeField] private float rotateSpeed = 360f;

    // 따라붙는 부드러움. 클수록 미끄러지듯 늦게 붙고, 0.02면 거의 즉각이다.
    [SerializeField] private float smoothTime = 0.08f;

    [SerializeField] private Transform rotateTarget;

    private Camera cam;
    private float angleVelocity;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // 마우스 화면 좌표를 월드 좌표로 바꾼다
        Vector3 mouse = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // 축에서 마우스를 향하는 방향의 각도
        Vector2 dir = mouse - transform.position;
        float target = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 멀면 빠르게, 가까우면 천천히 붙는다. rotateSpeed보다 빨리 돌지는 못한다.
        float angle = Mathf.SmoothDampAngle(rotateTarget.eulerAngles.z, target, ref angleVelocity, smoothTime, rotateSpeed);
        rotateTarget.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
