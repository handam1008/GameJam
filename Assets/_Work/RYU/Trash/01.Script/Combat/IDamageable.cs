using Vector3 = UnityEngine.Vector3;

namespace RYU.Combat
{
    public interface IDamageable
    {
        void TakeDamage(Vector3 dir);
    }
}
