using UnityEngine;

[CreateAssetMenu(fileName = "HoverableWord", menuName = "Scriptable Objects/HoverableWord")]
public class HoverableWord : ScriptableObject
{
    [System.NonSerialized]
    public int link;
    public InfluencableStats category;
    public string title;
    public string description; 
    public string iconName;
    public Color fontColor = Color.white;
}
