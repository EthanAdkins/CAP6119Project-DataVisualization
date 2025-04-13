using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class FloatingMenuController : MonoBehaviour
{
    public Transform positionTarget; // Assign to player's controller
    public GameObject menuUI; // Assign to floating menu
    public TextMeshProUGUI representativeNameText;
    public TextMeshProUGUI taxonomyText;
    public TextMeshProUGUI statsText;
    // public Image speciesImage;
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
        if (manager == null) return;
        
         switch (manager.model_lvl)
        {
            case TaxonomicLevels.Kingdom:
                representativeNameText.text = manager.kingdom.name;
                statsText.text = "Observation Count: " + manager.kingdom.count;
                break;
            case TaxonomicLevels.Phylum:
                representativeNameText.text = manager.phylum.name;
                statsText.text = "Observation Count: " + manager.phylum.count;
                break;
            case TaxonomicLevels.Class:
                representativeNameText.text = manager.taxclass.name;
                statsText.text = "Observation Count: " + manager.taxclass.count;
                break;
            case TaxonomicLevels.Order:
                representativeNameText.text = manager.order.name;
                statsText.text = "Observation Count: " + manager.order.count;
                break;
            case TaxonomicLevels.Family:
                representativeNameText.text = manager.family.name;
                statsText.text = "Observation Count: " + manager.family.count;
                break;
            case TaxonomicLevels.Genus:
                representativeNameText.text = manager.genus.name;
                statsText.text = "Observation Count: " + manager.genus.count;
                break;
            case TaxonomicLevels.Species:
                representativeNameText.text = manager.species.name;
                statsText.text = "Observation Count: " + manager.species.count;
                break;
        }

        taxonomyText.text = manager.kingdom.name + "\n" + manager.phylum.name + "\n" + manager.taxclass.name + "\n" + manager.order.name + "\n" + manager.family.name + "\n" + manager.genus.name + "\n" + manager.species.name;
        statsText.text = "Common Name: " + manager.species.commonName + "\nMin Depth: " + manager.species.minDepth + "\nMax Depth: " + manager.species.maxDepth;

    }
}
