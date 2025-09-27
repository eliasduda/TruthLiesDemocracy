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
    private RectTransform popUpCanvasTransform;

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

        popUpCanvasTransform = popUpCanvas.GetComponent<RectTransform>();
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
        HoverInfoPopUp ui = Instantiate(hoverInfoPopUpPrefab, popUpCanvas.transform);
        RectTransform uiRect = ui.GetComponent<RectTransform>();
        ui.SetupWord(hoverableWords[wordIndex]);

        // Convert screen point to canvas local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popUpCanvasTransform,
            mousePos,
            popUpCanvas.worldCamera,
            out Vector2 localPos
        );

        // Start with bottom-left corner at the mouse
        Vector2 pivot = new Vector2(0, 0);

        // Calculate pop-up size
        Vector2 size = uiRect.rect.size;
        Vector2 canvasSize = popUpCanvasTransform.rect.size;

        // Adjust pivot if it would go off canvas
        if (localPos.x + size.x > canvasSize.x / 2) pivot.x = 1;      // flip horizontally
        if (localPos.y + size.y > canvasSize.y / 2) pivot.y = 1;      // flip vertically

        uiRect.pivot = pivot;
        uiRect.localPosition = localPos;

        popUpStack.Add(ui);
    }
    void OnOpenNewPopUp(int unlockedAffordableState, EventData eventData, Vector2 mousePos)
    {
        // Instantiate the pop-up
        HoverInfoPopUp ui = Instantiate(hoverInfoPopUpPrefab, popUpCanvas.transform);
        RectTransform uiRect  = ui.GetComponent<RectTransform>();
        uiRect.localScale = Vector3.one;

        ui.SetupEvent(eventData, unlockedAffordableState > 1, unlockedAffordableState > 0);

        // Convert screen point to canvas local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popUpCanvasTransform,
            mousePos,
            popUpCanvas.worldCamera,
            out Vector2 localPos
        );

        // Start with bottom-left corner at the mouse
        Vector2 pivot = new Vector2(0, 0);

        // Calculate pop-up size
        Vector2 size = uiRect.rect.size;
        Vector2 canvasSize = popUpCanvasTransform.rect.size;

        // Adjust pivot if it would go off canvas
        if (localPos.x + size.x > canvasSize.x / 2) pivot.x = 1;      // flip horizontally
        if (localPos.y + size.y > canvasSize.y / 2) pivot.y = 1;      // flip vertically

        uiRect.pivot = pivot;
        uiRect.localPosition = localPos;


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
