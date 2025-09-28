using UnityEngine;

[CreateAssetMenu(fileName = "GamePlaySettings", menuName = "Scriptable Objects/GamePlaySettings")]
public class GamePlaySettings : ScriptableObject
{
    public string playerName;
    public Color playerColor;
    public string projectName;

    public Color mainColor;
    public Color lightColor;
    public Color backgroundColor;

    public Color colorSupport, colorAwareness, colorTrust;

    public Font defaultFont;

    public float startMoney;
    public PupilStats startStats;

    public int daysTotal;
    public float dayDurationSeconds;

    public int signatureGoal;

    public AnimationCurve trustSupportMultiplier;

    public DisussionSettings disussionSettings;


    public Color GetColorForStat(InfluencableStats stat)
    {
        switch(stat)
        {
            case InfluencableStats.Support:
                return colorSupport;
            case InfluencableStats.Trust:
                return colorTrust;
            case InfluencableStats.Awareness:
                return colorAwareness;
            case InfluencableStats.Signatures:
                return mainColor;
            default:
                return Color.white;

        }

    }
}
