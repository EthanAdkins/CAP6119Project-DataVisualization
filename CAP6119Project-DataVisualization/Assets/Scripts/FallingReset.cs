using UnityEngine;

public class FallingReset : MonoBehaviour
{
    public Transform player;              // Assign to player
    public float fallThreshold = -10f;    // Official falling height
    public Transform resetPoint;          // set to starting position

    void Update()
    {
        if (player.position.y < fallThreshold)
        {
            ResetPlayerPosition();
        }
    }

    void ResetPlayerPosition()
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false; // Disable to allow teleport
        }

        player.position = resetPoint.position;
        player.rotation = resetPoint.rotation;

        if (controller != null)
        {
            controller.enabled = true; // Re-enable after teleport
        }

        Debug.Log("Player reset to starting position.");
    }
}
