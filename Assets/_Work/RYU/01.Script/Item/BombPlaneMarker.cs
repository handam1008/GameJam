using UnityEngine;

// 폭격 비행기 식별용. BombItem이 씬에서 이걸 찾아 폭격을 부른다.
// 이 컴포넌트를 SupplyPlane과 같은 오브젝트에 붙인다.
[RequireComponent(typeof(SupplyPlane))]
public class BombPlaneMarker : MonoBehaviour
{
    private SupplyPlane plane;

    private void Awake()
    {
        plane = GetComponent<SupplyPlane>();
    }

    // 폭격 시작. 비행기가 랜덤 지점들을 지나며 폭격물을 떨어뜨린다
    public void StartBombing()
    {
        plane.CallPlane();
    }
}
