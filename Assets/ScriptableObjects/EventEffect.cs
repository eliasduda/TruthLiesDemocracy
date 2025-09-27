using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class EventEffect
{
    public EventEffectPair[] effects;
    public EventSelector[] whereAllOfTheseApply;
    public EventSelector[] andWhereOneOfTheseApply;
    public PupilSelector pupilSelector;
}

[System.Serializable]
public class EventEffectPair
{
    public InfluencableStats stat;
    public float amount;

}

[System.Serializable]
public class EventSelector
{
    public InfluencableStats stat;
    public StatComparator comparator;
    public float amount;

    public bool Compare(float amount)
    {
        switch(comparator)
        {
            case StatComparator.GreaterThan:
                return amount > this.amount;
            case StatComparator.LessThan:
                return amount < this.amount;
            case StatComparator.EqualTo:
                return Mathf.Approximately(amount, this.amount);
            default:
                Debug.LogError("Unknown comparator: " + comparator);
                return false;
        }
    }
}

[System.Serializable]
public class EventUnlockCondition
{
    public EventSelector[] AllOfTheseApply;
    public EventSelector[] andOneOfTheseApply;
    public EventEffectPair cost;
}
