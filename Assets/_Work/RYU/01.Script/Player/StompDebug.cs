using UnityEngine;

// 임시 진단용. Player에 붙여서 주변에 뭐가 있는지 매초 찍는다
public class StompDebug : MonoBehaviour
{
    private float t;

    private void Update()
    {
        t += Time.deltaTime;
        if (t < 1f) return;
        t = 0f;

        // 내 위치 반경 2 안의 모든 콜라이더를 훑는다
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);

        Collider2D enemy = GameObject.FindWithTag("Enemy")?.GetComponent<Collider2D>();
        float dist = enemy != null
            ? Vector2.Distance(transform.position, enemy.transform.position)
            : -1f;

        string near = "";
        foreach (var h in hits)
            near += h.name + "(" + LayerMask.LayerToName(h.gameObject.layer) + ") ";

        Debug.Log($"[Debug] 내위치={transform.position} 적까지거리={dist:F2} 반경2안={hits.Length}개: {near}");
    }
}
