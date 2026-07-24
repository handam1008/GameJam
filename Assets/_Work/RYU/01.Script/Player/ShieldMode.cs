using DG.Tweening;
using UnityEngine;

public class ShieldMode : MonoBehaviour
{
   
    [SerializeField] private GameObject shieldVisual;
    
    [SerializeField] private float popTime = 0.25f;
    
    [SerializeField] private float pulse = 0.08f;

    public bool IsShielded { get; private set; }

    private Vector3 normalScale;
    private float timer;

    private void Awake()
    {
        normalScale = shieldVisual.transform.localScale;
        shieldVisual.SetActive(false);
    }

    private void Update()
    {
        if (!IsShielded)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f)
            return;

       
        IsShielded = false;
        shieldVisual.transform.DOKill();
        shieldVisual.transform.DOScale(Vector3.zero, popTime)
            .SetEase(Ease.InBack)
            .OnComplete(() => shieldVisual.SetActive(false))
            .SetLink(shieldVisual);
    }

    public void Activate(float duration)
    {
        timer = duration;

       
        if (IsShielded)
            return;

        IsShielded = true;
        shieldVisual.SetActive(true);

        
        shieldVisual.transform.localScale = Vector3.zero;
        shieldVisual.transform.DOScale(normalScale, popTime)
            .SetEase(Ease.OutBack)
            .OnComplete(StartPulse)
            .SetLink(shieldVisual);
    }

    private void StartPulse()
    {
        if (pulse <= 0f)
            return;

        shieldVisual.transform.DOScale(normalScale * (1f + pulse), 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(shieldVisual);
    }
}
