using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Tilemaps;

public class TaxonomyManager : MonoBehaviour
{
    public static TaxonomyManager Instance; // Only one instance (Singelton)
    public SpecimenData specimenData;
    
    //private bool _modelsLoading;
    private bool _modelsReady = false;
    public bool ModelsReady
    {
        get => _modelsReady;
        private set => _modelsReady = value;
    }

    private AssetBundle _modelPrefabs;
    private AssetBundleCreateRequest _loadRequest;

    public delegate void LoadedAction();

    public static event LoadedAction OnLoad;

    public bool Loaded
    {
        get
        {
            return _loaded;
        }
        private set
        {
            _loaded = value;
            if (_loaded && OnLoad != null)
                OnLoad();
        }
    }

    private bool _loaded = false;
    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (!Loaded)
            LoadJsonData();
        if (!ModelsReady)
            LoadPrefabs();
    }

    void LoadJsonData()
    {
        string file = Path.Combine(Application.streamingAssetsPath, "Data.json");

        if (File.Exists(file))
        {
            string text = File.ReadAllText(file);
            specimenData = JsonUtility.FromJson<SpecimenData>(text);
            Debug.Log("JSON Data Loaded Successfully.");

            Loaded = true;
        }
        else
        {
            Debug.LogError("JSON file not found at: " + file);
        }
    }
    
    public GameObject LoadPrefabFromString(string s)
    {
        if (_modelPrefabs is null) throw new System.ComponentModel.InvalidAsynchronousStateException("Assets Not Loaded");
        if (string.IsNullOrEmpty(s))
        {
            throw new System.ArgumentException("Model String cannot be null or empty - please pass a model file");
        }

        var loadedObject = _modelPrefabs.LoadAsset<GameObject>(s);
        if (loadedObject is null)
        {
            throw new FileNotFoundException("no file found - please check the configuration");
        }
        return loadedObject;
    }
    
    
    void LoadPrefabs()
    {
        //_modelsLoading = true;
        // if loaded return 
        if (ModelsReady) return;
        
        // If somehow loaded in memory and LoadPrefabs is called set _modelPrefabs to the loaded instance
        if (AssetBundle.GetAllLoadedAssetBundles().Any(bundle => bundle.name.Equals("specimenmodels")))
        {
            _modelPrefabs = AssetBundle.GetAllLoadedAssetBundles()
                .First(bundle => bundle.name.Equals("specimenmodels"));
            ModelsReady = true;
            
            return;
        }

        string assetsFile = Path.Combine(Application.streamingAssetsPath, "AssetBundles/specimenmodels");
 
        Debug.Log("Start Model Loading: " + Time.time);
        
        _loadRequest = AssetBundle.LoadFromFileAsync(assetsFile);

        _loadRequest.completed += HandleAssetBundleLoaded;
        
        
    }

    private void HandleAssetBundleLoaded(AsyncOperation op)
    {
        if (!_loadRequest.isDone) return;
        
        _modelPrefabs = _loadRequest.assetBundle;
        if (_modelPrefabs is null)
        {
            Debug.LogError("Failed To Load Model Prefab Asset Bundle");
        }

        ModelsReady = true;
        
        Debug.Log("Models Loaded: " + Time.time);
        
       

        _loadRequest.completed -= HandleAssetBundleLoaded;
    }

    private void OnDestroy()
    {
        if (_loadRequest is not null)
        {
            _loadRequest.completed -= HandleAssetBundleLoaded;
        }
    }
}
