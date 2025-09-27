using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public UnityEvent<EventData> onTryBuyEvent = new UnityEvent<EventData>();
    public UnityEvent<InfluencableStats, float> onOneTimeStatInfluenced = new UnityEvent<InfluencableStats, float>();
    public UnityEvent<InfluencableStats, float, Pupil> onPupilStatInfluenced = new UnityEvent<InfluencableStats, float, Pupil>();
    public UnityEvent<InfluencableStats, float> OnStatChanged = new UnityEvent<InfluencableStats, float>();

    public UnityEvent<int, TMPHoverableText, Vector2> onHoverableWordHovered = new UnityEvent<int, TMPHoverableText, Vector2>();
    public UnityEvent<int, EventData, Vector2> onHoverableEventHovered = new UnityEvent<int, EventData, Vector2>();
    public UnityEvent<HoverInfoPopUp> onPopUpUnhovered = new UnityEvent<HoverInfoPopUp>();
    public UnityEvent onRecastVote = new UnityEvent();


    public List<EventInstance> activeTimedEvents = new List<EventInstance>();
    public List<EventInstance> activeInstantEvents = new List<EventInstance>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onTryBuyEvent.AddListener(OnEventBought);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (EventInstance eI in activeTimedEvents)
        {
            float delta = Mathf.Min(eI.timeRemaining, Time.fixedDeltaTime);
            eI.timeRemaining -= delta;
            if (eI.timeRemaining >= 0)
            {
                TriggerEvent(eI.eventData, delta / eI.eventData.duration.amount);
            }
        }
        foreach (EventInstance eI in activeInstantEvents)
        {
            eI.timeRemaining = 0;
            TriggerEvent(eI.eventData, 1);
        }

        activeInstantEvents.Clear();
        for (int i = activeTimedEvents.Count - 1; i >= 0; i--)
        {
            if (activeTimedEvents[i].timeRemaining <= 0)
            {
                onRecastVote.Invoke();
                activeTimedEvents.RemoveAt(i);
            }
        }
    }

    void OnEventBought(EventData eventData)
    {
        if (!eventData.CanAfford())
        {
            Debug.Log("Cannot afford event: " + eventData.eventName);
            return;
        }
        ApplyEffect(eventData.cost, null);
        if(eventData.duration != null && eventData.duration.amount > 0)
        {
            activeTimedEvents.Add(new EventInstance(eventData));
        }
        else
        {
            activeInstantEvents.Add(new EventInstance(eventData));
        }


    }

    void TriggerEvent(EventData eventData, float ratio = 1)
    {

        foreach (EventEffect effect in eventData.PerPupilEffects)
        {
            foreach (Pupil pupil in GameManager.instance.pupilManager.pupils)
            {
                if(effect.pupilSelector == PupilSelector.InMyRadius &&
                    !GameManager.instance.pupilManager.you.IsInMyRadius(pupil))
                {
                    continue;
                }
                if(effect.pupilSelector == PupilSelector.InMyRadius) Debug.Log("pupil " + pupil.name+ " is in radius");
                TriggerEventEffectPerPupil(effect, pupil, ratio);
            }
        }

        foreach (EventEffect effect in eventData.OneTimeEffects)
        {
            TriggerEventEffectOneTime(effect);
        }
    }

    private void TriggerEventEffectPerPupil(EventEffect effect, Pupil pupil, float ratio)
    {
        if(pupil == null) Debug.LogError("Pupil is null in TriggerEventEffectPerPupil");
        TriggerEffect(effect, pupil, ratio);
    }
    private void TriggerEventEffectOneTime(EventEffect effect)
    {
        TriggerEffect(effect, null);
    }

    private void TriggerEffect(EventEffect effect, Pupil pupil, float ratio = 1)
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
        //Debug.Log("TriggerEffect on " + (pupil != null ? pupil.name : "Game") + " allConditionsMet: " + allConditionsMet + " wanted to chane "+ effect.effects[0].stat);
        // If all conditions are met, apply the effects
        if (allConditionsMet)
        {

            foreach (var effectPair in effect.effects)
            {
                ApplyEffect(effectPair, pupil, ratio);
            }
        }
    }

    private bool ApplyEffect(EventEffectPair effect, Pupil pupil, float ratio = 1)
    {
        if (PupilStats.IsPerPupilStat(effect.stat))
        {
            if(pupil == null)
            {
                Debug.LogError("Trying to apply per-pupil stat effect without a pupil context"); 
                return false;
            }
            onPupilStatInfluenced.Invoke(effect.stat, effect.amount * ratio, pupil);
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
            case InfluencableStats.Support:
                return condition.Compare(pupil.stats.trust);
            case InfluencableStats.Trust:
                return condition.Compare(pupil.stats.support);
            case InfluencableStats.Awareness:
                return condition.Compare(pupil.stats.isAware);
            //aplied to game
            case InfluencableStats.MoneyPerDay:
                return condition.Compare(GameManager.instance.timeMoneyManager.MoneyPerDay);
            case InfluencableStats.MoneyTotal:
                return condition.Compare(GameManager.instance.timeMoneyManager.MoneyTotal);
            case InfluencableStats.DaysPassed:
                return condition.Compare(GameManager.instance.timeMoneyManager.DaysPassed);

            default:
                Debug.LogError("Unknown InfluencableStat in CheckCondition "+condition.stat);
                return false;
        }
    }
}
