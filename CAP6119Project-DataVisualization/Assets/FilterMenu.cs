using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FilterMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown taxonTypeDropdown;
    [SerializeField] private TMP_Dropdown taxonNameDropdown;

    private TaxonomyManager _dataManager;

    // private List<Filter.FilterComponent> _filterComps;
    
    // for now allow only one TaxonComp and one DepthComp (just until we have a basic filter working)
    private Filter.FilterTaxonComponent _fTaxonComp;

    private TaxonomicLevels _selectedLvl;

    private void Start()
    {
        _dataManager = TaxonomyManager.Instance;
    }

    public void OnTaxonTypeChanged(int value)
    {
        _dataManager ??= TaxonomyManager.Instance;
        if (_dataManager is null) return;

        if (!_dataManager.Loaded) return;
        
        var selected = taxonTypeDropdown.options[value];
        taxonNameDropdown.ClearOptions();
        List<String> taxonNames = new List<string>();
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
        }
        
        taxonNameDropdown.AddOptions(taxonNames);
    }

    public void OnTaxonNameSelected(int value)
    {
        string name = taxonTypeDropdown.options[value].text;
        
        // Add to selected filter
        Filter.FilterTaxonComponent newComp = new Filter.FilterTaxonComponent(_selectedLvl, name);

        //_filterComps.Add(newComp);
        _fTaxonComp = newComp;
    }
}
