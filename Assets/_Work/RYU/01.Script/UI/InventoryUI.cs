using UnityEngine;
using UnityEngine.UI;

// Q/E 슬롯의 아이콘을 인벤토리 상태에 맞춰 갱신한다.
// 슬롯이 바뀔 때마다(OnChanged) 아이콘을 새 아이템 그림으로 바꾸고,
// 비면 아이콘을 숨긴다.
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ItemInventory inventory;

    // 각 슬롯의 Icon 이미지 (Canvas/Q/Icon, Canvas/E/Icon)
    [SerializeField] private Image qIcon;
    [SerializeField] private Image eIcon;

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnChanged -= Refresh;
    }

    private void Refresh()
    {
        if (inventory == null)
            return;

        Show(qIcon, inventory.QItem);
        Show(eIcon, inventory.EItem);
    }

    // 아이템이 있으면 그 아이콘을 보여주고, 없으면 숨긴다
    private void Show(Image icon, AbstractItem item)
    {
        if (icon == null)
            return;

        if (item != null && item.icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
    }
}
