using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VisualisationToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public InfluencableStats stat;
    private Image image;
    public Color normalColor = new Color(1,1,1,0.5f);
    public Color clickedColor = new Color(1, 1, 1, 1f);

    public TextMeshProUGUI title;

    private InfluencableStats prevStat;
    private bool clicked;

    private void Start()
    {
        image = GetComponent<Image>();
        GameManager.instance.eventManager.onUpdateVisuals.AddListener(VisualsUpdated);
        image.color = normalColor;
    }

    private void VisualsUpdated()
    {
        if (GameManager.instance.pupilManager.visualizeStat != stat)
        {
            clicked = false; 
            image.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        prevStat = GameManager.instance.pupilManager.visualizeStat;
        image.color = clickedColor;
        if (title) title.text = stat.ToString();
        if (title) title.color = GameManager.instance.gamePlaySettings.GetColorForStat(stat);
        //GameManager.instance.pupilManager.ChnangeVisualisation(stat);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (clicked) return;
        image.color = normalColor;
        if (title) title.text = "";
        //GameManager.instance.pupilManager.ChnangeVisualisation(prevStat);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.instance.pupilManager.ChnangeVisualisation(stat);
        clicked = true; 
        image.color = clickedColor;
    }
}
