using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class FilterMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown taxonTypeDropdown;
    [SerializeField] private TMP_Dropdown taxonNameDropdown;
    [SerializeField] private TMP_InputField filterCountInputField;
    [SerializeField] private SliderValueController maxDepthSlider;
    [SerializeField] private SliderValueController minDepthSlider;


    private TaxonomyManager _dataManager;
    [SerializeField] private SpecimenDataManager _specMan;

    // private List<Filter.FilterComponent> _filterComps;
    
    // for now allow only one TaxonComp and one DepthComp (just until we have a basic filter working)
    private Filter.FilterTaxonComponent _fTaxonComp;
    private Filter.FilterObservationCount _ocFilter;
    private Filter.FilterDepthComponent _dFilter;

    private int _ocFilterCount;
    private float _maxDepth;
    private float _selectedMinDepth;
    private float _selectedMaxDepth;


    private TaxonomicLevels _selectedLvl;

    private void Start()
    {
        _dataManager = TaxonomyManager.Instance;
        // Set the Max Depth fields for sliders -- Check the SpecimenDataManager?
        if (_dataManager.Loaded)
        {
            FindMaxDepth();   
        }
        
        // We may want to assign this differently when we link up the scenes: perhaps manual assignment would be best
        // To allow for merging scenes and having two of these OR to find and reset to only the active one on scene change?
        _specMan ??= FindFirstObjectByType<SpecimenDataManager>();

        minDepthSlider.slider.onValueChanged.AddListener(OnMinDepthChanged);
        maxDepthSlider.slider.onValueChanged.AddListener(OnMaxDepthChanged);
        filterCountInputField.onValueChanged.AddListener(OnCountFilterInputChanged);
    }

    void OnDestroy()
    {
        minDepthSlider.slider.onValueChanged.RemoveListener(OnMinDepthChanged);
        maxDepthSlider.slider.onValueChanged.RemoveListener(OnMaxDepthChanged);
        filterCountInputField.onValueChanged.RemoveListener(OnCountFilterInputChanged);
    }
    private void Update()
    {
        if (_maxDepth > 0) return;
        if (_dataManager.Loaded) FindMaxDepth();
    }

    private void FindMaxDepth()
    {
        float md = 0;
        foreach (Kingdom k in _dataManager.specimenData.Kingdoms)
        {
            md = Math.Max(md, k.maxDepth);
        }

        _maxDepth = md;
        minDepthSlider.UpdateMaxValue(md);
        maxDepthSlider.UpdateMaxValue(md);
        maxDepthSlider.slider.value = md;

        _dFilter = new Filter.FilterDepthComponent(0, (int)md);
    }

    public void OnTaxonTypeChanged(int value)
    {
        _dataManager ??= TaxonomyManager.Instance;
        if (_dataManager is null) return;

        if (!_dataManager.Loaded) return;
        
        var selected = taxonTypeDropdown.options[value];
        taxonNameDropdown.ClearOptions();
        List<String> taxonNames = new List<string>();
        taxonNames.Add(""); // Add empty to clear filter
        switch (selected.text)
        {
            case "Kingdom":
                taxonNames.AddRange(_dataManager.specimenData.Kingdoms.Select(k => k.name));
                _selectedLvl = TaxonomicLevels.Kingdom;
                break;
            case "Phylum":
                taxonNames.AddRange(_dataManager.specimenData.Kingdoms.SelectMany(k => k.Phyla)
                    .Select(p => p.name));
                _selectedLvl = TaxonomicLevels.Phylum;
                break;
            case "Class":
                taxonNames.AddRange(_dataManager.specimenData.Kingdoms.SelectMany(k => k.Phyla)
                    .SelectMany(p => p.Classes)
                    .Select(c => c.name));
                _selectedLvl = TaxonomicLevels.Class;
                break;
            case "Order":
                taxonNames.AddRange(_dataManager.specimenData.Kingdoms.SelectMany(k => k.Phyla)
                    .SelectMany(p => p.Classes)
                    .SelectMany(c => c.Orders)
                    .Select(o => o.name));
                _selectedLvl = TaxonomicLevels.Order;
                break;
            case "Family":
                taxonNames.AddRange(_dataManager.specimenData.Kingdoms.SelectMany(k => k.Phyla)
                    .SelectMany(p => p.Classes)
                    .SelectMany(c => c.Orders)
                    .SelectMany(o => o.Families)
                    .Select(f => f.name));
                _selectedLvl = TaxonomicLevels.Family;
                break;
            case "Genus":
                taxonNames.AddRange(_dataManager.specimenData.Kingdoms.SelectMany(k => k.Phyla)
                    .SelectMany(p => p.Classes)
                    .SelectMany(c => c.Orders)
                    .SelectMany(o => o.Families)
                    .SelectMany(f => f.Genera)
                    .Select(g => g.name));
                _selectedLvl = TaxonomicLevels.Genus;
                break;
            case "Species":
                taxonNames.AddRange(_dataManager.specimenData.Kingdoms.SelectMany(k => k.Phyla)
                    .SelectMany(p => p.Classes)
                    .SelectMany(c => c.Orders)
                    .SelectMany(o => o.Families)
                    .SelectMany(f => f.Genera)
                    .SelectMany(g => g.Species)
                    .Select(s => s.name));
                _selectedLvl = TaxonomicLevels.Species;
                break;
            case "":
                _fTaxonComp = null;
                break;
        }
        
        taxonNameDropdown.AddOptions(taxonNames);
    }

    public void OnTaxonNameSelected(int value)
    {
        string name = taxonNameDropdown.options[value].text;

        if (string.IsNullOrEmpty(name))
        {
            _fTaxonComp = null;
            return;
        }

        // Add to selected filter
        var newComp = new Filter.FilterTaxonComponent(_selectedLvl, name);
        Debug.Log($"Filtering to {_selectedLvl.ToString()} {name}");
        // -- For now basic One taxon comp for filter --
        // TODO: Enable advanced filtering with multiple components linked via AND and OR

        //_filterComps.Add(newComp);
        _fTaxonComp = newComp;
    }
    
    /// <summary>
    /// Sets the Count Filter component: when Count of 0 is selected remove the filter
    /// </summary>
    /// <param name="input">Value for Count filter</param>
    void OnCountFilterInputChanged(string input)
    {
        // TODO: Enable Selection of > or <
        if (int.TryParse(input, out int filterCount))
        {
            _ocFilterCount = filterCount;
            Filter.FilterObservationCount newOC = new Filter.FilterObservationCount(filterCount);
            _ocFilter = newOC;
        }
        else
        {
            Debug.LogWarning("Invalid number entered for filter count.");
            filterCountInputField.text = _ocFilterCount.ToString(); // Reset to previous valid value
        }
    }

    void OnMinDepthChanged(float value)
    {
        _selectedMinDepth = value;

        _dFilter = new Filter.FilterDepthComponent((int)_selectedMinDepth, (int)_selectedMaxDepth);
    }

    void OnMaxDepthChanged(float value)
    {
        _selectedMaxDepth = value;
        
        _dFilter = new Filter.FilterDepthComponent((int)_selectedMinDepth, (int)_selectedMaxDepth);
        Debug.Log($"Filtering to Min Depth of {_selectedMinDepth} and Max Depth of {_selectedMaxDepth}");

    }

    public void ApplyFilter()
    {
        // Pass only the non-null components:
        List<Filter.FilterComponent> param = new List<Filter.FilterComponent>();
        if (_fTaxonComp != null) param.Add(_fTaxonComp);
        if (_dFilter != null) param.Add(_dFilter);
        if (_ocFilter != null) param.Add(_ocFilter); 
        
        Debug.Log($"Applying {param.Count} filters");
        
        Filter f = new Filter(param.ToArray());

        _specMan.SetFilter(f);
    }
}
