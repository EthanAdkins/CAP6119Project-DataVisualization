using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Filter
{
    public abstract class FilterComponent
    {
        public abstract bool Match(object specimen);
        public abstract bool Equals(FilterComponent other);
    }

    /*public abstract class FilterExpressionComponent : FilterComponent
    {
        public abstract bool Match(object specimen);
    }

    public class FilterOperationComponent : FilterComponent
    {
        public enum Operation
        {
            AND = 0,
            OR = 1
        }

        private Operation _op;

        public FilterOperationComponent(Operation op = Operation.AND)
        {
            _op = op;
        }
        public override bool Equals(FilterComponent other)
        {
            if (other is FilterOperationComponent oc)
                return _op == oc._op;
            return false;
        }

        public bool Operate(bool v1, bool v2)
        {
            if (_op == Operation.OR) return v1 || v2;
            return v1 && v2;
        }
    }
    */

    public class FilterTaxonComponent : FilterComponent
    {
        private TaxonomicLevels _taxon;
        private string _name;

        public FilterTaxonComponent(TaxonomicLevels lvl, string name)
        {
            _taxon = lvl;
            _name = name;
        }
        
        // NOT robust enough if CHILD IN PARENT THEN TRUE
        public override bool Match(object root)
        {
            return FindTaxonNames(root).Contains(_name);
        }

        private List<string> FindTaxonNames(object root)
        {
            foreach (Kingdom k in TaxonomyManager.Instance.specimenData.Kingdoms)
            {
                if (root is Kingdom kr)
                {
                    if (kr.name == k.name)
                    {
                        List<string> retVal = new List<string>();
                        switch (_taxon)
                        {
                            case TaxonomicLevels.Kingdom: 
                                retVal.Add(kr.name); 
                                return retVal;
                                
                            case TaxonomicLevels.Phylum: 
                                retVal.AddRange(k.Phyla.Select(p => p.name)); 
                                return retVal;
                                
                            case TaxonomicLevels.Class :
                                retVal.AddRange(k.Phyla.SelectMany(p => p.Classes)
                                    .Select(c => c.name)); 
                                return retVal;
                            case TaxonomicLevels.Order:
                                retVal.AddRange(k.Phyla.SelectMany(p => p.Classes)
                                    .SelectMany(c => c.Orders)
                                    .Select(o => o.name)); 
                                return retVal;
                            case TaxonomicLevels.Family:
                                retVal.AddRange(k.Phyla.SelectMany(p => p.Classes)
                                    .SelectMany(c => c.Orders)
                                    .SelectMany(o => o.Families)
                                    .Select(f => f.name));
                                return retVal;
                            case TaxonomicLevels.Genus:
                                retVal.AddRange(k.Phyla.SelectMany(p => p.Classes)
                                    .SelectMany(c => c.Orders)
                                    .SelectMany(o => o.Families)
                                    .SelectMany(f => f.Genera)
                                    .Select(g => g.name));
                                return retVal;
                            case TaxonomicLevels.Species:
                                retVal.AddRange(k.Phyla.SelectMany(p => p.Classes)
                                    .SelectMany(c => c.Orders)
                                    .SelectMany(o => o.Families)
                                    .SelectMany(f => f.Genera)
                                    .SelectMany(g => g.Species)
                                    .Select(s => s.name));
                                return retVal;
                            default: 
                                return retVal;
                        }
                    }
                }
                else
                    foreach (Phylum p in k.Phyla)
                    {
                        if (root is Phylum pr)
                        {
                            if (pr.name == p.name)
                            {
                                List<string> retVal = new List<string>();
                                switch (_taxon)
                                {
                                    case TaxonomicLevels.Kingdom:
                                        retVal.Add(k.name);
                                        return retVal;

                                    case TaxonomicLevels.Phylum:
                                        retVal.Add(pr.name);
                                        return retVal;

                                    case TaxonomicLevels.Class:
                                        retVal.AddRange(pr.Classes.Select(c => c.name));
                                        return retVal;
                                    case TaxonomicLevels.Order:
                                        retVal.AddRange(pr.Classes.SelectMany(c => c.Orders)
                                            .Select(o => o.name));
                                        return retVal;
                                    case TaxonomicLevels.Family:
                                        retVal.AddRange(pr.Classes.SelectMany(c => c.Orders)
                                            .SelectMany(o => o.Families)
                                            .Select(f => f.name));
                                        return retVal;
                                    case TaxonomicLevels.Genus:
                                        retVal.AddRange(pr.Classes.SelectMany(c => c.Orders)
                                            .SelectMany(o => o.Families)
                                            .SelectMany(f => f.Genera)
                                            .Select(g => g.name));
                                        return retVal;
                                    case TaxonomicLevels.Species:
                                        retVal.AddRange(pr.Classes.SelectMany(c => c.Orders)
                                            .SelectMany(o => o.Families)
                                            .SelectMany(f => f.Genera)
                                            .SelectMany(g => g.Species)
                                            .Select(s => s.name));
                                        return retVal;
                                    default:
                                        return retVal;
                                }
                            }
                        }
                        else
                        {
                            foreach (TaxonClass c in p.Classes)
                            {
                                if (root is TaxonClass cr)
                                {
                                    if (c.name == cr.name)
                                    {
                                        List<string> retVal = new List<string>();
                                        switch (_taxon)
                                        {
                                            case TaxonomicLevels.Kingdom:
                                                retVal.Add(k.name);
                                                return retVal;

                                            case TaxonomicLevels.Phylum:
                                                retVal.Add(p.name);
                                                return retVal;

                                            case TaxonomicLevels.Class:
                                                retVal.Add(c.name);
                                                return retVal;
                                            case TaxonomicLevels.Order:
                                                retVal.AddRange(c.Orders.Select(o => o.name));
                                                return retVal;
                                            case TaxonomicLevels.Family:
                                                retVal.AddRange(c.Orders.SelectMany(o => o.Families)
                                                    .Select(f => f.name));
                                                return retVal;
                                            case TaxonomicLevels.Genus:
                                                retVal.AddRange(c.Orders.SelectMany(o => o.Families)
                                                    .SelectMany(f => f.Genera)
                                                    .Select(g => g.name));
                                                return retVal;
                                            case TaxonomicLevels.Species:
                                                retVal.AddRange(c.Orders.SelectMany(o => o.Families)
                                                    .SelectMany(f => f.Genera)
                                                    .SelectMany(g => g.Species)
                                                    .Select(s => s.name));
                                                return retVal;
                                            default:
                                                return retVal;
                                        }
                                    }
                                }
                                else
                                {
                                    //continue down
                                    foreach (Order o in c.Orders)
                                    {
                                        if (root is Order or)
                                        {
                                            if (o.name == or.name)
                                            {
                                                List<string> retVal = new List<string>();
                                                switch (_taxon)
                                                {
                                                    case TaxonomicLevels.Kingdom:
                                                        retVal.Add(k.name);
                                                        return retVal;

                                                    case TaxonomicLevels.Phylum:
                                                        retVal.Add(p.name);
                                                        return retVal;

                                                    case TaxonomicLevels.Class:
                                                        retVal.Add(c.name);
                                                        return retVal;
                                                    case TaxonomicLevels.Order:
                                                        retVal.Add(o.name);
                                                        return retVal;
                                                    case TaxonomicLevels.Family:
                                                        retVal.AddRange(o.Families.Select(f => f.name));
                                                        return retVal;
                                                    case TaxonomicLevels.Genus:
                                                        retVal.AddRange(o.Families.SelectMany(f => f.Genera)
                                                            .Select(g => g.name));
                                                        return retVal;
                                                    case TaxonomicLevels.Species:
                                                        retVal.AddRange(o.Families.SelectMany(f => f.Genera)
                                                            .SelectMany(g => g.Species)
                                                            .Select(s => s.name));
                                                        return retVal;
                                                    default:
                                                        return retVal;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (Family f in o.Families)
                                            {
                                                if (root is Family fr)
                                                {
                                                    if (f.name == fr.name)
                                                    {
                                                        List<string> retVal = new List<string>();
                                                        switch (_taxon)
                                                        {
                                                            case TaxonomicLevels.Kingdom:
                                                                retVal.Add(k.name);
                                                                return retVal;

                                                            case TaxonomicLevels.Phylum:
                                                                retVal.Add(p.name);
                                                                return retVal;

                                                            case TaxonomicLevels.Class:
                                                                retVal.Add(c.name);
                                                                return retVal;
                                                            case TaxonomicLevels.Order:
                                                                retVal.Add(o.name);
                                                                return retVal;
                                                            case TaxonomicLevels.Family:
                                                                retVal.Add(f.name);
                                                                return retVal;
                                                            case TaxonomicLevels.Genus:
                                                                retVal.AddRange(f.Genera.Select(g => g.name));
                                                                return retVal;
                                                            case TaxonomicLevels.Species:
                                                                retVal.AddRange(f.Genera.SelectMany(g => g.Species)
                                                                    .Select(s => s.name));
                                                                return retVal;
                                                            default:
                                                                return retVal;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (Genus g in f.Genera)
                                                    {
                                                        if (root is Genus gr)
                                                        {
                                                            if (g.name == gr.name)
                                                            {
                                                                List<string> retVal = new List<string>();
                                                                switch (_taxon)
                                                                {
                                                                    case TaxonomicLevels.Kingdom:
                                                                        retVal.Add(k.name);
                                                                        return retVal;

                                                                    case TaxonomicLevels.Phylum:
                                                                        retVal.Add(p.name);
                                                                        return retVal;

                                                                    case TaxonomicLevels.Class:
                                                                        retVal.Add(c.name);
                                                                        return retVal;
                                                                    case TaxonomicLevels.Order:
                                                                        retVal.Add(o.name);
                                                                        return retVal;
                                                                    case TaxonomicLevels.Family:
                                                                        retVal.Add(f.name);
                                                                        return retVal;
                                                                    case TaxonomicLevels.Genus:
                                                                        retVal.Add(g.name);
                                                                        return retVal;
                                                                    case TaxonomicLevels.Species:
                                                                        retVal.AddRange(g.Species.Select(s => s.name));
                                                                        return retVal;
                                                                    default:
                                                                        return retVal;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            foreach (Species s in g.Species)
                                                            {
                                                                if (root is Species sr)
                                                                {
                                                                    if (s.name == sr.name)
                                                                    {
                                                                        List<string> retVal = new List<string>();
                                                                        switch (_taxon)
                                                                        {
                                                                            case TaxonomicLevels.Kingdom:
                                                                                retVal.Add(k.name);
                                                                                return retVal;

                                                                            case TaxonomicLevels.Phylum:
                                                                                retVal.Add(p.name);
                                                                                return retVal;

                                                                            case TaxonomicLevels.Class:
                                                                                retVal.Add(c.name);
                                                                                return retVal;
                                                                            case TaxonomicLevels.Order:
                                                                                retVal.Add(o.name);
                                                                                return retVal;
                                                                            case TaxonomicLevels.Family:
                                                                                retVal.Add(f.name);
                                                                                return retVal;
                                                                            case TaxonomicLevels.Genus:
                                                                                retVal.Add(g.name);
                                                                                return retVal;
                                                                            case TaxonomicLevels.Species:
                                                                                retVal.Add(s.name);
                                                                                return retVal;
                                                                            default:
                                                                                return retVal;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
            }
            
            //return empty list if not found
            return new List<string>();
        }
        
        public override bool Equals(FilterComponent other)
        {
            if (other is FilterTaxonComponent ot)
            {
                return (_taxon == ot._taxon) && _name.Equals(ot._name);
            }
            
            return false;
        }
    }

    public class FilterDepthComponent : FilterComponent
    {
        private int _minDepth;
        private int _maxDepth;
        
        public FilterDepthComponent(int minDepth, int maxDepth)
        {
            _minDepth = minDepth;
            _maxDepth = maxDepth;
        }

        public override bool Match(object specimen)
        {
            if (_minDepth > _maxDepth) return false;
            return specimen switch
            {
                Kingdom speci => speci.minDepth >= _minDepth && speci.minDepth <= _maxDepth &&
                                 speci.maxDepth <= _maxDepth,
                Phylum speci => speci.minDepth >= _minDepth && speci.minDepth <= _maxDepth &&
                                speci.maxDepth <= _maxDepth,
                Order speci => speci.minDepth >= _minDepth && speci.minDepth <= _maxDepth &&
                               speci.maxDepth <= _maxDepth,
                Genus speci => speci.minDepth >= _minDepth && speci.minDepth <= _maxDepth &&
                               speci.maxDepth <= _maxDepth,
                TaxonClass speci => speci.minDepth >= _minDepth && speci.minDepth <= _maxDepth &&
                                    speci.maxDepth <= _maxDepth,
                Family speci => speci.minDepth >= _minDepth && speci.minDepth <= _maxDepth &&
                                speci.maxDepth <= _maxDepth,
                Species speci => speci.minDepth >= _minDepth && speci.minDepth <= _maxDepth &&
                                 speci.maxDepth <= _maxDepth,
                _ => false
            };
        }
        
        public override bool Equals(FilterComponent other)
        {
            if (other is FilterDepthComponent od)
            {
                return _minDepth == od._minDepth && _maxDepth == od._maxDepth;
            }
            
            return false;
        }
    }

    // What does filtering by count entail
    public class FilterObservationCount : FilterComponent
    {
        public enum ObservationCountFilterType
        {
            Max = 0,
            Min = 1
        }

        private int _count;
        private ObservationCountFilterType _type;

        public FilterObservationCount(int count, ObservationCountFilterType filterType = ObservationCountFilterType.Max)
        {
            _count = count;
            _type = filterType;
        }

        public override bool Match(object specimen)
        {
            if (_type == ObservationCountFilterType.Max)
            {
                return specimen switch
                {
                    Kingdom speci => speci.count <= _count,
                    Phylum speci => speci.count <= _count,
                    Order speci => speci.count <= _count,
                    Genus speci => speci.count <= _count,
                    TaxonClass speci => speci.count <= _count,
                    Family speci => speci.count <= _count,
                    Species speci => speci.count <= _count,
                    _ => false
                };
            }

            return specimen switch
            {
                Kingdom speci => speci.count >= _count,
                Phylum speci => speci.count >= _count,
                Order speci => speci.count >= _count,
                Genus speci => speci.count >= _count,
                TaxonClass speci => speci.count >= _count,
                Family speci => speci.count >= _count,
                Species speci => speci.count >= _count,
                _ => false
            };
        }
        
        public override bool Equals(FilterComponent other)
        {
            if (other is FilterObservationCount oc)
            {
                return _count == oc._count && _type == oc._type;
            }
            
            return false;
        }
    }
    private List<FilterComponent> _components = new List<FilterComponent>();
    //private Stack<FilterComponent> _components = new Stack<FilterComponent>();
    
    // Not all components are required (variable number)
    public Filter(params FilterComponent[] components)
    {
        foreach (FilterComponent c in components)
        {
            _components.Add(c);
            //_components.Push(c);
        }
    }

    /*public Filter(Stack<FilterComponent> s)
    {
        _components = CopyStack(s);
    }

    private Stack<FilterComponent> CopyStack(Stack<FilterComponent> s)
    {
        FilterComponent[] arr = new FilterComponent[s.Count];
        s.CopyTo(arr, 0);
        Array.Reverse(arr);
        return new Stack<FilterComponent>(arr);
    }*/

    public bool Match(object root)
    {
        bool m = false;
        //Stack<FilterComponent> s = CopyStack(_components);
        //m = Evaluate(ref s, root);
        
        // AND - ALL MUST MATCH
        m = _components.All(c => c.Match(root));

        return m;
    }
    //
    // private bool Evaluate(ref Stack<FilterComponent> s, object root)
    // {
    //     if (!s.TryPop(out var t)) throw new InvalidExpressionException("Filter Expression Invalid");
    //     if (t is FilterExpressionComponent e) return e.Match(root);
    //
    //     var v2 = Evaluate(ref s, root);
    //     var v1 = Evaluate(ref s, root);
    //     return (t as FilterOperationComponent).Operate(v1, v2);
    //     
    // }

    public bool Equals(Filter other)
    {
        if (other is null)
        {
            return _components.Count == 0;
        }
        
        // Check lists are same length
        // and then check for a counterpart for each component
        if (_components.Count != other._components.Count) return false;

        bool match = true;
        foreach (FilterComponent c in _components)
        {
            // search for an equivalent in other
            if (!other._components.Any(comp => comp.Equals(c)))
            {
                match = false;
                break;
            }
        }

        return match;
    }

    public static bool Equals(Filter f1, Filter f2)
    {
        if (f1 is null && f2 is null) return true;

        if (f1 is null) return f2._components.Count == 0;
        if (f2 is null) return f1._components.Count == 0;
        
        if (f1._components.Count != f2._components.Count) return false;

        foreach (FilterComponent c in f1._components)
        {
            // search for an equivalent in other
            if (!f2._components.Any(comp => comp.Equals(c)))
            {
                return false;
            }
        }

        return true;
    }
}
