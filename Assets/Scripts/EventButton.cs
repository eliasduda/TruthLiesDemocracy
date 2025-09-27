using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public TextMeshProUGUI buttonTitle;
    public EventData triggeringEvent;

    public bool isUnlocked = false;
    [System.NonSerialized]
    public bool canAfford = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonTitle.text = triggeringEvent.eventName;
        button.onClick.AddListener(OnTryUnlock);

        GameManager.instance.eventManager.OnStatChanged.AddListener(MoneyUpdated);

        if (isUnlocked) OnLockStateChanged(true);
        bool affrd = isUnlocked ? triggeringEvent.CanAfford() : triggeringEvent.CanAffordUnlock();
        OnCanAffordChanged(affrd);
    }

    private void MoneyUpdated(InfluencableStats stat, float amount)
    {
        if(stat == InfluencableStats.MoneyTotal)
        {
            bool affrd = isUnlocked? triggeringEvent.CanAfford() : triggeringEvent.CanAffordUnlock();
            if (affrd != canAfford) OnCanAffordChanged(affrd);
        }
    }

    void OnTryUnlock()
    {
        if (canAfford && !isUnlocked)
        {
            button.onClick.RemoveListener(OnTryUnlock);
            button.onClick.AddListener(OnTriggerEvent);
            OnLockStateChanged(true);
            bool affrd = triggeringEvent.CanAfford();
            if(affrd != canAfford) OnCanAffordChanged(affrd);
        }
    }

    void OnTriggerEvent()
    {
        if (canAfford && isUnlocked)
        {
            GameManager.instance.eventManager.onTryBuyEvent.Invoke(triggeringEvent);
        }
    }

    void OnLockStateChanged(bool isUnlocked)
    {
        this.isUnlocked = isUnlocked;
        button.enabled = isUnlocked && canAfford;
    }

    void OnCanAffordChanged(bool canAfford)
    {
        this.canAfford = canAfford;
        button.enabled = isUnlocked && canAfford;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instance.eventManager.onHoverableEventHovered.Invoke(canAfford && isUnlocked? 0 : isUnlocked ? 1 : 2, triggeringEvent, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

}
