using UnityEngine;

public class GrabTextFieldInput : MonoBehaviour
{
    [SerializeField] string text;
    public bool changesName = true;
    public bool changeProject = false;

    public GamePlaySettings gamePlaySettings;

    public event System.EventHandler OnInputFinished;

    public void GrabInputFieldText(string input)
    {
        text = input;
        Debug.Log("Input Field Text: " + text);
        if(changesName)
        {
            gamePlaySettings.playerName = text;
        }
        else if(changeProject)
        {
            gamePlaySettings.projectName = text;
        }
        OnInputFinished?.Invoke(this, System.EventArgs.Empty);
        this.gameObject.SetActive(false);
    }


}
