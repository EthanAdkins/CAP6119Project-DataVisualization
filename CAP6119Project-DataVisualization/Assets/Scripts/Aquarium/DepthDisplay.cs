using UnityEngine;
using TMPro;

public class DepthDisplay : MonoBehaviour
{
    public Transform waterObject;
    public TextMeshProUGUI text;
    public float viewWindowHeight = 20f;
    public AquariumScroller aquariumScroller;
    void Update()
    {
        float waterHeight = waterObject.localScale.y;

        // World Y of the bottom of the water
        float waterBottomY = waterObject.position.y - (waterHeight / 2f);
        float bottomDepth = waterBottomY + waterHeight;
        float topDepth = bottomDepth - viewWindowHeight;

        text.text = $"{topDepth:f1}m\n|\n{bottomDepth:F1}m\n\n" +
            $"traversal speed\n" +
            $"{aquariumScroller.scrollSpeed:f1}m/s";
    }
}
