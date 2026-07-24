using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInventory : MonoBehaviour
{
    // 아이템 효과를 받을 대상. 비우면 이 오브젝트 자신
    [SerializeField] private GameObject user;

    private AbstractItem qItem;
    private AbstractItem eItem;

    public AbstractItem QItem => qItem;
    public AbstractItem EItem => eItem;

    // 슬롯이 바뀔 때 발행. UI 갱신용
    public event Action OnChanged;

    private void Awake()
    {
        if (user == null)
            user = gameObject;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return;

        if (kb.qKey.wasPressedThisFrame)
        {
            Debug.Log($"[Inv] Q 눌림. Q슬롯={(qItem != null ? qItem.name : "비어있음")}", this);
            Use(ref qItem);
        }

        if (kb.eKey.wasPressedThisFrame)
        {
            Debug.Log($"[Inv] E 눌림. E슬롯={(eItem != null ? eItem.name : "비어있음")}", this);
            Use(ref eItem);
        }
    }

    // 주울 때 부른다. Q 먼저, 차 있으면 E. 둘 다 차 있으면 false
    public bool TryPickUp(AbstractItem item)
    {
        if (item == null)
            return false;

        if (qItem == null)
            qItem = item;
        else if (eItem == null)
            eItem = item;
        else
            return false;

        Debug.Log($"[Inv] 주움: {item.name} → Q={(qItem != null ? qItem.name : "-")}, E={(eItem != null ? eItem.name : "-")}", this);
        OnChanged?.Invoke();
        return true;
    }

    private void Use(ref AbstractItem slot)
    {
        if (slot == null)
            return;

        slot.Use(user);
        slot = null;
        OnChanged?.Invoke();
    }
}
