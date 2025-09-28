using TMPro;
using UnityEngine;

public class ProjectSetter : MonoBehaviour
{
    TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = text.text.Replace("NAME", GameManager.instance.gamePlaySettings.playerName);
        text.text = text.text.Replace("PROJECT", GameManager.instance.gamePlaySettings.projectName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
