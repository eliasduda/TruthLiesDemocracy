using System;
using UnityEngine;

public class PupilVisualUpdater : MonoBehaviour
{
    public Pupil pupil;
    public SpriteRenderer spriteRenderer;

    public float radius, radiusCurrent;
    public float innerOpacity, innerOpacityCurrent;
    public float outline, outlineCurrent;
    public float smooth = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pupil = GetComponent<Pupil>();
        if (spriteRenderer == null) Debug.LogError("No SpriteRenderer found in children of " + gameObject.name);

        GameManager.instance.eventManager.onPupilStatInfluenced.AddListener(UpdateVisuals);
    }

    void Start()
    {
        radius = spriteRenderer.material.GetFloat("_radius");
        innerOpacity = spriteRenderer.material.GetFloat("_innerOpacity");
        outline = spriteRenderer.material.GetFloat("_outline");
    }

    void Update()
    {
        radiusCurrent = Mathf.Lerp(radiusCurrent, radius, Time.deltaTime * smooth);
        innerOpacityCurrent = Mathf.Lerp(innerOpacityCurrent, innerOpacity, Time.deltaTime * smooth);
        outlineCurrent = Mathf.Lerp(outlineCurrent, outline, Time.deltaTime * smooth);
        spriteRenderer.material.SetFloat("_radius", radiusCurrent);
        spriteRenderer.material.SetFloat("_innerOpacity", innerOpacityCurrent);
        //spriteRenderer.material.SetFloat("_outline", outlineCurrent);
    }

    // Update is called once per frame
    void UpdateVisuals(InfluencableStats arg0, float arg1, Pupil arg2)
    {
        if (pupil != arg2) return;

        radius = pupil.stats.trust;
        //spriteRenderer.material.SetFloat("outline", pupil.stats.isAware);

        innerOpacity = pupil.stats.support;
        switch (GameManager.instance.pupilManager.visualizeStat)
        {
            case InfluencableStats.Awareness:
                innerOpacity = pupil.stats.isAware;
                break;
            case InfluencableStats.Trust:
                innerOpacity = pupil.stats.isAware;
                break;
        }

        outline = pupil.stats.isAware > 0 ? 1f : 0f;

    }
}
