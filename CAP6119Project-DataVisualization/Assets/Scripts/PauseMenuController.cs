using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuController : MonoBehaviour
{
    [Header("Tab Content Panels")]
    [SerializeField] private GameObject filtersTabContent;
    [SerializeField] private GameObject optionsTabContent;

    public Transform positionTarget; // Assign to player's camera
    public GameObject menuUI; // Assign to main menu
    public InputActionReference toggleKey; // Assign to open/close menu button on controller
    public TMP_InputField densityInputField; // Assign to model density display

    public SpecimenDataManager specimenDataManager;
    public int modelDensity = 634; // Taken from SpecimenDataManager's totalDensity
    private bool isVisible = false;

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

    void Start()
    {
        menuUI.SetActive(false); // Start hidden
        ShowOptionsTab();

        if (specimenDataManager != null)
        {
            modelDensity = specimenDataManager.TotalDensity;
            densityInputField.text = modelDensity.ToString(); // Display current value
            // Listen for when the user finishes editing the field
            densityInputField.onEndEdit.AddListener(OnDensityInputChanged);
        }
        else
        {
            Debug.LogWarning("SpecimenDataManager reference not set on MainMenuController.");
        }
    }

    void Update()
    {
        // If visible, follow the player’s view
        if (isVisible)
        {
            FollowPosition();
        }
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        isVisible = !isVisible;
        menuUI.SetActive(isVisible);
    }

    public void ShowMenu() => menuUI.SetActive(isVisible = true);
    public void HideMenu() => menuUI.SetActive(false);

    public void ShowFiltersTab()
    {
        filtersTabContent.SetActive(true);
        optionsTabContent.SetActive(false);
    }

    public void ShowOptionsTab()
    {
        filtersTabContent.SetActive(false);
        optionsTabContent.SetActive(true);
    }

    void FollowPosition()
    {
        if (positionTarget == null) return;

        // Offset from the player camera
        Vector3 offset = positionTarget.forward * 1f;
        transform.position = positionTarget.position + offset;

        // Face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    void OnDensityInputChanged(string input)
    {
        if (int.TryParse(input, out int newDensity) && newDensity != 0)
        {
            modelDensity = newDensity;
            Debug.Log($"Model density updated to {modelDensity}");            
        }
        else
        {
            Debug.LogWarning("Invalid number entered for model density.");
            densityInputField.text = modelDensity.ToString(); // Reset to previous valid value
        }
    }

    void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ChangeModelDensityAndReload()
    {
        if (specimenDataManager.TotalDensity != modelDensity)
        specimenDataManager.TotalDensity = modelDensity;
        ResetScene();
    }

    void ChangeScene(string sceneName)
    {
         if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    void QuitApp()
    {
        Debug.Log("Quitting application...");
        Application.Quit();
    }
}
