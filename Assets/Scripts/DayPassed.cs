using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class DayPassed : MonoBehaviour
{
    public AudioClip dayPassedSound;
    public AudioSource audioSource;
    public Image dayPassed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dayPassed.enabled = false;
    }

    public void SetDayActive()
    {
        dayPassed.enabled = true;
        AudioSource.PlayClipAtPoint(dayPassedSound, Camera.main.transform.position);
    }
}
