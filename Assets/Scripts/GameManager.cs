using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GamePlaySettings gamePlaySettings;
    public PupilManager pupilManager;
    public EventManager eventManager;
    public TimeAndMoneyManager timeMoneyManager;
    public HoverPopUpManager hoverPopUpManager;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;

            timeMoneyManager = GetComponent<TimeAndMoneyManager>();
            eventManager = GetComponent<EventManager>();
            hoverPopUpManager = GetComponent<HoverPopUpManager>();
            pupilManager = GetComponent<PupilManager>();
            if( timeMoneyManager == null) Debug.LogError("TimeAndMoneyManager component not found on GameManager");
            if( eventManager == null) Debug.LogError("EventManager component not found on GameManager");
            if( hoverPopUpManager == null) Debug.LogError("HoverPopUpManager component not found on GameManager");
            if( pupilManager == null) Debug.LogError("PupilManager component not found on GameManager");
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
