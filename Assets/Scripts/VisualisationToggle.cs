using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VisualisationToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InfluencableStats stat;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instance.pupilManager.ChnangeVisualisation(stat);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.pupilManager.ChnangeVisualisation(InfluencableStats.Signatures);
    }
}
