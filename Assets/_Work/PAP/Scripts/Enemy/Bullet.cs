using _Work.RYU._01.Script.FeedBack;
using RYU.Combat;
using Systems;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected float maxSpeed = 60f;
    [SerializeField] protected float spinBonus = 0.05f;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected int maxBounces = 2;
    [SerializeField] private FeedBackPlayer player;

    protected Vector2 velocity;
    protected CircleCollider2D myCol;
    protected int currentBounces = 0;
    protected CinemachineImpulseSource _impulseSource;

    protected virtual void Awake()
    {
        myCol = GetComponent<CircleCollider2D>();
        _impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    public void Init(Vector2 direction)
    {
        velocity = direction.normalized * speed;
        SetRotation();
    }

    protected virtual void Update()
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
    
    protected virtual void Bounce(Vector2 normal, Collider2D wall, Vector2 contactPoint)
    {
        if (wall.CompareTag("Going"))
        {
            transform.position = -transform.position + (Vector3)(normal * 0.05f);
            return;
        }
        _impulseSource.GenerateImpulseWithVelocity(velocity * speed/5000f);
        player.PlayAllFeedBack();
        if (wall.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(velocity);
            PoolManager.Instance.Push(this);
            return;
        }
        if (Vector2.Dot(velocity, normal) >= 0f)
            return;
        
        WallHitEffect effect = wall.GetComponentInParent<WallHitEffect>();
        if (effect != null)
        {
            effect.ShowHit(transform.position);
        }

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
