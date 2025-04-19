using System.Collections.Generic;
using System.Linq;
public class Filter
{
    public abstract class FilterComponent
    {
        public abstract bool Match(object specimen);
        public abstract bool Equals(FilterComponent other);
    }

    public class FilterTaxonComponent : FilterComponent
    {
        private TaxonomicLevels _taxon;
        private string _name;

        public FilterTaxonComponent(TaxonomicLevels lvl, string name)
        {
            _taxon = lvl;
            _name = name;
        }

        public override bool Match(object root)
        {
            return _taxon switch
            {
                TaxonomicLevels.Kingdom => root is Kingdom k && k.name.Equals(_name),
                TaxonomicLevels.Class => root is TaxonClass c && c.name.Equals(_name),
                TaxonomicLevels.Family => root is Family f && f.name.Equals(_name),
                TaxonomicLevels.Species => root is Species s && s.name.Equals(_name),
                TaxonomicLevels.Genus => root is Genus g && g.name.Equals(_name),
                TaxonomicLevels.Order => root is Order o && o.name.Equals(_name),
                TaxonomicLevels.Phylum => root is Phylum p && p.name.Equals(_name),
                _ => false
            };
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
    
    // Not all components are required (variable number)
    public Filter(params FilterComponent[] components)
    {
        foreach (FilterComponent c in components)
        {
            _components.Add(c);
        }
    }

    public bool Match(object root)
    {
        bool m = false;
        // at least one match??
        m = _components.All(c => c.Match(root));

        return m;
    }

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
