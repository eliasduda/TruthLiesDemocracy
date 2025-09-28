using UnityEngine;

public class GrabTextFieldInput : MonoBehaviour
{
    [SerializeField] string text;
    public bool changesName = true;
    public bool changeProject = false;

    GamePlaySettings gamePlaySettings;

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
    }


}
