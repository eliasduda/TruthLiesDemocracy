using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    public string eventName;

    public string description;

    public Sprite icon;

    public EventCategory category;

    public EventEffect[] PerPupilEffects;

    public EventEffect[] OneTimeEffects;
}
