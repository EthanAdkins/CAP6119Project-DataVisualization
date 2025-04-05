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
        // _CaughtFishSpeciesManagers.Add(caughtFish);
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
        UpdateFishChart(_CaughtFishSpeciesManagersCounts);

    }

    public void UpdateFishChart(Dictionary<SpeciesManager, int> _CaughtFishSpeciesManagersCounts)
    {
        barChart.ClearData();
        int maxValue = 0;
        foreach (var kvp in _CaughtFishSpeciesManagersCounts)
        {
            string fishName = kvp.Key.SpeciesName;
            int count = kvp.Value;
            if (count > maxValue)
            {
                maxValue = count;
            }

            barChart.AddXAxisData(fishName);
            barChart.AddData(0, count); // 0 is the index of the first serie
        }

        var yAxis = barChart.EnsureChartComponent<YAxis>();
        yAxis.min = 0;
        yAxis.max = maxValue + 5;
        yAxis.type = Axis.AxisType.Value;
        yAxis.splitNumber = 5;

        barChart.RefreshChart(); // Optional: force a refresh
    }
}
