using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{

    public AudioClip[] typingSounds;
    public bool skipping = false;

    public void WriteText(SpeechHandler handler, string text, TextMeshProUGUI textComponent, bool hasEventAfterLine = false, bool isInputField = false, float delay = 0.04f)
    {
        StartCoroutine(TypeText(handler, text, textComponent, hasEventAfterLine, isInputField, delay));
    }

    private System.Collections.IEnumerator TypeText(SpeechHandler handler, string text, TextMeshProUGUI textComponent, bool hasEventAfterLine, bool isInputField, float delay)
    {
        textComponent.text = text;
        textComponent.maxVisibleCharacters = 0;
        foreach (char c in text)
        {
            if(skipping)
            {
                textComponent.maxVisibleCharacters = text.Length;
                skipping = false;
                if (hasEventAfterLine) handler.InvokeInTextEvent();
                yield break;
            }
            textComponent.maxVisibleCharacters++;
            if (typingSounds.Length > 0 && c != ' ')
            {
                AudioSource.PlayClipAtPoint(typingSounds[Random.Range(0, typingSounds.Length)], Camera.main.transform.position);
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
