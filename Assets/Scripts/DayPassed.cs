using DG.Tweening;
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
        transform.DOPunchScale(new Vector3(1.2f, 1.2f, 0), 1f, 5, 1);
        dayPassed.enabled = true;
    }
}
