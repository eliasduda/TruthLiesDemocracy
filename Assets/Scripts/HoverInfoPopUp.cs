using System;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverInfoPopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //[System.NonSerialized]
    public bool isMouseOver;

    public TextMeshProUGUI titleTMP;
    public TextMeshProUGUI decription;
    public RectTransform background;

    public TMPHoverableText hoverableText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupWord(HoverableWord word)
    {
        titleTMP.text = word.title;
        decription.text = word.description;
        //UpdateBackground();
    }

    public void SetupEvent(EventData eventData, bool isLocked, bool isUnaffordable)
    {
        string lockedOrUnaffordable = isLocked ? "<color=grey> (LOCKED)</color>" : isUnaffordable ? "<color=red> (UNAFFORDABLE)</color>" : "";
        titleTMP.text = eventData.eventName + lockedOrUnaffordable;
        string fulltext = GameManager.instance.hoverPopUpManager.Parse(eventData.eventDescription);
        string changes = GetChangesText(eventData);
        decription.text = fulltext + "\n\n" + changes;
        //UpdateBackground();
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

    public void UpdateBackground()
    {
        // Force TMP to update its layout
        decription.ForceMeshUpdate();

        // Get preferred size
        Vector2 textSize = new Vector2(
            decription.preferredWidth,
            decription.preferredHeight
        );


        // Resize background
        background.sizeDelta = textSize;
    }

    public string GetChangesText(EventData eventData)
    {
        string text = "Get: \n";
        foreach (EventEffect effects in eventData.OneTimeEffects)
        {
            foreach (EventEffectPair effect in effects.effects) {
                text += GetChangesText(effect, 0);
            }
        }
        text += "Affect Pupils: \n";
        foreach (EventEffect effects in eventData.PerPupilEffects)
        {
            foreach (EventEffectPair effect in effects.effects)
            {
                text += GetChangesText(effect, eventData.duration.amount, "", " per pupil");
            }
        }
        if(eventData.cost.amount > 0) text += GetChangesText(eventData.cost, 0, "COST: ");
        if(eventData.duration.amount > 0) text += GetChangesText(eventData.duration, 0, "Takes: ");
        return text;
    }

    public string GetChangesText(EventEffectPair effect, float duration, string lineAdditionFron = "", string LineAdditionBack = "")
    {
        string text = "";
        HoverableWord catergoryWord;
        bool isValidStat = GameManager.instance.hoverPopUpManager.GetHoverableWordByCategory(effect.stat, out catergoryWord);
        Color fontColor = isValidStat ? catergoryWord.fontColor : Color.white;
        string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(fontColor);
        string perSec = duration > 0 ? $" ({(effect.amount / duration).ToString("F2")} sec)" : "";
        string changeTxt = $"<color=#{hex}>{lineAdditionFron} {effect.stat.ToString()}: {effect.amount} {perSec} {LineAdditionBack} </color>";
        if (isValidStat) changeTxt = $"<link=\"{catergoryWord.link}\">{changeTxt}</link>";
        text += changeTxt;
        text += "\n";
        return text;
    }

}
