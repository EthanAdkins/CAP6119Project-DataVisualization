using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using XCharts.Runtime;

public class FishingGameManager : MonoBehaviour
{
    [SerializeField] private GameObject _SpeciesPrefab;
    [SerializeField] private Transform _SpawnLocation;
    public SpecimenDataManager DataMan;
    public BarChart barChart;
    private Dictionary<SpeciesManager, int> _CaughtFishSpeciesManagersCounts = new Dictionary<SpeciesManager, int>();
    private GameObject _lastCaughtFish;
    public string currentLevel = "name";
    private void Awake()
    {
        barChart.ClearData();
        barChart.EnsureChartComponent<Title>().text = "Fish Caught";

        barChart.AddSerie<Bar>("Fish");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DataMan is null) DataMan = FindFirstObjectByType<SpecimenDataManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void caughtFish()
    {
        
        print("CAUGHT FISH");
        // This is so the fish don't stack
        if (_lastCaughtFish != null)
        {
            if (_lastCaughtFish.transform.position == _SpawnLocation.position)
            {
                _lastCaughtFish.SetActive(false);
            }
        }
        
        SpeciesManager caughtFish = DataMan.GetRandomFish();
        if (caughtFish != null)
        {
            GameObject instance = Instantiate(caughtFish.SpeciesPrefab, _SpawnLocation.position,
                        _SpawnLocation.rotation);
            instance.AddComponent<XRGrabInteractable>();
            instance.GetComponent<Rigidbody>().useGravity = false;
            instance.GetComponent<Rigidbody>().isKinematic = true;

            _lastCaughtFish = instance;

            if (_CaughtFishSpeciesManagersCounts.ContainsKey(caughtFish))
            {
                _CaughtFishSpeciesManagersCounts[caughtFish]++;
            }
            else
            {
                _CaughtFishSpeciesManagersCounts[caughtFish] = 1;
            }
            UpdateFishChart(currentLevel, false);
        }
        else
        {
            Debug.Log("Models not loaded yet!");
        }
        

    }

    public void UpdateFishChart(string level="name", bool changeLevel=false)
    {
        if (changeLevel)
        {
            currentLevel = level;
        }
        level = level.ToLower();
        barChart.ClearData();
        int maxValue = 0;
        Dictionary<string, int> fishLevelCounts = new Dictionary<string, int>();
        foreach (var kvp in _CaughtFishSpeciesManagersCounts)
        {
            string fishName = "";
            if (level == "kingdom")
            {
                fishName = kvp.Key.kingdom.name;
            }
            else if (level == "phylum")
            {
                fishName = kvp.Key.phylum.name;
            }
            else if (level == "taxclass")
            {
                fishName = kvp.Key.taxclass.name;
            }
            else if (level == "family")
            {
                fishName = kvp.Key.family.name;
            }
            else if (level == "order")
            {
                fishName = kvp.Key.order.name;
            }
            else if (level == "genus")
            {
                fishName = kvp.Key.genus.name;
            }
            else if (level == "species")
            {
                fishName = kvp.Key.species.name;
            } else
            {
                fishName = kvp.Key.SpeciesName;
            }

            if (fishLevelCounts.ContainsKey(fishName))
            {
                fishLevelCounts[fishName] += kvp.Value;
            }
            else
            {
                fishLevelCounts[fishName] = kvp.Value;
            }
        }
        foreach (var entry in fishLevelCounts)
        {
            barChart.AddXAxisData(entry.Key); 
            barChart.AddData(0, entry.Value); 
            if (entry.Value > maxValue)
            {
                maxValue = entry.Value; 
            }
        }

        var yAxis = barChart.EnsureChartComponent<YAxis>();
        yAxis.min = 0;
        yAxis.max = maxValue + 5;
        yAxis.type = Axis.AxisType.Value;
        yAxis.splitNumber = 5;

        barChart.RefreshChart(); 
    }
}
