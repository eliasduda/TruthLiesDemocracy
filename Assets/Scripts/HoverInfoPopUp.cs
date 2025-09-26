using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverInfoPopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.NonSerialized]
    public bool isMouseOver;

    public TextMeshProUGUI titleTMP;
    public TextMeshProUGUI decription;

    public TMPHoverableText hoverableText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(HoverableWord word)
    {
        titleTMP.text = word.title;
        decription.text = word.description;
    }

    public void Close()
    {
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        GameManager.instance.eventManager.onPopUpUnhovered.Invoke(this);
    }
}
