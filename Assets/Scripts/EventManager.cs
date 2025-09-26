using System;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public UnityEvent<EventData> onTryEventTrigger = new UnityEvent<EventData>();
    public UnityEvent<InfluencableStats, float> onOneTimeStatInfluenced = new UnityEvent<InfluencableStats, float>();
    public UnityEvent<InfluencableStats, float, Pupil> onPupilStatInfluenced = new UnityEvent<InfluencableStats, float, Pupil>();
    public UnityEvent<InfluencableStats, float> OnStatChanged = new UnityEvent<InfluencableStats, float>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onTryEventTrigger.AddListener(OnEventTriggerd);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEventTriggerd(EventData eventData)
    {
        foreach (EventEffect effect in eventData.PerPupilEffects)
        {
            foreach (var pupil in GameManager.instance.pupilManager.pupils)
            {
                TriggerEventEffectPerPupil(effect, pupil);
            }
        }

        foreach (EventEffect effect in eventData.OneTimeEffects)
        {
            TriggerEventEffectOneTime(effect);
        }

    }
    private void TriggerEventEffectPerPupil(EventEffect effect, Pupil pupil)
    {
        if(pupil == null) Debug.LogError("Pupil is null in TriggerEventEffectPerPupil");
        TriggerEffect(effect, pupil);
    }
    private void TriggerEventEffectOneTime(EventEffect effect)
    {
        TriggerEffect(effect, null);
    }

    private void TriggerEffect(EventEffect effect, Pupil pupil)
    {
        bool allConditionsMet = true;
        // Check all "whereAllOfTheseApply" conditions
        foreach (var condition in effect.whereAllOfTheseApply)
        {
            if (!CheckCondition(condition, pupil))
            {
                allConditionsMet = false;
                break;
            }
        }
        // Check "andWhereOneOfTheseApply" conditions if all previous conditions are met
        if (allConditionsMet && effect.andWhereOneOfTheseApply.Length > 0)
        {
            bool oneConditionMet = false;
            if (effect.andWhereOneOfTheseApply.Length == 0) oneConditionMet = true;
            foreach (var condition in effect.andWhereOneOfTheseApply)
            {
                if (CheckCondition(condition, pupil))
                {
                    oneConditionMet = true;
                    break;
                }
            }
            allConditionsMet = oneConditionMet;
        }
        // If all conditions are met, apply the effects
        if (allConditionsMet)
        {
            foreach (var effectPair in effect.effects)
            {
                ApplyEffect(effectPair, pupil);
            }
        }
    }

    private bool ApplyEffect(EventEffectPair effect, Pupil pupil)
    {
        if (PupilStats.IsPerPupilStat(effect.stat))
        {
            if(pupil == null)
            {
                Debug.LogError("Trying to apply per-pupil stat effect without a pupil context"); 
                return false;
            }
            onPupilStatInfluenced.Invoke(effect.stat, effect.amount, pupil);
        }
        else
        {
            onOneTimeStatInfluenced.Invoke(effect.stat, effect.amount);
        }
        return true;
    }

    bool CheckCondition(EventSelector condition, Pupil pupil)
    {
        if(PupilStats.IsPerPupilStat(condition.stat) && pupil == null)
        {
            Debug.LogError("Trying to check per-pupil stat condition without a pupil context"); return false;
        }

        switch(condition.stat)
        {
            //aplied to student
            case InfluencableStats.Alignment:
                return condition.Compare(pupil.stats.alignment);
            case InfluencableStats.Engagement:
                return condition.Compare(pupil.stats.engagement);
            //aplied to game
            case InfluencableStats.MoneyPerDay:
                return condition.Compare(GameManager.instance.timeMoneyManager.MoneyPerDay);
            case InfluencableStats.MoneyTotal:
                return condition.Compare(GameManager.instance.timeMoneyManager.MoneyTotal);
            case InfluencableStats.DaysPassed:
                return condition.Compare(GameManager.instance.timeMoneyManager.DaysPassed);

            default:
                Debug.LogError("Unknown InfluencableStat in CheckCondition");
                return false;
        }
    }
}
