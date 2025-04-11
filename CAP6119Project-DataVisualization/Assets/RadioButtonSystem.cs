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
        gameManager = GetComponent<FishingGameManager>();
    }

    public void Submit()
    {
        Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
        gameManager.UpdateFishChart(toggle.GetComponentInChildren<Text>().text, true);
    }
}
