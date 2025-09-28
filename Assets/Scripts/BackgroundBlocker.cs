using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundBlocker : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("PointerBackground");
        GameManager.instance.eventManager.onClickedBackground.Invoke();
    }
}
