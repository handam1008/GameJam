using Systems;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] float speed = 20f;
    [SerializeField] float maxSpeed = 60f;
    [SerializeField] float spinBonus = 0.05f;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] private int maxBounces = 2;

    Vector2 velocity;
    CircleCollider2D myCol;
    private int currentBounces = 0;

    void Awake() => myCol = GetComponent<CircleCollider2D>();

    public void Init(Vector2 direction)
    {
        velocity = direction.normalized * speed;
        SetRotation();
    }

    void Update()
    {
        float r = myCol.radius * Mathf.Abs(transform.lossyScale.x);
        Collider2D wall = Physics2D.OverlapCircle(transform.position, r, wallLayer);

        if (wall != null)
        {
            ColliderDistance2D d = myCol.Distance(wall);
            Vector2 n = -d.normal;

            float pushOut = Mathf.Min(Mathf.Abs(d.distance) + 0.05f, r + 0.1f);
            transform.position += (Vector3)(n * pushOut);
            Bounce(n, wall, transform.position);
        }
        else
        {
            float move = velocity.magnitude * Time.deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity.normalized, move + 0.05f, wallLayer);

            if (hit.collider != null)
            {
                transform.position = hit.point + hit.normal * 0.05f;
                Bounce(hit.normal, hit.collider, hit.point);
            }
        }

        if (this == null) return; 
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }
    
    void Bounce(Vector2 normal, Collider2D wall, Vector2 contactPoint)
    {
        if (Vector2.Dot(velocity, normal) >= 0f)
            return;

        if (++currentBounces > maxBounces)
        {
            PoolManager.Instance.Push(this);
            return;
        }

        Vector2 reflected = Vector2.Reflect(velocity, normal);

        WallRotate rotate = wall.GetComponentInParent<WallRotate>();
        if (rotate != null)
            reflected += rotate.GetPointVelocity(contactPoint) * spinBonus;

        velocity = Vector2.ClampMagnitude(reflected, maxSpeed);

        SetRotation();
    }

    void SetRotation()
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    [field:SerializeField] public PoolItemSO Item { get; private set; }
    public GameObject GameObject => gameObject;
    public void ResetItem()
    {
        currentBounces = 0;
    }
}
