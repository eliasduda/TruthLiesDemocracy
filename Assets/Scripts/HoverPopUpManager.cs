using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoverPopUpManager : MonoBehaviour
{
    public HoverableWord[] hoverableWords;

    public HoverInfoPopUp hoverInfoPopUpPrefab;
    public Canvas popUpCanvas;

    private List<HoverInfoPopUp> popUpStack = new List<HoverInfoPopUp>();


    private bool skipNextFrame = false;
    private void Awake()
    {
        int i = 0;
        foreach (var word in hoverableWords)
        {
            word.link = i;
            i++;
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(var word in hoverableWords)
        {
            word.description = Parse(word.description);
        }

        GameManager.instance.eventManager.onHoverableWordHovered.AddListener(OnOpenNewPopUp);
        GameManager.instance.eventManager.onHoverableEventHovered.AddListener(OnOpenNewPopUp);
        GameManager.instance.eventManager.onPopUpUnhovered.AddListener(OnLefPopUp);
    }

    // Update is called once per frame
    void Update()
    {
        if (skipNextFrame)
        {
            skipNextFrame = false;
            return;
        }

        if (popUpStack.Count == 0) return;
        //else if (!popUpStack[popUpStack.Count - 1].isMouseOver) OnLefPopUp(popUpStack[popUpStack.Count - 1]); // Recursively close if the next one is also not hovered over

    }

    public string Parse(string input)
    {
        return Regex.Replace(input, @"\$(\w+)_(\w+)", match =>
        {
            string title = match.Groups[1].Value; // "gold"
            string keep = match.Groups[2].Value; // "1"

            // Find matching HoverableWord
            HoverableWord word = System.Array.Find(hoverableWords,
                w => w.title.Equals(title, System.StringComparison.OrdinalIgnoreCase));

            if (word != null)
            {
                string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(word.fontColor);
                // Build text with color, title, and optional icon inside the link
                string content = $"<color=#{hex}>{keep}</color>";

                if (!string.IsNullOrEmpty(word.iconName))
                    content += $" <sprite name={word.iconName}>";

                return $"<link=\"{word.link}\">{content}</link>";
            }

            // If no match, leave original text
            return match.Value;
        });
    }

    void OnOpenNewPopUp(int wordIndex, TMPHoverableText hoverable, Vector2 mousePos)
    {
        Debug.Log("Opening pop-up for word index: " + wordIndex);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popUpCanvas.transform as RectTransform,
            mousePos,
            popUpCanvas.worldCamera,
            out pos
        );

        HoverInfoPopUp ui = Instantiate(hoverInfoPopUpPrefab, popUpCanvas.transform);
        ui.transform.localPosition = pos;
        ui.transform.localScale = Vector3.one;

        ui.SetupWord(hoverableWords[wordIndex]);

        popUpStack.Add(ui);
    }
    void OnOpenNewPopUp(int unlockedAffordableState, EventData eventData, Vector2 mousePos)
    {
        Debug.Log("Opening pop-up for event : " + eventData.eventName);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popUpCanvas.transform as RectTransform,
            mousePos,
            popUpCanvas.worldCamera,
            out pos
        );

        HoverInfoPopUp ui = Instantiate(hoverInfoPopUpPrefab, popUpCanvas.transform);
        ui.transform.localPosition = pos;
        ui.transform.localScale = Vector3.one;

        ui.SetupEvent(eventData, unlockedAffordableState > 1, unlockedAffordableState > 0);

        popUpStack.Add(ui);
    }

    void OnLefPopUp(HoverInfoPopUp left)
    {
        Debug.Log("Left pop-up: " + left.titleTMP.text);
        if (popUpStack.Count <= 0) return; // No popups to close
        if (left == popUpStack[popUpStack.Count - 1])
        {
            left.Close();
            popUpStack.RemoveAt(popUpStack.Count - 1);
            
        }
        skipNextFrame = true;
    }

    public bool GetHoverableWordByCategory(InfluencableStats category, out HoverableWord outWord)
    {
        outWord = System.Array.Find(hoverableWords, w => w.category == category);
        return outWord != null;
    }

}
