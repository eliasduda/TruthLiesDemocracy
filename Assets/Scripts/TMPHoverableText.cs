using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TMPHoverableText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI myTMP;

    private bool hoveringLink;
    private int hoveredLinkIndex;
    bool isPointerOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTMP = GetComponent<TextMeshProUGUI>();
        if (myTMP == null) Debug.LogError("No TextMeshProUGUI component found on " + gameObject.name);
        SetText(myTMP.text);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(myTMP, mousePos, null);
        

        if (linkIndex != -1 && isPointerOver)
        {
            if (linkIndex != hoveredLinkIndex) 
            { 
                var linkInfo = myTMP.textInfo.linkInfo[linkIndex];
                int link = int.Parse(linkInfo.GetLinkID());
                Debug.Log("Hover over: " + linkInfo.GetLinkID());

                GameManager.instance.eventManager.onHoverableWordHovered.Invoke(
                    link,
                    this,
                    mousePos
                );
                hoveringLink = true;
                hoveredLinkIndex = linkIndex;
            }
        }
        else
        {
            hoveredLinkIndex = -1;
            hoveringLink = false;
        }
    }

    public void SetText(string newText)
    {
        if (myTMP != null)
        {
            myTMP.text = GameManager.instance.hoverPopUpManager.Parse(newText);
            Debug.Log("Set text to: " + myTMP.text);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        Debug.Log("Pointer entered over " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        Debug.Log("Pointer exited over " + gameObject.name);
    }
}

