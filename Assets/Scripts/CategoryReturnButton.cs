using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CategoryReturnButton : MonoBehaviour
{
    public GameObject categoryPanel;



    public void ExitPanel()
    {
        float currentX = categoryPanel.transform.position.x;

        float actualScaledWidth = categoryPanel.GetComponent<RectTransform>().rect.width * categoryPanel.GetComponent<RectTransform>().lossyScale.x;

        categoryPanel.transform.DOMoveX(Screen.width + actualScaledWidth, 0.3f).SetEase(Ease.InSine);
    }
}
