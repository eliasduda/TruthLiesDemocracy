using UnityEngine;

[CreateAssetMenu(fileName = "GamePlaySettings", menuName = "Scriptable Objects/GamePlaySettings")]
public class GamePlaySettings : ScriptableObject
{
    public string playerName;
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

}
