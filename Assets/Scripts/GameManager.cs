using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public PupilManager pupilManager;
    public EventManager eventManager;
    public TimeAndMoneyManager timeMoneyManager;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;

            timeMoneyManager = GetComponent<TimeAndMoneyManager>();
            eventManager = GetComponent<EventManager>();
            if( timeMoneyManager == null) Debug.LogError("TimeAndMoneyManager component not found on GameManager");
            if( eventManager == null) Debug.LogError("EventManager component not found on GameManager");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
