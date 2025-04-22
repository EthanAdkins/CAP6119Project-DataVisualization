using System;
using UnityEngine;

public class bobberSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bobberSplash;
    [SerializeField] private AudioSource _fishSplash;
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
}
