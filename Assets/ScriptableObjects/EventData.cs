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

    public bool AffectedSupport()
    {
        foreach(EventEffect effect in PerPupilEffects)
        {
            foreach(EventEffectPair pair in effect.effects)
            {
                if(pair.stat == InfluencableStats.Support && pair.amount > 0) return true;
            }
        }
        return false;
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
