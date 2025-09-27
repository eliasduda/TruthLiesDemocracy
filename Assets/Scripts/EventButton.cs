using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Slider cooldownSlider;
    public TextMeshProUGUI buttonTitle;
    public EventData triggeringEvent;

    public bool isUnlocked = false;
    [System.NonSerialized]
    public bool canAfford = false;
    public bool isOnCooldown;

    private float coolDownTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonTitle.text = triggeringEvent.eventName;
        button.onClick.AddListener(OnTryUnlock);

        GameManager.instance.eventManager.OnStatChanged.AddListener(MoneyUpdated);

        if (isUnlocked) OnLockStateChanged(true);
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            coolDownTimer -= Time.deltaTime;
            cooldownSlider.value = 1 - (coolDownTimer / (triggeringEvent.coolDown + triggeringEvent.duration.amount));
            if (coolDownTimer <= 0)
            {
                OnCoolDownChanged(false);
            }
        }
    }

    private void MoneyUpdated(InfluencableStats stat, float amount)
    {
        if(stat == InfluencableStats.MoneyTotal)
        {
            Debug.Log("MoneyUpdated for " + triggeringEvent.eventName + " to "+amount);
            bool affrd = isUnlocked? triggeringEvent.CanAfford() : triggeringEvent.CanAffordUnlock();
            if (affrd != canAfford) OnCanAffordChanged(affrd);
        }
    }

    void OnTryUnlock()
    {
        if (canAfford && !isUnlocked)
        {
            OnLockStateChanged(true);
        }
    }

    void OnTriggerEvent()
    {
        if (canAfford && isUnlocked && !isOnCooldown)
        {
            if (triggeringEvent.IsTimedEvent())
            {
                OnCoolDownChanged(true);
            }
            GameManager.instance.eventManager.onTryBuyEvent.Invoke(triggeringEvent);
        }
    }

    void OnLockStateChanged(bool isUnlocked)
    {
        this.isUnlocked = isUnlocked;
        button.enabled = isUnlocked && canAfford;

        if (isUnlocked)
        {
            button.onClick.RemoveListener(OnTryUnlock);
            button.onClick.AddListener(OnTriggerEvent);
        }

        bool affrd = triggeringEvent.CanAfford();
        if (affrd != canAfford) OnCanAffordChanged(affrd);
    }

    void OnCanAffordChanged(bool canAfford)
    {
        this.canAfford = canAfford;
        button.enabled = isUnlocked && canAfford;
    }
    void OnCoolDownChanged(bool isOnCooldown)
    {
        coolDownTimer = triggeringEvent.coolDown + triggeringEvent.duration.amount;
        this.isOnCooldown = isOnCooldown;
        cooldownSlider.enabled = isOnCooldown;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //GameManager.instance.eventManager.onHoverableEventHovered.Invoke(canAfford && isUnlocked? 0 : isUnlocked ? 1 : 2, triggeringEvent, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

}
