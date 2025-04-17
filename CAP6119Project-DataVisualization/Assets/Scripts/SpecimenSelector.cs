using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

using UnityEngine.InputSystem;

public class SpecimenSelector : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public FloatingMenuController menuController; // Assign to FloatingMenu
    private SpecimenBehavior currentSpecimen; // Tracks the active selection
    public InputActionReference triggerAction; // Assign to controller trigger

    private void OnEnable()
    {
        triggerAction.action.performed += OnTriggerPressed;
        triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPressed;
        triggerAction.action.Disable();
    }

        private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<SpecimenBehavior>(out var specimen))
            {
                // If a new specimen is selected, update the UI
                if (specimen != currentSpecimen)
                {
                    currentSpecimen = specimen;
                    menuController.SetInfo(currentSpecimen.manager);
                }
                menuController.ShowMenu();
            }
        }
    }
}
