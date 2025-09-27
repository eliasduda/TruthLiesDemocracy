using DG.Tweening;
using UnityEngine;

public class CategoryButton : MonoBehaviour
{
    public GameObject categoryPanel;
    public GameObject categoryBG;

    public void ToggleCategoryPanel()
    {
       categoryPanel.transform.DOMoveX(categoryBG.transform.position.x, 0.3f).SetEase(Ease.OutSine);
    }

}
