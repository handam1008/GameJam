using _Work.RYU._01.Script.Item;
using _Work.RYU._01.Script.Player;
using UnityEngine;
using UnityEngine.UI;

// Q/E 슬롯의 아이콘을 인벤토리 상태에 맞춰 갱신한다.
// 분노 아이템은 발동 중(체력 1)이면 빨간 아이콘으로 바뀐다.
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ItemInventory inventory;
    [SerializeField] private LastStand lastStand;

    // 각 슬롯의 Icon 이미지 (Canvas/Q/Icon, Canvas/E/Icon)
    [SerializeField] private Image qIcon;
    [SerializeField] private Image eIcon;

    // 각 슬롯의 쿨타임/지속시간 표시 (Image Type = Filled). 효과 진행도만큼 채워진다
    [SerializeField] private Image qFill;
    [SerializeField] private Image eFill;

    // 아이템 사용 시 살짝 흔들 슬롯. 각 슬롯 루트에 UIShake를 붙여 연결
    [SerializeField] private UIShake qShake;
    [SerializeField] private UIShake eShake;

    private void Awake()
    {
        if (lastStand == null)
            lastStand = FindAnyObjectByType<LastStand>();
    }

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnChanged += Refresh;
            inventory.OnItemUsed += OnUsed;
        }

        // 분노 발동/해제 시에도 아이콘을 다시 그린다
        if (lastStand != null)
            lastStand.OnRageChanged += OnRage;

        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnChanged -= Refresh;
            inventory.OnItemUsed -= OnUsed;
        }

        if (lastStand != null)
            lastStand.OnRageChanged -= OnRage;
    }

    // 아이템 쓴 슬롯을 살짝 흔든다
    private void OnUsed(bool isQ)
    {
        UIShake shake = isQ ? qShake : eShake;
        if (shake != null)
            shake.Pulse();
    }

    private void OnRage(bool raging) => Refresh();

    // 쿨타임/지속시간은 매 프레임 변하니 폴링해서 채운다
    private void Update()
    {
        if (inventory == null)
            return;

        SetFill(qFill, inventory.QItem);
        SetFill(eFill, inventory.EItem);
    }

    private void SetFill(Image fill, AbstractItem item)
    {
        if (fill == null)
            return;

        float ratio = item != null ? item.CooldownRatio01(inventory.User) : 0f;
        fill.fillAmount = ratio;
    }

    private void Refresh()
    {
        if (inventory == null)
            return;

        Show(qIcon, inventory.QItem);
        Show(eIcon, inventory.EItem);
    }

    private void Show(Image icon, AbstractItem item)
    {
        if (icon == null)
            return;

        if (item == null)
        {
            icon.enabled = false;
            return;
        }

        // 분노 아이템이고 발동 중이면 빨간 아이콘, 아니면 평소 아이콘
        Sprite sprite = item.icon;
        if (item is AngerItem anger && lastStand != null && lastStand.IsRaging && anger.rageIcon != null)
            sprite = anger.rageIcon;

        if (sprite != null)
        {
            icon.sprite = sprite;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
    }
}
