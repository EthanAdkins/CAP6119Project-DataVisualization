using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;

public class SpecimenDataManager : MonoBehaviour
{
    //private MonoBehaviour dataLoader; // set to whatever class Justin creates for loading the data
    private bool _loaded = false;
    private bool _loading = false;
    private Awaitable _processingRequest;

    private bool _spawned = false;
    private string current_spawned_filter; //empty = all
    private List<SpeciesManager> SpeciesControllers;
    // Needs to be a list of attributes to filter to
    // (Figuring this out will be a stretch depending on how robust we want)
    private Filter filter = null;
    private int sample_count; //use to get distribution for spawn

    public TaxonomyManager JSONDataManager;

    // Total count of spawned instances --> will wanna mess with this and rendering to ensure that 
    // runtime is not negatively impacted too much as well as allow good representation of data
    public int TotalDensity
    {
        get
        {
            return _td;
        }
        set
        {
            _td = value;
            OnUpdateDensity();
        }
    }

    private int _td = 1000;
    public CreateSpawnPoints SpawnPointManager;
    public bool initialSpawn = true;
    [SerializeField] private GameObject cameraCanvas;
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

    public delegate void FilterChangedAction(Filter newFilter);
    public event FilterChangedAction OnFilterChanged;

    public GameObject getModel(string m_string)
    {
        try
        {
            return JSONDataManager.LoadPrefabFromString(m_string);
        }
        catch (ArgumentException e) //might actually catch other exceptions could make custom exception class
        {
            Debug.Log(
                $"Missing model for: {m_string}");
            Debug.LogException(e);
            return null; // avoid making manager obj if we have an error
        }
        catch (System.IO.FileNotFoundException fe)
        {
            Debug.Log($"Cannot find {m_string} file for: : {m_string} ");
            Debug.LogException(fe);
            return null; // ignore faulty data
        }
    }

    private async Awaitable ProcessData()
    {
        await Awaitable.MainThreadAsync();
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
            if (!String.IsNullOrEmpty(k.model))
            {
                string m_string = k.model;
                if (m_string.Equals("None"))
                {
                    Debug.LogError($"No model for {k.name}. Skipping.");
                    continue;
                }
                TaxonomicLevels lvl = TaxonomicLevels.Kingdom;
                GameObject model = getModel(m_string);

                // Error getting model from model string
                // Continue will skip this from data set
                if (model is null)
                {
                    Debug.LogError($"Error finding model {m_string} for {k.name}");
                    continue;
                }

                GameObject nObj = new GameObject();
                nObj.AddComponent<SpeciesManager>();
                nObj.name = k.name + " Manager";
                nObj.transform.SetParent(gameObject.transform);
                SpeciesManager manager = nObj.GetComponent<SpeciesManager>();

                if (k.maxDepth > maxDepth) maxDepth = k.maxDepth;
                // Current issue link to which level the model came from: Should be doable

                manager.Setup(k, null, null, null, null, null, null, sample_count, model, lvl);

                SpeciesControllers.Add(manager);

                continue; // No need to continue if model represents this group
            }

            foreach (Phylum p in k.Phyla)
            {
                if (!String.IsNullOrEmpty(p.model))
                {
                    string m_string = p.model;
                    if (m_string.Equals("None"))
                    {
                        Debug.LogError($"No model for {p.name}. Skipping.");
                        continue;
                    }
                    TaxonomicLevels lvl = TaxonomicLevels.Phylum;
                    GameObject model = getModel(m_string);

                    // Error getting model from model string
                    // Continue will skip this from data set
                    if (model is null)
                    {
                        Debug.LogError($"Error finding model {m_string} for {p.name}");
                        continue;
                    }

                    GameObject nObj = new GameObject();
                    nObj.AddComponent<SpeciesManager>();
                    nObj.name = p.name + " Manager";
                    nObj.transform.SetParent(gameObject.transform);
                    SpeciesManager manager = nObj.GetComponent<SpeciesManager>();

                    if (p.maxDepth > maxDepth) maxDepth = p.maxDepth;

                    // Current issue link to which level the model came from: Should be doable

                    manager.Setup(k, p, null, null, null, null, null, sample_count, model, lvl);

                    SpeciesControllers.Add(manager);

                    continue; // Do not proceed further down tree when model found
                }

                foreach (TaxonClass c in p.Classes)
                {
                    if (!String.IsNullOrEmpty(c.model))
                    {
                        string m_string = c.model;
                        if (m_string.Equals("None"))
                        {
                            Debug.LogError($"No model for {c.name}. Skipping.");
                            continue;
                        }
                        TaxonomicLevels lvl = TaxonomicLevels.Class;
                        GameObject model = getModel(m_string);

                        // Error getting model from model string
                        // Continue will skip this from data set
                        if (model is null)
                        {
                            Debug.LogError($"Error finding model {m_string} for {c.name}");
                            continue;
                        }

                        GameObject nObj = new GameObject();
                        nObj.AddComponent<SpeciesManager>();
                        nObj.name = c.name + " Manager";
                        nObj.transform.SetParent(gameObject.transform);
                        SpeciesManager manager = nObj.GetComponent<SpeciesManager>();

                        if (c.maxDepth > maxDepth) maxDepth = c.maxDepth;


                        // Current issue link to which level the model came from: Should be doable

                        manager.Setup(k, p, c, null, null, null, null, sample_count, model, lvl);

                        SpeciesControllers.Add(manager);

                        continue; // Do not proceed further down tree when model found
                    }

                    foreach (Order o in c.Orders)
                    {
                        if (!String.IsNullOrEmpty(o.model))
                        {
                            string m_string = o.model;
                            if (m_string.Equals("None"))
                            {
                                Debug.LogError($"No model for {o.name}. Skipping.");
                                continue;
                            }
                            TaxonomicLevels lvl = TaxonomicLevels.Order;
                            GameObject model = getModel(m_string);

                            // Error getting model from model string
                            // Continue will skip this from data set
                            if (model is null)
                            {
                                Debug.LogError($"Error finding model {m_string} for {o.name}");
                                continue;
                            }

                            GameObject nObj = new GameObject();
                            nObj.AddComponent<SpeciesManager>();
                            nObj.name = o.name + " Manager";
                            nObj.transform.SetParent(gameObject.transform);
                            SpeciesManager manager = nObj.GetComponent<SpeciesManager>();

                            if (o.maxDepth > maxDepth) maxDepth = o.maxDepth;

                            // Current issue link to which level the model came from: Should be doable

                            manager.Setup(k, p, c, o, null, null, null, sample_count, model, lvl);

                            SpeciesControllers.Add(manager);

                            continue; // Do not proceed further down tree when model found
                        }

                        foreach (Family f in o.Families)
                        {
                            if (!String.IsNullOrEmpty(f.model))
                            {
                                string m_string = f.model;
                                if (m_string.Equals("None"))
                                {
                                    Debug.LogError($"No model for {f.name}. Skipping.");
                                    continue;
                                }
                                TaxonomicLevels lvl = TaxonomicLevels.Family;
                                GameObject model = getModel(m_string);

                                // Error getting model from model string
                                // Continue will skip this from data set
                                if (model is null)
                                {
                                    Debug.LogError($"Error finding model {m_string} for {f.name}");
                                    continue;
                                }

                                GameObject nObj = new GameObject();
                                nObj.AddComponent<SpeciesManager>();
                                nObj.name = f.name + " Manager";
                                nObj.transform.SetParent(gameObject.transform);
                                SpeciesManager manager = nObj.GetComponent<SpeciesManager>();

                                if (f.maxDepth > maxDepth) maxDepth = f.maxDepth;


                                // Current issue link to which level the model came from: Should be doable

                                manager.Setup(k, p, c, o, f, null, null, sample_count, model, lvl);

                                SpeciesControllers.Add(manager);

                                continue; // Do not proceed further down tree when model found
                            }

                            foreach (Genus g in f.Genera)
                            {
                                if (!String.IsNullOrEmpty(g.model))
                                {
                                    string m_string = g.model;
                                    if (m_string.Equals("None"))
                                    {
                                        Debug.LogError($"No model for {g.name}. Skipping.");
                                        continue;
                                    }
                                    TaxonomicLevels lvl = TaxonomicLevels.Genus;
                                    GameObject model = getModel(m_string);

                                    // Error getting model from model string
                                    // Continue will skip this from data set
                                    if (model is null)
                                    {
                                        Debug.LogError($"Error finding model {m_string} for {g.name}");
                                        continue;
                                    }

                                    GameObject nObj = new GameObject();
                                    nObj.AddComponent<SpeciesManager>();
                                    nObj.name = g.name + " Manager";
                                    nObj.transform.SetParent(gameObject.transform);
                                    SpeciesManager manager = nObj.GetComponent<SpeciesManager>();

                                    if (g.maxDepth > maxDepth) maxDepth = g.maxDepth;

                                    // Current issue link to which level the model came from: Should be doable

                                    manager.Setup(k, p, c, o, f, g, null, sample_count, model, lvl);

                                    SpeciesControllers.Add(manager);

                                    continue; // Do not proceed further down tree when model found

                                }

                                foreach (Species s in g.Species)
                                {
                                    if (!String.IsNullOrEmpty(s.model))
                                    {
                                        string m_string = s.model;
                                        if (m_string.Equals("None"))
                                        {
                                            Debug.LogError($"No model for {s.name}. Skipping.");
                                            continue;
                                        }
                                        TaxonomicLevels lvl = TaxonomicLevels.Species;
                                        GameObject model = getModel(m_string);

                                        // Error getting model from model string
                                        // Continue will skip this from data set
                                        if (model is null)
                                        {
                                            Debug.LogError($"Error finding model {m_string} for {s.name}");
                                            continue;
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
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        SpawnPointManager.SetMaxDepth(maxDepth);

        // Add More error handling?
        // Trigger spawn when done
        //Loaded = true;
        //_loading = false;

        Debug.Log("Processing Done: " + Time.time);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cameraCanvas != null)
        {
            cameraCanvas.SetActive(true);
        }
        JSONDataManager = TaxonomyManager.Instance;
        if (SpawnPointManager is null) SpawnPointManager = FindFirstObjectByType<CreateSpawnPoints>();


        if (initialSpawn)
        {
            OnLoaded += Spawn;
        }
    }

    private void OnDestroy()
    {
        if (initialSpawn)
        {
            OnLoaded -= Spawn;
        }
    }

    private void Awake()
    {

    }

    // First time spawn only?
    void Spawn()
    {
        // Only trigger the spawn if we have loaded the data and
        // the current spawn is different from the new filter
        if (!_loaded) return;

        // For filtering actually avoid using SPAWN method:
        // Just set active / deactive as needed
        foreach (SpeciesManager m in SpeciesControllers)
        {
            m.Spawn(); // change this to a managed event call
                       // Start a coroutine within each manager
        }

        // trigger after processing all spawns --> Should the event pass the current filter?
        //current_spawned_filter = filter;
    }

    // Update is called once per frame
    void Update()
    {
        // If not done processing/loading data into this manager and JSON data and Model data is ready
        // Start the coroutine for loading
        if (!_loaded && !_loading && JSONDataManager.Loaded && JSONDataManager.ModelsReady)
        {
            _loading = true;
            // If JSONDataManager Loaded:
            _processingRequest = ProcessData();

            //StartCoroutine(ProcessData()); // Analyze if we need to yield return in other points
            // (currently spawn one species manager per iteration)
            Debug.Log("Processing Started: " + Time.time);
        }
        else if (_loading && !_processingRequest.IsCompleted)
        {
            Debug.Log($"Processing: {Time.deltaTime}");
        }
        else if (_loading && _processingRequest.IsCompleted)
        {
            _loading = false;
            Loaded = true;
            if (cameraCanvas != null)
            {
                cameraCanvas.SetActive(false);
            }
        }
        else if (Loaded && initialSpawn && !_spawned)
        {
            if (SpeciesControllers.All(m => m.spawned))
                _spawned = true;

            // Raise spawn event ONCE when filter changes and after everything loads for the first time
            if (!_spawned) Spawn(); // make this a coroutine too?
        }
    }

    private void OnUpdateDensity()
    {
        foreach (SpeciesManager c in SpeciesControllers)
        {
            c.RequiresRespawn = true;
        }
    }
    
    public void SetFilter(Filter newFilter)
    {
        switch (newFilter)
        {
            case null when filter is null:
                return;
            case null when filter is not null:
                break;
            case not null:
                Debug.Log($"Filter Changed: {newFilter.Equals(filter)}");
                if (newFilter.Equals(filter)) return; //do nothing if filter has not changed
                break;
        }

        Filter of = filter;

        filter = newFilter;

        OnFilterChanged?.Invoke(newFilter);
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

    public SpeciesManager FindSpeciesManager(string name, TaxonomicLevels level)
    {
        if (SpeciesControllers == null)
        {
            Debug.LogWarning("SpeciesControllers list is null.");
            return null;
        }

        foreach (var manager in SpeciesControllers)
        {
            if (manager == null) continue;

            // Match based on level and corresponding name
            switch (level)
            {
                case TaxonomicLevels.Kingdom:
                    if (manager.kingdom != null && manager.kingdom.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return manager;
                    break;
                case TaxonomicLevels.Phylum:
                    if (manager.phylum != null && manager.phylum.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return manager;
                    break;
                case TaxonomicLevels.Class:
                    if (manager.taxclass != null && manager.taxclass.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return manager;
                    break;
                case TaxonomicLevels.Order:
                    if (manager.order != null && manager.order.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return manager;
                    break;
                case TaxonomicLevels.Family:
                    if (manager.family != null && manager.family.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return manager;
                    break;
                case TaxonomicLevels.Genus:
                    if (manager.genus != null && manager.genus.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return manager;
                    break;
                case TaxonomicLevels.Species:
                    if (manager.SpeciesName.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return manager;
                    break;
            }
        }

        Debug.LogWarning($"No SpeciesManager found for name: {name} at level: {level}");
        return null;
    }


}