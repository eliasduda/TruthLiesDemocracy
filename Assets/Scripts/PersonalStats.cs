using System;
using TMPro;
using UnityEngine;

public class PersonalStats : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI nameUI;
    public TextMeshProUGUI supportUI;
    public TextMeshProUGUI trustUI;
    public TextMeshProUGUI awarenessUI;
    public TextMeshProUGUI signedUI;
    public LineRenderer lr;
    public float lineX;

    private Pupil currentPupil;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        supportUI.color = GameManager.instance.gamePlaySettings.colorSupport;
        trustUI.color = GameManager.instance.gamePlaySettings.colorTrust;
        awarenessUI.color = GameManager.instance.gamePlaySettings.colorAwareness;
        signedUI.color = GameManager.instance.gamePlaySettings.mainColor;

        GameManager.instance.eventManager.onClickedPupil.AddListener(ShowPupil);
        GameManager.instance.eventManager.onClickedBackground.AddListener(ShowNothing);
        GameManager.instance.eventManager.onPupilStatInfluenced.AddListener(UpdateCurrent);

        ShowNothing();
    }

    private void UpdateCurrent(InfluencableStats arg0, float arg1, Pupil arg2)
    {
        if(arg2 == currentPupil)
        {
            ShowPupil(currentPupil);
        }
    }

    private void ShowNothing()
    {
        panel.SetActive(false);
        currentPupil = null;
    }

    private void ShowPupil(Pupil pupil)
    {
        DisplayPupil(pupil);
        panel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentPupil != null)
        {
            lr.SetPosition(0, currentPupil.transform.position);
            lr.SetPosition(1, new Vector2(lineX, currentPupil.transform.position.y));
        }
    }

    void DisplayPupil(Pupil pupil) { 
        currentPupil = pupil;

        nameUI.text = currentPupil.stats.name;
        supportUI.text = currentPupil.stats.support.ToString("F1");
        trustUI.text = currentPupil.stats.trust.ToString("F1");
        awarenessUI.text = currentPupil.stats.isAware.ToString("F1");
        signedUI.text = currentPupil.stats.hasSigned.ToString();
        signedUI.color = currentPupil.stats.hasSigned ? GameManager.instance.gamePlaySettings.mainColor : Color.red;
    }
}
