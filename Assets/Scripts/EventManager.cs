using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEventTriggerd(EventData eventData)
    {
        // Apply event effects to game state
        foreach (var effect in eventData.effects)
        {
            bool allConditionsMet = true;
            // Check all "whereAllOfTheseApply" conditions
            foreach (var condition in effect.whereAllOfTheseApply)
            {
                if (!CheckCondition(condition))
                {
                    allConditionsMet = false;
                    break;
                }
            }
            // Check "andWhereOneOfTheseApply" conditions if all previous conditions are met
            if (allConditionsMet && effect.andWhereOneOfTheseApply.Length > 0)
            {
                bool oneConditionMet = false;
                if(effect.andWhereOneOfTheseApply.Length == 0) oneConditionMet = true;
                foreach (var condition in effect.andWhereOneOfTheseApply)
                {
                    if (CheckCondition(condition))
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
                    ApplyEffect(effectPair);
                }
            }
        }
        // Deduct event cost from game state
        ApplyEffect(eventData.cost);
    }

    private void ApplyEffect(EventEffectPair cost)
    {
        throw new NotImplementedException();
    }

    bool CheckCondition(EventSelector condition)
    {
        // Implement logic to check if the condition is met based on the game state
        // This is a placeholder implementation
        return true;
    }
}
