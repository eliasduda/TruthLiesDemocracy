using UnityEngine;

public class TimeAndMoneyManager : MonoBehaviour
{
    public int DaysPassed { get; private set; }
    public float MoneyTotal { get; private set; }
    public float MoneyPerDay { get; private set; }


// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.eventManager.onOneTimeStatChanged.AddListener(ChangeStat);
    }

    // Update is called once per frame
    void Update()
    {
        
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
                break;
            case InfluencableStats.MoneyPerDay:
                MoneyPerDay += amount;
                break;
            case InfluencableStats.DaysPassed:
                DaysPassed += (int)amount;
                break;
            default:
                Debug.LogError("Unknown stat in TimeAndMoneyManager: " + stat);
                break;
        }
    }
}
