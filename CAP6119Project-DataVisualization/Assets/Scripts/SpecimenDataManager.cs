using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;

public class SpecimenDataManager : MonoBehaviour
{
       
    //private MonoBehaviour dataLoader; // set to whatever class Justin creates for loading the data
    private bool _loaded = false;
    private bool _loading = false;

    private bool _spawned = false;
    private string current_spawned_filter; //empty = all
    private List<SpeciesManager> SpeciesControllers;
    // Needs to be a list of attributes to filter to
    // (Figuring this out will be a stretch depending on how robust we want)
    private string filter = "";
    private int sample_count; //use to get distribution for spawn
    
    public TaxonomyManager JSONDataManager;
    
    // Total count of spawned instances --> will wanna mess with this and rendering to ensure that 
    // runtime is not negatively impacted too much as well as allow good representation of data
    public int TotalDensity = 1000;
    public CreateSpawnPoints SpawnPointManager;

    private AssetBundle _modelPrefabs;
    private GameObject LoadPrefabFromString(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            throw new ArgumentException("Model String cannot be null or empty - please pass a model file");
        }

        var loadedObject = _modelPrefabs.LoadAsset<GameObject>(s); // figure out how the model is stored and prepend path as needed
        if (loadedObject == null)
        {
            throw new FileNotFoundException("no file found - please check the configuration");
        }
        return loadedObject as GameObject;
    }
    
    private bool ProcessData()
    {
        
        sample_count = JSONDataManager.specimenData.totalCount;
        
        if (SpeciesControllers is not null)
            // need to delete all previous controllers OR just create ones that dont exist
            SpeciesControllers.Clear();
        else
            SpeciesControllers = new List<SpeciesManager>();
        
        // walk through and create SpeciesManagers
        // Need to clear strings once going back to the level that that model is from
        foreach (Kingdom k in JSONDataManager.specimenData.Kingdoms)
        {
            string m_string = k.model;
            TaxonomicLevels lvl = TaxonomicLevels.Kingdom;
            
            foreach (Phylum p in k.Phyla)
            {
                if (String.IsNullOrEmpty(m_string))
                {
                    m_string = p.model;
                    lvl = TaxonomicLevels.Phylum;
                }
                
                foreach (TaxonClass c in p.Classes)
                {
                    if (String.IsNullOrEmpty(m_string))
                    {
                        m_string = c.model;
                        lvl = TaxonomicLevels.Class;
                    }
                    
                    foreach (Order o in c.Orders)
                    {
                        if (String.IsNullOrEmpty(m_string))
                        {
                            m_string = o.model;
                            lvl = TaxonomicLevels.Order;
                        }
                        
                        foreach (Family f in o.Families)
                        {
                            if (String.IsNullOrEmpty(m_string))
                            {
                                m_string = f.model;
                                lvl = TaxonomicLevels.Family;
                            }
                            
                            foreach (Genus g in f.Genera)
                            {
                                if (String.IsNullOrEmpty(m_string))
                                {
                                    m_string = g.model;
                                    lvl = TaxonomicLevels.Genus;
                                }
                                
                                foreach (Species s in g.Species)
                                {
                                    if (String.IsNullOrEmpty(m_string))
                                    {
                                        m_string = s.model;
                                        lvl = TaxonomicLevels.Species;
                                    }
                                    
                                    GameObject model = LoadPrefabFromString(m_string);

                                    GameObject nObj = new GameObject();
                                    nObj.AddComponent<SpeciesManager>();
                                    nObj.name = s.name + " Manager";
                                    nObj.transform.SetParent(gameObject.transform);
                                    SpeciesManager manager = nObj.GetComponent<SpeciesManager>();
                                    
                                    // Current issue link to which level the model came from: Should be doable
                                    
                                    manager.Setup(k,p,c,o,f,g,s, sample_count, model, lvl);

                                    SpeciesControllers.Add(manager);

                                    if (lvl == TaxonomicLevels.Species) m_string = "";
                                }

                                if (lvl == TaxonomicLevels.Genus) m_string = "";
                            }

                            if (lvl == TaxonomicLevels.Family) m_string = "";
                        }

                        if (lvl == TaxonomicLevels.Order) m_string = "";
                    }

                    if (lvl == TaxonomicLevels.Class) m_string = "";
                }

                if (lvl == TaxonomicLevels.Phylum) m_string = "";
            }
        }

        //return false if error?
        
        //JSONDataManager.OnLoad -= ProcessData()
        return true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SpawnPointManager is null) SpawnPointManager = FindFirstObjectByType<CreateSpawnPoints>();
        
        _modelPrefabs = AssetBundle.LoadFromFile("Assets/AssetBundles/specimenmodels");
        if (_modelPrefabs is null)
        {
            Debug.LogError("Failed To Load Model Prefab Asset Bundle");
        }
    }

    private void Awake()
    {
        JSONDataManager ??= FindFirstObjectByType<TaxonomyManager>();
        
        // set the data loader component and activate?
        if (!_loaded && JSONDataManager.Loaded)
        {
            // If JSONDataManager Loaded:
            _loading = true;
            _loaded = ProcessData();
        }
        // Else callback:
            // JSONDataManager.OnLoad += ProcessData()

        if (_loaded && !_spawned)
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
        
        // Make Async - Multi-thread?
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
        // process data is an expensive method: we really want to ensure this is done before scene is displayed
        if (!_loaded && JSONDataManager.Loaded && !_loading)
        {
            _loading = true;
            _loaded = ProcessData();
            // For now just only attempt to load once
            //_loading = false;
        }
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
