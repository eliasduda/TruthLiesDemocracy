using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SetPupilArea : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SetPupilAreaAfterUpdate());
    }

    private IEnumerator SetPupilAreaAfterUpdate()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame to ensure all initializations are done
        GameManager.instance.pupilManager.pupilArea = GetComponent<BoxCollider2D>();
    }
}
