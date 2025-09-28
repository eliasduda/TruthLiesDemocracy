using TMPro;
using UnityEngine;

public class ProjectSetter : MonoBehaviour
{
    public TextMeshProUGUI projectTitle;
    public TextMeshProUGUI winScreen;
    public TextMeshProUGUI looseScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectTitle.text = projectTitle.text.Replace("NAME", GameManager.instance.gamePlaySettings.playerName);
        projectTitle.text = projectTitle.text.Replace("PROJECT", GameManager.instance.gamePlaySettings.projectName);
        winScreen.text = winScreen.text.Replace("NAME", GameManager.instance.gamePlaySettings.playerName);
        winScreen.text = winScreen.text.Replace("PROJECT", GameManager.instance.gamePlaySettings.projectName);
        looseScreen.text = looseScreen.text.Replace("NAME", GameManager.instance.gamePlaySettings.playerName);
        looseScreen.text = looseScreen.text.Replace("PROJECT", GameManager.instance.gamePlaySettings.projectName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
