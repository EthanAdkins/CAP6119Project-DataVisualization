using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TaxonomyManager : MonoBehaviour
{
    public static TaxonomyManager Instance; // Only one instance (Singelton)
    public SpecimenData specimenData;

    public bool Loaded
    {
        get
        {
            return _loaded;
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

        LoadJsonData();
    }

    void LoadJsonData()
    {
        string file = Path.Combine(Application.streamingAssetsPath, "Data.json");

        if (File.Exists(file))
        {
            string text = File.ReadAllText(file);
            specimenData = JsonUtility.FromJson<SpecimenData>(text);
            Debug.Log("JSON Data Loaded Successfully.");
            
            _loaded = true;
        }
        else
        {
            Debug.LogError("JSON file not found at: " + file);
        }
    }
}
