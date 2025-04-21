using UnityEngine;
using TMPro;

public class DepthDisplay : MonoBehaviour
{
    public GameObject waterObject;
    public Transform marker;
    public TextMeshProUGUI text;
    public float viewWindowHeight = 20f;
    public AquariumScroller aquariumScroller;
    void Update()
    {
        float waterHeight = marker.localScale.y;

        // World Y of the bottom of the water
        float waterBottomY = waterObject.GetComponent<Renderer>().bounds.min.y;
        float bottomDepth = waterBottomY + waterHeight;
        float topDepth = bottomDepth - viewWindowHeight;

        text.text = $"{topDepth:f1}m\n|\n{bottomDepth:F1}m\n\n" +
            $"traversal speed\n" +
            $"{aquariumScroller.scrollSpeed:f1}m/s";
    }
}
