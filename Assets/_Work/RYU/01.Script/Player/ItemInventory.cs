using System;
using System.Collections;
using _Work.RYU._01.Script.Item;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInventory : MonoBehaviour
{
    // 아이템 효과를 받을 대상. 비우면 이 오브젝트 자신
    [SerializeField] private GameObject user;

    // 증폭 2번째 발동까지의 간격(초)
    [SerializeField] private float amplifyDelay = 0.5f;

    private AbstractItem qItem;
    private AbstractItem eItem;

    // 각 슬롯 아이템을 이미 썼는지 (KeepAfterUse 아이템의 효과 종료 판단용)
    private bool qUsed;
    private bool eUsed;

    public AbstractItem QItem => qItem;
    public AbstractItem EItem => eItem;
    public GameObject User => user;

    // 슬롯이 바뀔 때 발행. UI 갱신용
    public event Action OnChanged;

    // 아이템을 쓰는 순간 발행(true=Q, false=E). UI 흔들기용
    public event Action<bool> OnItemUsed;

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
            UseSlot(true);

        if (kb.eKey.wasPressedThisFrame)
            UseSlot(false);

        // 슬롯에 남겨둔(KeepAfterUse) 아이템은 효과가 끝나면 비운다
        ClearIfExpired(ref qItem, ref qUsed);
        ClearIfExpired(ref eItem, ref eUsed);
    }

    // isQ면 Q슬롯 사용. 반대편에 증폭이 있으면 2번 발동하고 증폭을 소진한다
    private void UseSlot(bool isQ)
    {
        AbstractItem slot = isQ ? qItem : eItem;
        if (slot == null)
            return;

        // 증폭이나 패시브는 자기 키로 발동 안 된다
        if (slot is AmplifyItem || slot.IsPassive)
            return;

        // 첫 발동
        slot.Use(user);
        slot.PlayUseSound();
        OnItemUsed?.Invoke(isQ);

        // 반대편에 증폭이 있으면 0.5초 뒤 한 번 더 발동하고 증폭을 소진한다
        AbstractItem other = isQ ? eItem : qItem;
        if (other is AmplifyItem)
        {
            StartCoroutine(UseAgainAfter(slot, amplifyDelay, isQ));

            if (isQ) eItem = null;
            else qItem = null;
        }

        // 사용 슬롯 처리 (지속형이면 남김)
        if (slot.KeepAfterUse(user))
        {
            if (isQ) qUsed = true; else eUsed = true;
        }
        else
        {
            if (isQ) qItem = null; else eItem = null;
        }

        OnChanged?.Invoke();
    }

    // 증폭의 두 번째 발동. 간격을 두고 한 번 더 Use
    private IEnumerator UseAgainAfter(AbstractItem item, float delay, bool isQ)
    {
        yield return new WaitForSeconds(delay);
        if (item != null)
        {
            item.Use(user);
            item.PlayUseSound();
            OnItemUsed?.Invoke(isQ);
        }
    }

    private void ClearIfExpired(ref AbstractItem slot, ref bool used)
    {
        if (slot == null || !used)
            return;

        // 썼고, 효과가 완전히 끝나면 슬롯 비움 (진행도 0이 아니라 IsFinished로 판단)
        if (slot.IsFinished(user))
        {
            slot = null;
            used = false;
            OnChanged?.Invoke();
        }
    }

    // 주울 때 부른다. Q 먼저, 차 있으면 E. 둘 다 차 있으면 false
    public bool TryPickUp(AbstractItem item)
    {
        if (item == null)
            return false;

        if (qItem == null)
        {
            qItem = item;
            qUsed = false;
        }
        else if (eItem == null)
        {
            eItem = item;
            eUsed = false;
        }
        else
            return false;

        // 패시브(분노 등)는 슬롯에 뜨되, 줍는 즉시 발동한다
        if (item.IsPassive)
            item.Use(user);

        OnChanged?.Invoke();
        return true;
    }

}
