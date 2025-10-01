using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundBlocker : MonoBehaviour
{
    public void InvokeOnClick()
    {
        Debug.Log("PointerBackground");
        GameManager.instance.eventManager.onClickedBackground.Invoke();
    }

}
