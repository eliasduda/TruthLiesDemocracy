using UnityEngine;
using System.Collections.Generic;

public class GenerateDaysVisualizer : MonoBehaviour
{

    public int maxDays = 30;
    public GameObject dayVisualizerPrefab;
    public GameObject divider;
    public int dividerAfterDays = 5;

    private List<DayPassed> dayVisualizeList = new List<DayPassed>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < maxDays; i++)
        {
           GameObject temp =  Instantiate(dayVisualizerPrefab, transform);
            dayVisualizeList.Add(temp.GetComponent<DayPassed>());
            if ((i + 1) % dividerAfterDays == 0 && i != maxDays - 1)
            {
                Instantiate(divider, transform);
            }
        }

        if(GameManager.instance.eventManager == null)
        {
            Debug.LogError("No EventManager found in GameManager");
            return;
        }
        GameManager.instance.eventManager.OnStatChanged.AddListener((stat, value) =>
        {          
            if (stat == InfluencableStats.DaysPassed)
            {
                OnDayChange();
            }
        });
    }


    private void OnDayChange()
    {
        int index = GameManager.instance.timeMoneyManager.DaysPassed - 1;
        dayVisualizeList[index].PassDay();
    }

}
