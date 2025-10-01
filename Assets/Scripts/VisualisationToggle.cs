using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VisualisationToggle : MonoBehaviour
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

    public void OnPointerEnter()
    {
        prevStat = GameManager.instance.pupilManager.visualizeStat;
        image.color = clickedColor;
        if (title) title.text = stat.ToString();
        if (title) title.color = GameManager.instance.gamePlaySettings.GetColorForStat(stat);
        //GameManager.instance.pupilManager.ChnangeVisualisation(stat);
    }

    public void OnPointerExit()
    {
        if (clicked) return;
        image.color = normalColor;
        if (title) title.text = "";
        //GameManager.instance.pupilManager.ChnangeVisualisation(prevStat);
    }

    public void OnPointerUp()
    {
        GameManager.instance.pupilManager.ChnangeVisualisation(stat);
        clicked = true; 
        image.color = clickedColor;
    }

    [ExecuteAlways]
    public class DrawUIRect : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt == null) return;

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            Gizmos.color = Color.red;
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
        }
    }

}


