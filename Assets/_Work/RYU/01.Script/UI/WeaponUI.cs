using UnityEngine;
using UnityEngine.UI;

// 장착한 무기를 UI에 표시한다. UI는 항상 떠 있다.
// 큰 네모박스(Filled Image)가 남은 사용량(0~1)에 비례해 줄어든다.
public class WeaponUI : MonoBehaviour
{
    [SerializeField] private WeaponHolder holder;

    // 무기 아이콘. 무기 없으면 꺼진다
    [SerializeField] private Image icon;

    // 사용량에 따라 줄어드는 큰 네모박스. Image Type을 Filled로 설정
    [SerializeField] private Image box;

    private void Update()
    {
        if (holder == null)
            return;

        bool hasWeapon = holder.HasWeapon;

        // 무기 있을 때만 아이콘 켜고 갱신 (Q/E 슬롯처럼)
        if (icon != null)
        {
            icon.enabled = hasWeapon;
            if (hasWeapon && holder.Weapon.icon != null)
                icon.sprite = holder.Weapon.icon;
        }

        // 남은 비율만큼 채운다. 무기 없으면 0
        if (box != null)
            box.fillAmount = hasWeapon ? Mathf.Clamp01(holder.RemainingRatio) : 0f;
    }
}
