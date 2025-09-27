using System;
using UnityEngine;

public class TimeAndMoneyManager : MonoBehaviour
{
    public int DaysPassed { get; private set; }
    public float MoneyTotal { get; private set; }
    public float MoneyPerDay { get; private set; }
    public float SignaturesGained { get; private set; }

    private float dayTimer;


// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.eventManager.onOneTimeStatInfluenced.AddListener(ChangeStat);
        MoneyTotal = GameManager.instance.gamePlaySettings.startMoney;

        NewDayStarts();
        GameManager.instance.eventManager.OnStatChanged.Invoke(InfluencableStats.MoneyTotal, MoneyTotal);
        GameManager.instance.eventManager.OnStatChanged.Invoke(InfluencableStats.DaysPassed, DaysPassed);
        GameManager.instance.eventManager.OnStatChanged.Invoke(InfluencableStats.Signatures, SignaturesGained);
    }

    // Update is called once per frame
    void Update()
    {
        dayTimer -= Time.deltaTime;
        if(dayTimer <= 0)
        {
            NewDayStarts();
        }
    }

    public void NewDayStarts()
    {
        ChangeStat(InfluencableStats.DaysPassed, DaysPassed+1);
        dayTimer = GameManager.instance.gamePlaySettings.dayDurationSeconds;
    }

    public void ChangeStat(InfluencableStats stat, float amount)
    {
        if (PupilStats.IsPerPupilStat(stat))
        {
            Debug.LogError("Tried to change per-pupil stat in TimeAndMoneyManager: " + stat); return;
        }
        switch (stat)
        {
            case InfluencableStats.MoneyTotal:
                MoneyTotal += amount;
                GameManager.instance.eventManager.OnStatChanged.Invoke(stat, MoneyTotal);
                break;
            case InfluencableStats.MoneyPerDay:
                MoneyPerDay += amount;
                GameManager.instance.eventManager.OnStatChanged.Invoke(stat, MoneyPerDay);
                break;
            case InfluencableStats.DaysPassed:
                DaysPassed += (int)amount;
                GameManager.instance.eventManager.OnStatChanged.Invoke(stat, DaysPassed);
                break;
            case InfluencableStats.Signatures:
                SignaturesGained += (int)amount;
                GameManager.instance.eventManager.OnStatChanged.Invoke(stat, SignaturesGained);
                break;
            default:
                Debug.LogError("Unknown stat in TimeAndMoneyManager: " + stat);
                break;
        }
    }

    internal bool CanAfford(float amount)
    {
        Debug.Log("Checking if can afford " + amount + " with total " + MoneyTotal);
        return MoneyTotal - Mathf.Abs(amount) >= 0;
    }
}
