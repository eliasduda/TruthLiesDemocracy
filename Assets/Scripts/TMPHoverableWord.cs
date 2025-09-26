using TMPro;
using UnityEngine;

public class TMPHoverableWord : MonoBehaviour
{
    public TextMeshProUGUI myTMP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTMP = GetComponent<TextMeshProUGUI>();
        if (myTMP == null) Debug.LogError("No TextMeshProUGUI component found on " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(myTMP, Input.mousePosition, null);
        if (linkIndex != -1)
        {
            var linkInfo = myTMP.textInfo.linkInfo[linkIndex];
            Debug.Log("Hover over: " + linkInfo.GetLinkID());
        }
    }
}

public class HoverableWordInfo
{
    public string word;
    public string description;
    public HoverableWordInfo(string word, string description)
    {
        this.word = word;
        this.description = description;
    }
}
