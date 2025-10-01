using System.Collections;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{

    private float timer = 0.3f;

    public void SetTimer(float seconds)
    {
        timer = seconds;
    }

    private void Start()
    {
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}
