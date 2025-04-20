using UnityEngine;

public class AquariumScroller : MonoBehaviour
{
    public Transform waterObject;
    public Transform tankObject;
    public float scrollSpeed = 1f;
    public float waterHeight => waterObject.localScale.y;
    public float maxY => waterHeight / 2f;
    public float minY => -waterHeight / 2f;

    public float scrollInput = 0f; // -1 to 1 range

    private bool wasScrolling = false;

    void Update()
    {
        bool isScrolling = Mathf.Abs(scrollInput) > 0.01f;

        // If scroll state changed, notify specimens
        if (isScrolling != wasScrolling)
        {
            wasScrolling = isScrolling;

            SpecimenMovement[] specimens = Object.FindObjectsByType<SpecimenMovement>(FindObjectsSortMode.None);

            if (!isScrolling)
            {
                Bounds newBounds;
                Renderer renderer = waterObject.GetComponent<Renderer>();
                if (renderer != null)
                    newBounds = renderer.bounds;
                else
                    newBounds = waterObject.GetComponent<Collider>().bounds;

               // SpecimenMovement[] specimens = Object.FindObjectsByType<SpecimenMovement>(FindObjectsSortMode.None);
                foreach (var s in specimens)
                {
                    s.SetWaterBounds(newBounds);
                    s.UpdateInitialY();
                }
            }

            foreach (var s in specimens)
            {
                if (isScrolling) s.PauseMovement();
                else s.ResumeMovement();
            }
        }

        // Apply scroll
        if (isScrolling)
        {
            Vector3 pos = tankObject.localPosition;
            pos.y = Mathf.Clamp(pos.y + scrollInput * scrollSpeed * Time.deltaTime, minY, maxY);
            tankObject.localPosition = pos;
        }
    }

    public void SetScrollInput(float input) => scrollInput = input; // Called by buttons
    public void SetScrollSpeed(float input) => scrollSpeed = input; // Called by slider
}
