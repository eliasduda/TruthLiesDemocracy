using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CategoryReturnButton : MonoBehaviour
{
    public GameObject categoryPanel;



    public void ExitPanel()
    {
        float currentX = categoryPanel.transform.position.x;
        categoryPanel.transform.DOMoveX(currentX + categoryPanel.GetComponent<RectTransform>().rect.width, 0.3f).SetEase(Ease.InSine);
    }
}
