using GLTFast.Schema;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
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
    public TaxonomicLevels currentLevel = TaxonomicLevels.Kingdom;
    [SerializeField] private TMP_Text _lastCaughtFishText;
    private readonly string caughtFishSignTextPrefix = "You Caught a ";

    private void Awake()
    {
        
        DataMan = FindFirstObjectByType<SpecimenDataManager>();

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
            if (instance.TryGetComponent<SpecimenBehavior>(out SpecimenBehavior behavior))
            {
                behavior.manager = caughtFish;
            }
            //instance.AddComponent<XRGrabInteractable>();
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            XRGrabInteractable grab = instance.GetComponent<XRGrabInteractable>();
            if (grab == null)
            {
                grab = instance.AddComponent<XRGrabInteractable>();
            }

            grab.selectExited.AddListener(OnFishReleased);  // Register release callback
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
    private void OnFishReleased(SelectExitEventArgs args)
    {
        GameObject releasedFish = args.interactableObject.transform.gameObject;
        Rigidbody rb = releasedFish.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.angularDamping = float.PositiveInfinity;
            rb.linearDamping = 1;

            Debug.Log($"{releasedFish.name} released and physics enabled.");
        }
    }

    public void UpdateFishChart(TaxonomicLevels level=TaxonomicLevels.Phylum, bool changeLevel=false)
    {
        if (changeLevel)
        {
            currentLevel = level;
        }
        barChart.ClearData();
        int maxValue = 0;
        Dictionary<string, int> fishLevelCounts = new Dictionary<string, int>();
        foreach (var kvp in _CaughtFishSpeciesManagersCounts)
        {
            string fishName = "";
            switch (level)
            {
                case TaxonomicLevels.Kingdom:
                    if (kvp.Key.kingdom == null)
                    {
                        fishName = BuildPhyloTree(kvp.Key.root);
                        break;
                    }
                    else
                    {
                        fishName = kvp.Key.kingdom.name;
                        break;
                    }
                case TaxonomicLevels.Phylum:
                    if (kvp.Key.phylum == null)
                    {
                        fishName = BuildPhyloTree(kvp.Key.root);
                        break;
                    }
                    else
                    {
                        fishName = kvp.Key.phylum.name;
                        break;
                    }
                case TaxonomicLevels.Class:
                    if (kvp.Key.taxclass == null)
                    {
                        fishName = BuildPhyloTree(kvp.Key.root);
                        break;
                    }
                    else
                    {
                        fishName = kvp.Key.taxclass.name;
                        break;
                    }
                case TaxonomicLevels.Order:
                    if (kvp.Key.order == null)
                    {
                        fishName = BuildPhyloTree(kvp.Key.root);
                        break;
                    }
                    else
                    {
                        fishName = kvp.Key.order.name;
                        break;
                    }
                case TaxonomicLevels.Family:
                    if (kvp.Key.family == null)
                    {
                        fishName = BuildPhyloTree(kvp.Key.root);
                        break;
                    }
                    else
                    {
                        fishName = kvp.Key.family.name;
                        break;
                    }
                case TaxonomicLevels.Genus:
                    if (kvp.Key.genus == null)
                    {
                        fishName = BuildPhyloTree(kvp.Key.root);
                        break;
                    }
                    else
                    {
                        fishName = kvp.Key.genus.name;
                        break;
                    }
                case TaxonomicLevels.Species:
                    if (kvp.Key.species == null)
                    {
                        fishName = BuildPhyloTree(kvp.Key.root);
                        break;
                    }
                    else
                    {
                        fishName = kvp.Key.species.commonName;
                        break;
                    }

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
    private string BuildPhyloTree(object root, int depth = 0)
    {
        // Recursive version called after root is identified
        // - Skip to bottom of this definition for start of function logic
        string BuildPhyloTreeRecursive(object node, int depth)
        {
            if (node is Species s) // Base case
            {
                return s.commonName;
            }
            else if (node is Genus g)
            {
                string result = g.name;
                foreach (var sp in g.Species)
                    result += BuildPhyloTreeRecursive(sp, depth + 1);
                return result;
            }
            else if (node is Family f)
            {
                string result = f.name;
                foreach (var ge in f.Genera)
                    result += BuildPhyloTreeRecursive(ge, depth + 1);
                return result;
            }
            else if (node is Order o)
            {
                string result = o.name;
                foreach (var fa in o.Families)
                    result += BuildPhyloTreeRecursive(fa, depth + 1);
                return result;
            }
            else if (node is TaxonClass c)
            {
                string result = c.name;
                foreach (var or in c.Orders)
                    result += BuildPhyloTreeRecursive(or, depth + 1);
                return result;
            }
            else if (node is Phylum p)
            {
                string result = p.name;
                foreach (var cl in p.Classes)
                    result += BuildPhyloTreeRecursive(cl, depth + 1);
                return result;
            }
            else if (node is Kingdom k)
            {
                string result = k.name;
                foreach (var ph in k.Phyla)
                    result += BuildPhyloTreeRecursive(ph, 1);
                return result;
            }
            else return "";
        }

        if (root is Species s) // Base case
        {
            return s.commonName;
        }
        else if (root is Genus g)
        {
            string result = g.name;
            foreach (var sp in g.Species)
                result += BuildPhyloTreeRecursive(sp, depth + 1);
            return result;
        }
        else if (root is Family f)
        {
            string result = f.name;
            foreach (var ge in f.Genera)
                result += BuildPhyloTreeRecursive(ge, depth + 1);
            return result;
        }
        else if (root is Order o)
        {
            string result = o.name;
            foreach (var fa in o.Families)
                result += BuildPhyloTreeRecursive(fa, depth + 1);
            return result;
        }
        else if (root is TaxonClass c)
        {
            string result = c.name;
            foreach (var or in c.Orders)
                result += BuildPhyloTreeRecursive(or, depth + 1);
            return result;
        }
        else if (root is Phylum p)
        {
            string result = p.name;
            foreach (var cl in p.Classes)
                result += BuildPhyloTreeRecursive(cl, depth + 1);
            return result;
        }
        else if (root is Kingdom k)
        {
            string result = k.name;
            foreach (var ph in k.Phyla)
                result += BuildPhyloTreeRecursive(ph, depth + 1);
            return result;
        }
        else return "No taxonomic info found.";
    }
}
