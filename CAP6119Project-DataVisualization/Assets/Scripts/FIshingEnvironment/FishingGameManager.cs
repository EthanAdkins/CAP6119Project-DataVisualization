using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
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
    [SerializeField] private TMP_Text _lastCaughtFishText;
    private readonly string caughtFishSignTextPrefix = "You Caught a ";

    private void Awake()
    {
        if (DataMan is null)
        {
            DataMan = FindFirstObjectByType<SpecimenDataManager>();
        }
        else
        {
            DataMan = FindFirstObjectByType<SpecimenDataManager>();
        }

        barChart.ClearData();
        barChart.EnsureChartComponent<Title>().text = "Fish Caught";

        barChart.AddSerie<Bar>("Fish");
        _lastCaughtFishText.text = "";

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            //instance.AddComponent<XRGrabInteractable>();
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
            _lastCaughtFishText.text = caughtFishSignTextPrefix + GetTaxonDisplayName(caughtFish) + "!";
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
            fishName = GetTaxonNameFromSpeciesManager(kvp.Key, level);

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

    private string GetTaxonNameFromSpeciesManager(SpeciesManager manager, string level)
    {
        level = level.ToLower();

        // Try direct access first (safe null checks)
        switch (level)
        {
            case "kingdom": return manager.kingdom?.name ?? "[Unknown Kingdom]";
            case "phylum": return manager.phylum?.name ?? "[Unknown Phylum]";
            case "taxclass": return manager.taxclass?.name ?? "[Unknown Class]";
            case "order": return manager.order?.name ?? "[Unknown Order]";
            case "family": return manager.family?.name ?? "[Unknown Family]";
            case "genus": return manager.genus?.name ?? "[Unknown Genus]";
            case "species": return manager.species?.name ?? "[Unknown Species]";
            default:
                // Fallback: use model_lvl and root
                return manager.model_lvl switch
                {
                    TaxonomicLevels.Kingdom => ((Kingdom)manager.root).name,
                    TaxonomicLevels.Phylum => ((Phylum)manager.root).name,
                    TaxonomicLevels.Class => ((TaxonClass)manager.root).name,
                    TaxonomicLevels.Order => ((Order)manager.root).name,
                    TaxonomicLevels.Family => ((Family)manager.root).name,
                    TaxonomicLevels.Genus => ((Genus)manager.root).name,
                    TaxonomicLevels.Species => ((Species)manager.root).name,
                    _ => "[Unknown Level]"
                };
        }
    }
    private string GetTaxonDisplayName(SpeciesManager manager)
    {
        string name = manager.root switch
        {
            Species s => s.name,
            Genus g => g.name,
            Family f => f.name,
            Order o => o.name,
            TaxonClass c => c.name,
            Phylum p => p.name,
            Kingdom k => k.name,
            _ => "Unknown"
        };

        string level = manager.model_lvl.ToString();

        return $"[{level}] {name}";
    }
}
