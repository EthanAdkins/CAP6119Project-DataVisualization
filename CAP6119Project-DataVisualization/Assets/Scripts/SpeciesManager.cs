using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SpeciesManager : MonoBehaviour // Bit of a misnomer now
{
    public SpecimenDataManager DataMan;
    public string SpeciesName
    {
        get
        {
            return root switch
            { // Have to use ugly pattern matching because interfaces don't work with JSONUtility
                Species s     => s.name,
                Genus g       => g.name,
                Family f      => f.name,
                Order o       => o.name,
                TaxonClass c  => c.name,
                Phylum p      => p.name,
                Kingdom k     => k.name,
                _             => "Unknown"
            };
        }
    }


    public double Distribution;
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
    public object root;
    public bool Ready => _ready;

    private bool _ready = false;
    private bool _active = false;

    private Filter _filter = null;
    private bool _filterChanged = false;

    // Flag to indicate if we need to respawn
    public bool RequiresRespawn = false;
    private bool BeginSpawn = false;

    public float MinDepth
    {
        get
        {
            return root switch
            { // Have to use ugly pattern matching because interfaces don't work with JSONUtility
                Species s     => s.minDepth,
                Genus g       => g.minDepth,
                Family f      => f.minDepth,
                Order o       => o.minDepth,
                TaxonClass c  => c.minDepth,
                Phylum p      => p.minDepth,
                Kingdom k     => k.minDepth,
                _             => 0f
            };
        }
    }

    public float MaxDepth
    {
        get
        {
            return root switch
            { // Have to use ugly pattern matching because interfaces don't work with JSONUtility
                Species s     => s.maxDepth,
                Genus g       => g.maxDepth,
                Family f      => f.maxDepth,
                Order o       => o.maxDepth,
                TaxonClass c  => c.maxDepth,
                Phylum p      => p.maxDepth,
                Kingdom k     => k.maxDepth,
                _             => 0f
            };
        }
    }
    
    public int Count
    {
        get
        {
            return root switch
            { // Have to use ugly pattern matching because interfaces don't work with JSONUtility
                Species s     => s.count,
                Genus g       => g.count,
                Family f      => f.count,
                Order o       => o.count,
                TaxonClass c  => c.count,
                Phylum p      => p.count,
                Kingdom k     => k.count,
                _             => 0
            };
        }
    }

    public CreateSpawnPoints SpawnPointManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Setup(Kingdom k, Phylum p, TaxonClass c, Order o, Family f, Genus g,
        Species dataPoint, int TotalSampleCount, GameObject model, TaxonomicLevels lvl)
    {
        // we can just use the Species Object instead of copying the data
        kingdom = k;
        phylum = p;
        taxclass = c;
        order = o;
        family = f;
        genus = g;
        species = dataPoint;
        SpeciesPrefab = model;
        model_lvl = lvl;

        switch (model_lvl)
        {
            case TaxonomicLevels.Kingdom:
                root = kingdom;
                break;
            case TaxonomicLevels.Phylum:
                root = phylum;
                break;
            case TaxonomicLevels.Class:
                root = taxclass;
                break;
            case TaxonomicLevels.Order:
                root = order;
                break;
            case TaxonomicLevels.Family:
                root = family;
                break;
            case TaxonomicLevels.Genus:
                root = genus;
                break;
            case TaxonomicLevels.Species:
                root = species;
                break;
        }
        
        int count = root switch
        { // Have to use ugly pattern matching because interfaces don't work with JSONUtility
            Species sp     => sp.count,
            Genus ge       => ge.count,
            Family fa      => fa.count,
            Order or       => or.count,
            TaxonClass cl  => cl.count,
            Phylum ph      => ph.count,
            Kingdom ki     => ki.count,
            _             => 0
        };

        Distribution = ((double)count) / (double)TotalSampleCount;
        
        _ready = true;
        spawned = false;
    }
    
    public bool spawned { get; private set; } = false;

    void Start()
    {
        if (DataMan is null) DataMan = FindFirstObjectByType<SpecimenDataManager>();
        SpawnPointManager = DataMan.SpawnPointManager;
        DataMan.OnFilterChanged += OnFilterChanged;
        spawned = false;
    }

    // Event Listener for Spawn Event on SpecimenDataManager
    
    // Split filter out into a different method and event: Filter will just activate/deactivate existing objects
    
    // Look into performance and rendering things we can do to ensure no issues with larger spawns
    public void Spawn()
    {
        if (root == null)
        {
            Debug.LogWarning($"{gameObject.name} SpeciesManager has null root.");
            return; // yield break;
        }
        // do nothing if not ready to spawn
        if (!_ready) return; // yield break;
        DataMan ??= FindFirstObjectByType<SpecimenDataManager>();
        SpawnPointManager = DataMan.SpawnPointManager;

        // do nothing if already spawned
        if (spawned) return; // yield break;
        
        specimens ??= new List<GameObject>();
        if (specimens.Count == 0)
        {
            
            // Spawn exact count in data (does not allow for adjustable amount of spawns for performance)
            int count = Math.Max((int)Math.Floor(DataMan.TotalDensity * Distribution),1); //spawn min of 1
            for (int i = 0; i < count; i++)
            {
                // Create a new GameObject from prefab
                Vector3 point = SpawnPointManager.GetSpawnPoint(MinDepth, MaxDepth, SpeciesPrefab);
                
                // Need to set parent to the SpawnPointManager to ensure correct placement regardless of
                // Location of manager in the world
                GameObject instance = Instantiate(SpeciesPrefab, point, 
                    quaternion.identity, gameObject.transform); //test what happens when setting parent to this manager
                // Also need to figure out spawn placement and want to avoid overlaps, perhaps consider using waypoints
                // Can define size on count of waypoints consumed?
                // Behavior will be secondary --> but how they move if we want movement to avoid weird behavior
                // is gonna be a challenge
                
                // Add reference to this manager to spawned instance for selection
                if (instance.TryGetComponent<SpecimenBehavior>(out SpecimenBehavior behavior))
                {
                    behavior.manager = this;
                }
                
                instance.SetActive(_active);

                specimens.Add(instance);

                //if (i % 10 == 0) yield return null; // split to max 10 spawns per frame update? (maybe make 1 even)
            }
        }

        spawned = true;
        _active = true;
    }

    private void OnFilterChanged(Filter newFilter)
    {
        _filter = newFilter;
        _filterChanged = true;
    }

    private System.Collections.IEnumerator Filter(Filter newFilter)
    {
        // check if we match the filter
        // Disable the entities if not
        bool match = newFilter.Match(root);

        if (match != _active)
        {
            _active = match;
            //Debug.Log($"Match: {match} for {model_lvl.ToString()} {name} Min: {MinDepth} Max: {MaxDepth} Count: {Count}");
            int c = 0;
            foreach (GameObject s in specimens)
            {
                s.SetActive(match);
                if (++c > 10) // filter max of 10 per frame update
                {
                    c = 0;
                    yield return null;
                }
            }
        }
    }

    private System.Collections.IEnumerator Respawn()
    {
        int currentCount = specimens.Count;

        int desiredCount = Math.Max((int)Math.Floor(DataMan.TotalDensity * Distribution),1); //spawn min of 1;
        
        if (currentCount == desiredCount) yield break;
        if (currentCount > desiredCount)
        {
            int removeCount = currentCount - desiredCount;
            
            // delete x instances
            for (int i = 0; i < removeCount; i++)
            {
                GameObject remove = specimens[^1];

                specimens.Remove(remove);
                
                GameObject.Destroy(remove);

                if (i % 2 == 0)
                {
                    yield return null;
                }
            }
        }
        else if (currentCount < desiredCount)
        {
            int newCount = desiredCount - currentCount;

            for (int i = 0; i < newCount; i++)
            {
                // Create a new GameObject from prefab
                Vector3 point = SpawnPointManager.GetSpawnPoint(MinDepth, MaxDepth, SpeciesPrefab);

                // Need to set parent to the SpawnPointManager to ensure correct placement regardless of
                // Location of manager in the world
                GameObject instance = Instantiate(SpeciesPrefab, point,
                    quaternion.identity, gameObject.transform); //test what happens when setting parent to this manager
                // Also need to figure out spawn placement and want to avoid overlaps, perhaps consider using waypoints
                // Can define size on count of waypoints consumed?
                // Behavior will be secondary --> but how they move if we want movement to avoid weird behavior
                // is gonna be a challenge

                // Add reference to this manager to spawned instance for selection
                if (instance.TryGetComponent<SpecimenBehavior>(out SpecimenBehavior behavior))
                {
                    behavior.manager = this;
                }
                
                instance.SetActive(_active);

                specimens.Add(instance);

                if (i % 2 == 0) yield return null;
            }
        }

        spawned = true;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (_filterChanged)
        {
            _filterChanged = false;
            StartCoroutine(Filter(_filter)); // split disable / enable of entities across frame updates
        }

        if (_ready && RequiresRespawn)
        {
            spawned = false;
            RequiresRespawn = false;
            StartCoroutine(Respawn());
        }
    }

    private void OnDestroy()
    {
        DataMan.OnFilterChanged -= OnFilterChanged;
    }
}
