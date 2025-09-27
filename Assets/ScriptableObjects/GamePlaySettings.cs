using UnityEngine;

[CreateAssetMenu(fileName = "GamePlaySettings", menuName = "Scriptable Objects/GamePlaySettings")]
public class GamePlaySettings : ScriptableObject
{
    public Color mainColor;
    public Color lightColor;
    public Color backgroundColor;

    public Font defaultFont;

    public float startMoney;
    public PupilStats startStats;

    public int daysTotal;
    public float dayDurationSeconds;

    public int signatureGoal;

    public AnimationCurve trustSupportMultiplier;

}
