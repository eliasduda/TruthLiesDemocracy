using System;
using TMPro;
using UnityEngine;

public class StatUpdater : MonoBehaviour
{
    public InfluencableStats updatedStat;
    public TextMeshProUGUI updateField;
    private string startstring;
    public string replacestring;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startstring = updateField.text;
        //Debug.Log("StatUpdater startstring: " + GameManager.instance.eventManager);
        GameManager.instance.eventManager.OnStatChanged.AddListener(OnTimePassed);

        if (updatedStat == InfluencableStats.Signatures)
        {
            updateField.text = "<sprite name=signature> " + 0 + " / " + GameManager.instance.gamePlaySettings.signatureGoal;
        }
    }

    private void OnTimePassed(InfluencableStats stat, float amount)
    {
        if(stat == updatedStat)
        {
            updateField.text = startstring.Replace(replacestring, amount.ToString("0"));
            if(updatedStat == InfluencableStats.Signatures)
            {
                updateField.text = "<sprite name=signature> "+amount + " / " + GameManager.instance.gamePlaySettings.signatureGoal;
            }
        }
    }

}
