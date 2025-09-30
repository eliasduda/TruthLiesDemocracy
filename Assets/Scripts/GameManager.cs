using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GamePlaySettings gamePlaySettings;
    public PupilManager pupilManager;
    public EventManager eventManager;
    public TimeAndMoneyManager timeMoneyManager;
    public HoverPopUpManager hoverPopUpManager;



    public string playerName = "Student 404";
    public Color playerColor = Color.white;

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

    [Header("Stat Images")]
    [Tooltip("List of images representing each stat in the order of the InfluenceableStat enum")]
    public List<Sprite> statImages;
    [Tooltip("Reference of the InfluenceableStat enum")]
    [SerializeField] private InfluencableStats statTypeReference;

    public Sprite GetStatImage(InfluencableStats stat)
    {
        if(stat == InfluencableStats.Signatures) return statImages[statImages.Count - 1]; // Signatures is always the last image

        int index = (int)stat;
        if (index < 0 || index >= statImages.Count)
        {
            Debug.LogError("Stat image index out of range for stat: " + stat);
            return null;
        }
        return statImages[index];
    }

}
