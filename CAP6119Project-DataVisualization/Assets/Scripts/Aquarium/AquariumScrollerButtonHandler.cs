using UnityEngine;

public class AquariumScrollButtonHandler : MonoBehaviour
{
    public AquariumScroller scroller;
    public bool scrollUp = false; // assign in Inspector

    public void ScrollOnce()
    {
        float direction = scrollUp ? -1f : 1f;
        scroller.SetScrollInput(direction);
    }

    public void StopScrolling()
    {
        scroller.SetScrollInput(0f);
    }
}
