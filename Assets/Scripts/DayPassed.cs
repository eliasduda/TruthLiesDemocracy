using UnityEngine;
using UnityEngine.UI;

public class DayPassed : MonoBehaviour
{
    public Image dayPassed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dayPassed.enabled = false;
    }

    public void PassDay()
    {
        dayPassed.enabled = true;
    }
}
