using System;
using System.Collections.Generic;

[Serializable]
public class SpecimenData
{
    // Total Number of Specimen in DataSet
    public int totalCount;
    // Number of Kingdoms in DataSet
    public int numKingdoms;
    // List of Kingdoms in Dataset
    public List<Kingdom> Kingdoms;
}

[Serializable]
public class Kingdom
{
    public string name;
    public string model;
    public int count;
    public int numPhyla;
    public List<Phylum> Phyla;
}

[Serializable]
public class Phylum
{
    public string name;
    public string model;
    public int count;
    public int numClasses;
    public List<TaxonClass> Classes;
}

[Serializable]
public class TaxonClass
{
    public string name;
    public string model;
    public int count;
    public int numOrders;
    public List<Order> Orders;
}

[Serializable]
public class Order
{
    public string name;
    public string model;
    public int count;
    public int numFamilies;
    public List<Family> Families;
}

[Serializable]
public class Family
{
    public string name;
    public string model;
    public int count;
    public int numGenera;
    public List<Genus> Genera;
}

[Serializable]
public class Genus
{
    public string name;
    public string model;
    public int count;
    public int numSpecies;
    public List<Species> Species;
}

[Serializable]
public class Species
{
    public string name;
    public string commonName;
    public string model;
    public int count;
    public bool sedentary;
    public float maxDepth;
    public float minDepth;
}

public enum TaxonomicLevels
{
    Kingdom,
    Phylum,
    Class,
    Order,
    Family,
    Genus,
    Species
}
