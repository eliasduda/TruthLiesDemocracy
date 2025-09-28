using System;
using UnityEngine;

public class PupilVisualUpdater : MonoBehaviour
{
    public Pupil pupil;
    public SpriteRenderer sefaultSR, glowSR;

    private float radius, radiusCurrent;
    public Vector2 minMaxGlow;
    public Vector2 minMaxOutline;
    private float innerIsColored, innerIsColoredCurrent;
    private float outline, outlineCurrent;
    private float outerOpacity, outerOpacityCurrent;
    private Color mainColor;
    public float smooth = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        pupil = GetComponent<Pupil>();
        if (sefaultSR == null) Debug.LogError("No SpriteRenderer found in children of " + gameObject.name);

        GameManager.instance.eventManager.onPupilStatInfluenced.AddListener(UpdateVisuals);
        GameManager.instance.eventManager.onUpdateVisuals.AddListener(UpdateVisuals);
    }

    void Start()
    {
        radius = sefaultSR.material.GetFloat("_radius");
        innerIsColored = sefaultSR.material.GetFloat("_hasBackgroundColor");
        outline = sefaultSR.material.GetFloat("_outline");
        outerOpacity = glowSR.material.GetFloat("_outerOpacity");
        UpdateVisuals();
    }

    void Update()
    {
        radiusCurrent = Mathf.Lerp(radiusCurrent, radius, Time.deltaTime * smooth);
        innerIsColoredCurrent = Mathf.Lerp(innerIsColoredCurrent, innerIsColored, Time.deltaTime * smooth);
        outerOpacityCurrent = Mathf.Lerp(outerOpacityCurrent, outerOpacity, Time.deltaTime * smooth);
        outlineCurrent = Mathf.Lerp(outlineCurrent, outline, Time.deltaTime * smooth);
        sefaultSR.material.SetFloat("_radius", radiusCurrent);
        sefaultSR.material.SetFloat("_hasBackgroundColor", 1 - innerIsColoredCurrent);
        sefaultSR.material.SetFloat("_outline", outlineCurrent);
        sefaultSR.material.SetColor("_innerColor", mainColor);
        glowSR.material.SetFloat("_outerOpacity", outerOpacityCurrent);
        glowSR.material.SetFloat("_radius", radiusCurrent);
        glowSR.material.SetColor("_innerColor", mainColor);
    }

    // Update is called once per frame
    void UpdateVisuals()
    {
        UpdateVisuals(InfluencableStats.Awareness, 0, pupil);
    }
    void UpdateVisuals(InfluencableStats arg0, float arg1, Pupil arg2)
    {
        if (pupil != arg2) return;

        radius = pupil.stats.trust;
        //spriteRenderer.material.SetFloat("outline", pupil.stats.isAware);

        innerIsColored = pupil.stats.hasSigned ? 1 : 0;
        mainColor = GameManager.instance.gamePlaySettings.mainColor;
        switch (GameManager.instance.pupilManager.visualizeStat)
        {
            case InfluencableStats.Awareness:
                innerIsColored = pupil.stats.isAware;
                mainColor = GameManager.instance.gamePlaySettings.colorAwareness;
                break;
            case InfluencableStats.Trust:
                innerIsColored = pupil.stats.trust;
                mainColor = GameManager.instance.gamePlaySettings.colorTrust;
                break;
            case InfluencableStats.Support:
                innerIsColored = pupil.stats.support;
                mainColor = GameManager.instance.gamePlaySettings.colorSupport;
                break;
        }
        outline = Mathf.Lerp(minMaxOutline.x, minMaxOutline.y, pupil.stats.isAware);
        outerOpacity = Mathf.Lerp( minMaxGlow.x , minMaxGlow.y, pupil.stats.support);

    }
}
