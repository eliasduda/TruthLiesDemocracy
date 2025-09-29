using UnityEngine;
using UnityEngine.InputSystem;

public class SceneSwitch : MonoBehaviour
{
    public static bool tutorialEnd = false;

    InputAction click;

    private void Start()
    {
        click = InputSystem.actions.FindAction("Click");
    }

    public static void SwitchScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (tutorialEnd && click.WasPressedThisFrame() && !Typewriter.isPlaying)
            SwitchScene("UISetup");
    }
}
