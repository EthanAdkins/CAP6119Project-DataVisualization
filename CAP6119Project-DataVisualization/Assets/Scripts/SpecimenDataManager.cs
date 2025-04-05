using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using Debug = UnityEngine.Debug;

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
    [SerializeField] private bool initialSpawn = true;
    public bool Loaded
    {
        get
        {
            return _loaded;
        }
        private set
        {
            _loaded = value;
            if (_loaded && OnLoaded != null)
            {
                OnLoaded();
            }
        }
    }
    
    public delegate void SpawnManagersLoadedAction();

    public static event SpawnManagersLoadedAction OnLoaded;


    public delegate void FilterChangedAction(string newFilter, string oldFilter);
    public event FilterChangedAction OnFilterChanged;
    
    private bool _modelsReady = false;
    private bool ModelsReady
    {
        get
        {
            return _modelsReady;
        }
        set
        {
            _modelsReady = value;
            if (_modelsReady && JSONDataManager is not null 
                             && JSONDataManager.Loaded)
                ProcessData();
        }
    }

    private void JSONDataReady()
    {
        if (ModelsReady) ProcessData();
    }

    private AssetBundle _modelPrefabs;
    private GameObject LoadPrefabFromString(string s)
    {
        if (_modelPrefabs is null) throw new InvalidAsynchronousStateException("Assets Not Loaded");
        if (string.IsNullOrEmpty(s))
        {
            throw new ArgumentException("Model String cannot be null or empty - please pass a model file");
        }

        var loadedObject = _modelPrefabs.LoadAsset<GameObject>(s);
        if (loadedObject is null)
        {
            throw new FileNotFoundException("no file found - please check the configuration");
        }
        return loadedObject;
    }
    
    // Update to use coroutines so that this does not block the user/scene during setup (enable background async loading)
    private void ProcessData()
    {
        if (_loading) return;
        _loading = true;
        
        sample_count = JSONDataManager.specimenData.totalCount;
        
        if (SpeciesControllers is not null)
            // need to delete all previous controllers OR just create ones that dont exist
            SpeciesControllers.Clear();
        else
            SpeciesControllers = new List<SpeciesManager>();
        
        // walk through and create SpeciesManagers
        // Need to clear strings once going back to the level that that model is from
        float maxDepth = 0;
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

                                    GameObject model = null;
                                    try
                                    {
                                        model = LoadPrefabFromString(m_string);

                                    }
                                    catch (ArgumentException e) //might actually catch other exceptions could make custom exception class
                                    {
                                        Debug.Log(
                                            $"Missing model for: {k.name} {p.name} {c.name} {o.name} {f.name} {g.name} {s.name}");
                                        Debug.LogException(e);
                                        continue; // avoid making manager obj if we have an error
                                    }
                                    catch(FileNotFoundException fe)
                                    {
                                        Debug.Log($"Cannot find {m_string} file for: : {k.name} {p.name} {c.name} {o.name} {f.name} {g.name} {s.name} ");
                                        Debug.LogException(fe);
                                        continue; // ignore faulty data
                                    }
                                    
                                    GameObject nObj = new GameObject();
                                    nObj.AddComponent<SpeciesManager>();
                                    nObj.name = s.name + " Manager";
                                    nObj.transform.SetParent(gameObject.transform);
                                    SpeciesManager manager = nObj.GetComponent<SpeciesManager>();

                                    if (s.maxDepth > maxDepth) maxDepth = s.maxDepth;

                                    // Current issue link to which level the model came from: Should be doable

                                    manager.Setup(k, p, c, o, f, g, s, sample_count, model, lvl);

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

        SpawnPointManager.SetMaxDepth(maxDepth);
        
        // Add More error handling?
        // Trigger spawn when done
        Loaded = true;
        _loading = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SpawnPointManager is null) SpawnPointManager = FindFirstObjectByType<CreateSpawnPoints>();
        
        // Need to ensure this is loaded before triggering ProcessData
        // Should this be moved into the DataManager so Loaded is only fired after these assets are ready?
        // Leverage coroutines to load AssetBundle async?
        // Definitely want this to be DontDestroyOnLoad like the JSON data therefore move to DataManager?
        // Create a new ModelAssetBundle singleton that is DontDestroyOnLoad and loads these models?
        _modelPrefabs = AssetBundle.LoadFromFile("Assets/AssetBundles/specimenmodels");
        if (_modelPrefabs is null)
        {
            Debug.LogError("Failed To Load Model Prefab Asset Bundle");
        }

        ModelsReady = true;

        OnLoaded += Spawn;
    }

    private void OnDestroy()
    {
        OnLoaded -= Spawn;
        // Check if the JSONDataManager is null
        // If not null unsub ProcessData from OnLoad
        TaxonomyManager.OnLoad -= JSONDataReady;  //ProcessData;
    }

    private void Awake()
    {
        JSONDataManager ??= FindFirstObjectByType<TaxonomyManager>();
        
        // JSONDataManager is already loaded so event wont fire Manually call process
        if (!_loaded && !_loading && JSONDataManager.Loaded && ModelsReady)
        {
            // If JSONDataManager Loaded:
            ProcessData();
        }
        else
            TaxonomyManager.OnLoad += JSONDataReady; //ProcessData;

        if (_loaded && !_spawned)
            Spawn();
    }

    // First time spawn only?
    void Spawn()
    {
        // Only trigger the spawn if we have loaded the data and
        // the current spawn is different from the new filter
        if (!_loaded) return;
        
        // Create a random co-ord within a spawn zone
        // Inst. an object at this spawn zone
        // Repeat
        // Can this be threaded AND avoid collisions
        
        // For filtering actually avoid using SPAWN method:
        // Just set active / deactive as needed
        
        foreach (SpeciesManager m in SpeciesControllers)
        {
            m.Spawn();
        }
        
        // trigger after processing all spawns --> Should the event pass the current filter?
        current_spawned_filter = filter;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialSpawn)
        {
            if (SpeciesControllers.All(m => m.spawned))
                _spawned = true;

            // Raise spawn event ONCE when filter changes and after everything loads for the first time
            if (!_spawned) Spawn();
        }
    }

    void FilterChangeEvent(string newFilter)
    {
        _spawned = false;
        filter = newFilter;
    }

    public SpeciesManager GetRandomFish()
    {
        if (SpeciesControllers == null || SpeciesControllers.Count == 0)
        {
            Debug.LogWarning("No species controllers available.");
            return null;
        }

        return SpeciesControllers[UnityEngine.Random.Range(0, SpeciesControllers.Count)];
    }

}
