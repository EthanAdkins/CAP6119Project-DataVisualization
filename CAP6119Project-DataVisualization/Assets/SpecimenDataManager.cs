using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Linq;

public class SpecimenDataManager : MonoBehaviour
{
    public struct DataPoint
    {
        public float Distribution;
        public string SpeciesName;
        public GameObject SpeciesPrefab;
        public List<string> Attributes; // we will likely want to create something that can represent these
        // Class Attribute --> create some that are general i.e. size etc. and then create a generic one to represent
        // any that are not? Or just represent as string?
        // Class Attribute<T> --> string name, T Value
        
        public DataPoint(string name, float dist, GameObject prefab, IEnumerable<string> attr)
        {
            SpeciesName = name;
            Distribution = dist;

            SpeciesPrefab = prefab;
            
            Attributes = new List<string>();
            foreach (string s in attr)
            {
                Attributes.Add(s);
            }
        }
    }
    
    
    
    //private MonoBehaviour dataLoader; // set to whatever class Justin creates for loading the data
    private List<DataPoint> data;
    private bool _loaded = false;
    private bool _spawned = false;
    private string current_spawned_filter; //empty = all
    
    private List<SpeciesManager> SpeciesControllers;
    // Needs to be a list of attributes to filter to
    // (Figuring this out will be a stretch depending on how robust we want)
    private string filter = ""; 
    public GameObject PlaceHolderPrefabTest;
    // Total count of spawned instances --> will wanna mess with this and rendering to ensure that 
    // runtime is not negatively impacted too much as well as allow good representation of data
    public int TotalDensity = 1000;
    public CreateSpawnPoints SpawnPointManager;

    bool LoadData()
    {
        //return dataLoader.Load();
        data = new List<DataPoint>();
        string[] attr = { "TestAttr" };
        data.Add(new DataPoint("Test1", 0.1f, PlaceHolderPrefabTest, attr));
        data.Add(new DataPoint("Test2", 0.5f, PlaceHolderPrefabTest, attr));
        data.Add(new DataPoint("Test3", 0.4f, PlaceHolderPrefabTest, new []{"TestAttr2"}));

        if (SpeciesControllers is not null)
            SpeciesControllers.Clear();
        else
            SpeciesControllers = new List<SpeciesManager>();
        
        foreach (DataPoint s in data)
        {
            // need to create new objects with SpeciesManager components
            // Create a Prefab Species Manager
            GameObject nObj = new GameObject();
            nObj.AddComponent<SpeciesManager>();
            nObj.transform.SetParent(gameObject.transform);

            SpeciesManager manager = nObj.GetComponent<SpeciesManager>();
            manager.Setup(s);
            
            SpeciesControllers.Add(manager);
            // Add listener for spawn/filter event
        }
        
        // Process lowest distr to ensure TotalDensity * lowest_distr >= 1

        _spawned = false;

        return true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SpawnPointManager is null) SpawnPointManager = FindFirstObjectByType<CreateSpawnPoints>();
        
        // set the data loader component and activate?
        if (!_loaded)
            _loaded = LoadData();

        if (_loaded)
            Spawn();
    }

    void Spawn()
    {
        // Only trigger the spawn if we have loaded the data and
        // the current spawn is different from the new filter
        if (!_loaded /*|| (current_spawned_filter is not null && current_spawned_filter.Equals(filter))*/) return;
        
        // Create a random co-ord within a spawn zone
        // Inst. an object at this spawn zone
        // Repeat
        // Can this be threaded AND avoid collisions
        
        // For filtering actually avoid using SPAWN method:
        // Just set active / deactive as needed
        
        foreach (SpeciesManager m in SpeciesControllers)
        {
            // Change to be a managed event
            m.Spawn(filter);
        }
        
        current_spawned_filter = filter;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpeciesControllers.All(m => m.spawned))
            _spawned = true;
     
        // Raise spawn event ONCE when filter changes and after everything loads for the first time
        if (!_spawned) Spawn();
    }

    void FilterChangeEvent(string newFilter)
    {
        _spawned = false;
        filter = newFilter;
    }
}
