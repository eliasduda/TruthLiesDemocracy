using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    public string eventName;
    public string eventDescription;
    public Sprite eventImage;

    public EventCategory category;

    public EventEffect[] PerPupilEffects;

    public EventEffect[] GeneralEffects;

    public EventEffectPair cost;
    public EventEffectPair duration;

    public float coolDown;

    public bool occupiesYou = false;

    public EventUnlockCondition unlockCondition;

    public bool CanAfford()
    {
        return cost != null && GameManager.instance.timeMoneyManager.CanAfford(cost.amount);
    }
    public bool CanAffordUnlock()
    {
        return unlockCondition.cost != null && GameManager.instance.timeMoneyManager.CanAfford(unlockCondition.cost.amount);
    }

    public bool IsTimedEvent()
    {
        return duration != null && duration.amount > 0;
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

    public bool AppliedInRadius()
    {
        bool f = false;
        foreach(EventEffect ee in PerPupilEffects)
        {
            if (ee.pupilSelector == PupilSelector.InMyRadius) f = true;
        }
        return f;
    }

    public string GetCostDescription(bool canAfford)
    {
        string costStr = "";
        if (cost != null && Mathf.Abs(cost.amount) > 0)
        {
            Color c = canAfford ? Color.white : Color.red;
            string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(c); ;
            costStr += "\nCost: \n";
            costStr += $"<color=#{hex}>{cost.amount}</color>\n"; 
        }
        if (duration != null && duration.amount > 0)
        {
            costStr += "\nTakes: \n";
            costStr += duration.amount / GameManager.instance.gamePlaySettings.dayDurationSeconds + " Days \n";
        }
        return costStr;
    }

}

[System.Serializable]
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
