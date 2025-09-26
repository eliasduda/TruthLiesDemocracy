using UnityEngine;

[System.Serializable]
public class EventEffect
{
    public EventEffectPair[] effects;
    public EventSelector[] whereAllOfTheseApply;
    public EventSelector[] andWhereOneOfTheseApply;
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
}
