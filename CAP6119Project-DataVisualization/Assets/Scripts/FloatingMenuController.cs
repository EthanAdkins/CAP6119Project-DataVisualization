using UnityEngine;
using UnityEngine.InputSystem;

public class FloatingMenuController : MonoBehaviour
{
    public Transform positionTarget; // Assign to player's controller
    public GameObject menuUI; // Assign to floating menu
    public InputActionReference toggleKey; // Assign to open/close key on controller

    void OnEnable()
    {
        toggleKey.action.performed += ToggleMenu;
        toggleKey.action.Enable();
    }

    void OnDisable()
    {
        toggleKey.action.performed -= ToggleMenu;
        toggleKey.action.Disable();
    }

    private bool isVisible = false;

    void Start()
    {
        menuUI.SetActive(false); // Start hidden
    }

    void Update()
    {
        // If visible, follow the player’s view
        if (isVisible)
        {
            FollowWrist();
        }
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        isVisible = !isVisible;
        menuUI.SetActive(isVisible);
    }

    public void ShowMenu()
    {
        isVisible = true;
        menuUI.SetActive(isVisible);
    }

    public void HideMenu()
    {
        isVisible = false;
        menuUI.SetActive(isVisible);
    }

    void FollowWrist()
    {
        if (positionTarget == null) return;

        // Offset from the wrist
        Vector3 offset = positionTarget.forward * 0.01f + positionTarget.up * .2f + positionTarget.right * .2f;
        transform.position = positionTarget.position + offset;

        // Face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void SetInfo(SpeciesManager manager)
    {

    }
}
