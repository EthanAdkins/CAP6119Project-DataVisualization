using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SpeciesManager : MonoBehaviour
{
    private string current_spawned_filter = ""; //empty = all

    public SpecimenDataManager DataMan;
    public string SpeciesName => species.name;

    public float Distribution;
    public GameObject SpeciesPrefab;

    private List<GameObject> specimens; //instances of this species

    public Kingdom kingdom;
    public Phylum phylum;
    public TaxonClass taxclass;
    public Family family;
    public Order order;
    public Genus genus;
    public Species species;
    public TaxonomicLevels model_lvl = TaxonomicLevels.Species; //default to species level model

    public bool Ready => _ready;

    private bool _ready = false;

    public CreateSpawnPoints SpawnPointManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Setup(Kingdom k, Phylum p, TaxonClass c, Order o, Family f, Genus g,
        Species dataPoint, int TotalSampleCount, GameObject model, TaxonomicLevels lvl)
    {
        // we can just use the Species Object instead of copying the data
        kingdom = k;
        phylum = p;
        taxclass = c;
        family = f;
        order = o;
        genus = g;
        species = dataPoint;
        SpeciesPrefab = model;
        model_lvl = lvl;
        
        Distribution = species.count / TotalSampleCount;

        _ready = true;
        spawned = false;
    }
    
    public bool spawned { get; private set; } = false;

    void Start()
    {
        if (DataMan is null) DataMan = FindFirstObjectByType<SpecimenDataManager>();
        SpawnPointManager = DataMan.SpawnPointManager;
        spawned = false;
    }

    // Event Listener for Spawn Event on SpecimenDataManager
    
    // Split filter out into a different method and event: Filter will just activate/deactivate existing objects
    
    // Look into performance and rendering things we can do to ensure no issues with larger spawns
    public void Spawn(string filter)
    {
        if (!_ready) return;
        DataMan ??= FindFirstObjectByType<SpecimenDataManager>();
        SpawnPointManager = DataMan.SpawnPointManager;
        
        // Do nothing if spawned already
        if (current_spawned_filter.Equals(filter) && spawned) return;
        
        specimens ??= new List<GameObject>();
        if (specimens.Count == 0)
        {
            int count = (int)Math.Floor(DataMan.TotalDensity * Distribution);
            for (int i = 0; i < count; i++)
            {
                // Create a new GameObject from prefab
                Vector3 point = SpawnPointManager.GetSpawnPoint();
                
                // Need to set parent to the SpawnPointManager to ensure correct placement regardless of
                // Location of manager in the world
                GameObject instance = Instantiate(SpeciesPrefab, point, 
                    quaternion.identity, SpawnPointManager.gameObject.transform);
                // Also need to figure out spawn placement and want to avoid overlaps, perhaps consider using waypoints
                // Can define size on count of waypoints consumed?
                // Behavior will be secondary --> but how they move if we want movement to avoid weird behavior
                // is gonna be a challenge
                
                specimens.Add(instance);
            }
        }

        spawned = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
