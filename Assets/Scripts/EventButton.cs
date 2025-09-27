using UnityEngine;
using UnityEngine.UI;

public class EventButton : MonoBehaviour
{
    Button button;
    EventData triggeringEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(OnTriggerEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEvent()
    {
        GameManager.instance.eventManager.onTryEventTrigger.Invoke(triggeringEvent);
    }
}
