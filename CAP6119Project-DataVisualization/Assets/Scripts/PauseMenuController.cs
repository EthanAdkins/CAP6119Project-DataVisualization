using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuController : MonoBehaviour
{
    [Header("Tab Content Panels")]
    [SerializeField] private GameObject filtersTabContents;
    [SerializeField] private GameObject optionsTabContents;
    [Header("Tab Content Buttons")]
    [SerializeField] private Button optionsTabButton;
    [SerializeField] private Button filtersTabButton;
    [Header("Tab Content Colors")]
    [SerializeField] private Color activeTabColor = new Color(0.2f, 0.5f, 1f); // #3399FF
    [SerializeField] private Color inactiveTabColor = new Color(0.282f, 0.373f, 0.463f); // #485F76
    

    [Header("")]
    public Transform positionTarget; // Assign to player's camera
    public GameObject menuUI; // Assign to main menu
    public InputActionReference toggleKey; // Assign to open/close menu button on controller
    public TMP_InputField densityInputField; // Assign to model density display
    public SpecimenDataManager specimenDataManager;
    public int modelDensity = 634; // Taken from SpecimenDataManager's totalDensity
    private bool isVisible = false;
    public TextMeshProUGUI currentSceneText;
    void OnEnable()
    {
        toggleKey.action.performed += ToggleMenu;
        SceneManager.sceneLoaded += OnSceneLoaded;
        toggleKey.action.Enable();
    }

    void OnDisable()
    {
        toggleKey.action.performed -= ToggleMenu;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        toggleKey.action.Disable();
    }

    void Start()
    {
        menuUI.SetActive(false); // Start hidden
        ShowOptionsTab();
        UpdateSceneLabel();

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
        if (isVisible)
        {
            // Face the camera
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        if (isVisible == true)
        {
            HideMenu();
        }
        else
        {
            FollowPosition();
            ShowMenu();

        }
    }

    public void ShowMenu() => menuUI.SetActive(isVisible = true);
    public void HideMenu() => menuUI.SetActive(isVisible = false);

    public void ShowFiltersTab()
    {
        filtersTabContents.SetActive(true);
        optionsTabContents.SetActive(false);

        filtersTabButton.image.color = activeTabColor;
        optionsTabButton.image.color = inactiveTabColor;
    }

    public void ShowOptionsTab()
    {
        filtersTabContents.SetActive(false);
        optionsTabContents.SetActive(true);

        filtersTabButton.image.color = inactiveTabColor;
        optionsTabButton.image.color = activeTabColor;
    }

    void FollowPosition()
    {
        if (positionTarget == null) return;

        // Offset from the player camera
        Vector3 offset = positionTarget.forward * 2f;
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

    public void UpdateSceneLabel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        currentSceneText.text = $"{sceneName}";
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateSceneLabel();
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeModelDensityAndReload()
    {
        if (specimenDataManager.TotalDensity != modelDensity)
        specimenDataManager.TotalDensity = modelDensity;
        ResetScene();
    }

    public void ChangeScene(string sceneName)
    {
         if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    public void QuitApp()
    {
        Debug.Log("Quitting application...");
        Application.Quit();
    }
}
