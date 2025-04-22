using System;
using UnityEngine;

public class bobberSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bobberSplash;
    [SerializeField] private AudioSource _fishSplash;
    [SerializeField] private float pushForce = 5f;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if (other.CompareTag("bobber"))
        {
            _bobberSplash.Play();
            Debug.Log("BobberSplash");
        }
        if (other.gameObject.layer == 9)
        {
            _fishSplash.Play();
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("bobber"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(Vector3.up * pushForce, ForceMode.Acceleration);
            }

        }
    }
}
