using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class RadioButtonSystem : MonoBehaviour
{
    ToggleGroup toggleGroup;
    FishingGameManager gameManager;
    private void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
        gameManager = FindFirstObjectByType<FishingGameManager>();
    }

    public void Submit()
    {
        Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (toggle == null)
        {
            Debug.Log("No toggle selected");
            return;
        }else
        {
            gameManager.UpdateFishChart(toggle.GetComponentInChildren<Text>().text, true);
        }
    }
}
