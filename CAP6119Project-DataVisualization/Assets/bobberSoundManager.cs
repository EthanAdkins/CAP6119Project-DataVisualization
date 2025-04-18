using System;
using UnityEngine;

public class bobberSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bobberSplash;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bobber"))
        {
            _bobberSplash.Play();
            Debug.Log("BobberSplash");
        }
    }
}
