using UnityEngine;
using UnityEngine.InputSystem;

public class SpeechContinueClicker : MonoBehaviour
{
    public SpeechHandler speaker;
    InputAction click;

    private void Start()
    {
        //speaker = GetComponent<SpeechHandler>();
        click = InputSystem.actions.FindAction("Click");
        if(speaker == null)
        {
            Debug.LogError("SpeechHandler component not found on the GameObject.");
        }
        if (click == null)
        {
            Debug.LogError("Click action not found in Input System.");
        }
    }

    private void Update()
    {
        if(click.WasPressedThisFrame())
        {
            speaker.WriteNextText();
        }
    }
}
