using System;
using DG.Tweening;
using UnityEngine;

public class DropFall : MonoBehaviour
{
    // 내려오는 데 걸리는 시간. 비행기의 Marker Hide Delay와 맞추면 표시와 착지가 일치한다
    [SerializeField] private float fallTime = 1.5f;

    // 착지 지점보다 얼마나 위에서 시작할지 (화면 기준 위쪽)
    [SerializeField] private float fallHeight = 6f;

    // 시작할 때 몇 배 크기로 떠 있을지
    [SerializeField] private float startScale = 2f;

    // 착지 전까지 꺼둘 것들. 적 AI, 콜라이더 등을 넣으면 공중에 있는 동안 얌전하다
    [SerializeField] private Behaviour[] enableOnLand;

    // 착지하면 켤 오브젝트 (상자 등)
    [SerializeField] private GameObject[] showOnLand;

    // 착지하면 끌 오브젝트 (낙하산 등)
    [SerializeField] private GameObject[] hideOnLand;

    // 착지하면 이 자리에 소환할 프리팹들 (적 등). 적 종류마다 낙하산 안 만들어도 됨
    [SerializeField] private GameObject[] spawnOnLand;

    // 소환 후 낙하산(자기 자신)을 지울지. 적 배달용이면 켠다
    [SerializeField] private bool destroySelfAfterSpawn = false;

    [SerializeField] private float startAlpha = 0.2f;

    private Vector3 groundScale;
    private Vector3 landingLocal;
    private float t;
    private bool landed;

    [SerializeField] private SpriteRenderer sr;
    
    private void Start()
    {
        foreach (Behaviour b in enableOnLand)
            b.enabled = false;

        // 착지 후에 켤 것들은 떨어지는 동안 숨겨둔다
        foreach (GameObject go in showOnLand)
            go.SetActive(false);

        groundScale = transform.localScale;

        // 착지 지점을 바닥 기준으로 기억한다. 맵이 돌아도 마커와 같은 자리에 내린다
        landingLocal = transform.localPosition;
    }

    private void Update()
    {
        if (landed)
            return;

        t += Time.deltaTime / fallTime;
        float k = Mathf.Clamp01(t);

        // 아래로 갈수록 빨라지는 낙하 곡선
        float ease = k * k * k;

        // 지금 이 순간의 착지 지점 (바닥이 돌아간 것 반영)
        Vector3 ground = transform.parent != null
            ? transform.parent.TransformPoint(landingLocal)
            : landingLocal;

        // 위에서 착지점으로 내려오면서 크기도 같이 줄인다
        transform.position = ground + Vector3.up * (fallHeight * (1f - ease));
        transform.localScale = Vector3.Lerp(groundScale * startScale, groundScale, ease);

        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(startAlpha, 1f, ease);
            sr.color = c;
        }

        if (k >= 1f)
            Land();
    }

    private void Land()
    {
        landed = true;

        foreach (Behaviour b in enableOnLand)
            b.enabled = true;

        // 상자는 켜고 낙하산은 끈다
        foreach (GameObject go in showOnLand)
            go.SetActive(true);

        foreach (GameObject go in hideOnLand)
            go.SetActive(false);

        // 후보 중 하나만 랜덤으로 뽑아 착지 자리에 소환한다. 같은 부모(맵)에 붙여 같이 움직이게 한다
        if (spawnOnLand != null && spawnOnLand.Length > 0)
        {
            GameObject prefab = spawnOnLand[UnityEngine.Random.Range(0, spawnOnLand.Length)];
            if (prefab != null)
            {
                GameObject spawned = Instantiate(prefab, transform.position, Quaternion.identity);
                if (transform.parent != null)
                    spawned.transform.SetParent(transform.parent, true);
            }
        }

        // 배달만 하는 낙하산이면 소환 후 자기 자신을 지운다
        if (destroySelfAfterSpawn)
        {
            Destroy(gameObject);
            return;
        }

        // 착지 순간 살짝 눌렸다 펴지는 반동
        transform.DOPunchScale(Vector3.one * -0.15f, 0.25f, 6)
            .SetLink(gameObject);
    }
}
