using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpeechTextContainer
{
    public string text;
    public bool eventAfterLine;
    public bool inputRequired;

    public SpeechEventsArgs eventArgs;
}

[System.Serializable]
public class SpeechEventsArgs : System.EventArgs
{
    public bool triggerNextDialog = false;
    public GameObject nextDialogueContainer;

    public bool triggersIconAppearance = false;
    public GameObject iconToAppear;

    public SpeechEventsArgs() { }

}

[RequireComponent(typeof(Typewriter))]
public class SpeechHandler : MonoBehaviour
{
    public SpeechTextContainer[] textContainer;

    public Typewriter typewriter;
    public TextMeshProUGUI _TMPTextField;
    public TMP_InputField _inputField;

    public delegate void TextEvent(object sender, SpeechEventsArgs e);

    public TextEvent OnTextFinished;
    public TextEvent OnLineFinished;

    private int index = 0;

    private void Start()
    {
        typewriter = GetComponent<Typewriter>();
        _TMPTextField = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        OnLineFinished += CheckEvent;
    }
    private void OnDisable()
    {
        OnLineFinished -= CheckEvent;
    }

    public void WriteNextText()
    {
        if(index < textContainer.Length)
        {
            if (textContainer[index].inputRequired)
            {
                _inputField.gameObject.SetActive(true);
            }
            typewriter.WriteText(this, textContainer[index].text, _TMPTextField, textContainer[index].eventAfterLine, textContainer[index].inputRequired);
        }

        if(index+1 >= textContainer.Length)
        {
            Debug.Log("All Text finished");
            OnTextFinished.Invoke(this, textContainer[index].eventArgs);
            return;
        }
        index++;

    }

    private void CheckEvent(object sender, SpeechEventsArgs e)
    {
        if (e.triggersIconAppearance)
        {
            ShowIcon(e.iconToAppear);
        }
    }

    public void SkipLine()
    {
        typewriter.skipping = true;
    }

    public void InvokeInTextEvent()
    {
        Debug.Log("InText Event Invoked");
        OnLineFinished?.Invoke(this, textContainer[index-1].eventArgs);
    }

    private void ShowIcon(GameObject icon)
    {
        icon.SetActive(true);
        Image image = icon.GetComponent<Image>();

        image.DOFade(1, 0.5f);
        image.transform.DOMove(_TMPTextField.transform.position + new Vector3(0, _TMPTextField.preferredHeight/2 + 30, 0), 0.5f);
    }

}
