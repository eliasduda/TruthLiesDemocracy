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

    public bool endTutorial = false;

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

    bool inputFinished = true;

    private int index = 0;

    private void Start()
    {
        typewriter = GetComponent<Typewriter>();
        _TMPTextField = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        OnLineFinished += CheckEvent;
        OnTextFinished += CheckEvent;
    }
    private void OnDisable()
    {
        OnLineFinished -= CheckEvent;
        OnTextFinished -= CheckEvent;
    }

    public void WriteNextText()
    {
        if(!inputFinished) return;
        if (Typewriter.isPlaying)
        {
            SkipLine();
            Typewriter.isPlaying = false;
            return;
        }
        Debug.Log("Writing next text, index: " + index);
        if (index < textContainer.Length)
        {
            if (textContainer[index].inputRequired)
            {
                _inputField.gameObject.SetActive(true);
                _inputField.GetComponent<GrabTextFieldInput>().OnInputFinished += (object sender, System.EventArgs e) =>
                {
                    inputFinished = true;
                };
                inputFinished = false;
            }
            typewriter.WriteText(this, textContainer[index].text, _TMPTextField, textContainer[index].eventAfterLine, textContainer[index].inputRequired);
        }

        if(index >= textContainer.Length)
        {
            Debug.Log("All Text finished");

            if(textContainer == null) Debug.LogWarning("Container is null");
            if (textContainer[textContainer.Length - 1] == null) Debug.LogWarning("Last element is null");

            OnTextFinished?.Invoke(this, textContainer[textContainer.Length-1].eventArgs);
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
        if (e.triggerNextDialog)
        {
            e.nextDialogueContainer.SetActive(true);
            this.gameObject.SetActive(false);
        }
        if (e.endTutorial)
        {
            SceneSwitch.tutorialEnd = true;
        }
    }

    public void SkipLine()
    {
        typewriter.skipping = true;
    }

    public void InvokeInTextEvent()
    {
        Debug.Log("InText Event Invoked for index: " + index);
        OnLineFinished?.Invoke(this, textContainer[index].eventArgs);
    }

    private void ShowIcon(GameObject icon)
    {
        print("Showing icon");
        icon.GetComponent<Image>().DOFade(1, 0.5f);
        icon.transform.DOMoveY(_TMPTextField.gameObject.transform.position.y + 100 , 0.5f);
    }

}
