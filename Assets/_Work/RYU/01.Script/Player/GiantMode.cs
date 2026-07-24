using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GiantMode : MonoBehaviour
{
    
    [SerializeField] private Transform visual;
    [SerializeField] private float growTime = 0.3f;

    public bool IsGiant { get; private set; }

    private Vector3 normalScale;
    private float timer;

    private void Awake()
    {
        if (visual == null)
            visual = transform;
        
        normalScale = visual.localScale;
    }

    private void Update()
    {
        if (!IsGiant)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f)
            return;
        
        IsGiant = false;
        visual.DOScale(normalScale, growTime).SetEase(Ease.InBack).SetLink(visual.gameObject);
    }

    public void Activate(float duration, float scale)
    {
        timer = duration;
        
        if (IsGiant)
            return;

        IsGiant = true;
        visual.DOScale(normalScale * scale, growTime).SetEase(Ease.OutBack).SetLink(visual.gameObject);
    }

    // 콜라이더든 트리거든 둘 다 받는다. 적 콜라이더 설정을 안 타서 안전하다
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStomp(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStomp(collision.gameObject);
    }

    private void TryStomp(GameObject target)
    {
        Debug.Log($"[Giant] 닿음: {target.name}, 거인={IsGiant}, 태그={target.tag}", this);

        if (!IsGiant)
            return;

        if (!target.CompareTag("Enemy"))
            return;

        Destroy(target);
    }
}
