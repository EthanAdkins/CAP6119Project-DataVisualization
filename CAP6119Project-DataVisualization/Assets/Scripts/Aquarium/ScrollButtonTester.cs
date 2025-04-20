using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollButtonTester : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ScrollDirection { Up, Down }
    public ScrollDirection direction;

    public AquariumScrollButtonHandler scrollHandler;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (direction == ScrollDirection.Up)
            scrollHandler.ScrollOnce();  // Up = -1 scrollInput
        else
            scrollHandler.ScrollOnce();  // Down = +1 scrollInput
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        scrollHandler.StopScrolling();
    }
}
