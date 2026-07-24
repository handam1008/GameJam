using DG.Tweening;
using Unity.Cinemachine;
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(collision.gameObject);
    }
}
