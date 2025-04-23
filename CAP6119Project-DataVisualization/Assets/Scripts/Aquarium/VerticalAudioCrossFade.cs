using UnityEngine;

public class VerticalAudioCrossfade : MonoBehaviour
{
    public AudioSource surfaceAudio;
    public AudioSource underwaterAudio;

    public GameObject tank; 
    public float transitionY = 0f; // height at which to crossfade
    public float fadeRange = 20f; // how many units above/below to start fading
    public float add = 10f;
    private Transform target; // transform to track for crossfade
    private BoxCollider box;

    void Update()
    {
        box = tank.GetComponent<BoxCollider>();
        add = box.center.y + (box.size.y / 2);
        target = tank.transform; 
        float t = Mathf.Clamp01((target.position.y - transitionY + add) / fadeRange);

        // t = 0 fully above water 
        // t = 1 fully below water  
        surfaceAudio.volume = Mathf.Min(1f - t, 0.5f);
        underwaterAudio.volume = Mathf.Min(t, 1.0f);
    }
}
