using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ResetPosition : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    [SerializeField] private XRGrabInteractable grabInteractable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        StartCoroutine(ResetOriginalPosition());
    }

    private System.Collections.IEnumerator ResetOriginalPosition()
    {
        float time = 0;
        float duration = 0.5f;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;  
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, originalPosition, time/duration);
            transform.rotation = Quaternion.Lerp(startRotation, originalRotation, time/duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

    }
}
