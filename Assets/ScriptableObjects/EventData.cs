using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    public string eventName;
    public string eventDescription;
    public Sprite eventImage;

    public EventCategory category;

    public EventEffect[] PerPupilEffects;

    public EventEffect[] OneTimeEffects;

    public EventEffectPair cost;
    public EventEffectPair duration;

    public EventUnlockCondition unlockCondition;

    public bool CanAfford()
    {
        return cost != null && GameManager.instance.timeMoneyManager.CanAfford(cost.amount);
    }
    public bool CanAffordUnlock()
    {
        return unlockCondition.cost != null && GameManager.instance.timeMoneyManager.CanAfford(unlockCondition.cost.amount);
    }
}

public class EventInstance
{
    public EventData eventData;
    public float timeRemaining;
    public EventInstance(EventData data)
    {
        eventData = data;
        timeRemaining = data.duration.amount;
    }
}
