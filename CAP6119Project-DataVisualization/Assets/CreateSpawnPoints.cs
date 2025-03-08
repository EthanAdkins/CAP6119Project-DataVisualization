using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class CreateSpawnPoints : MonoBehaviour
{
    public int TotalDistribution = 100;

    // private struct SpawnPoint
    // {
    //     private bool in_use;
    //     private Vector3 coord;
    // }
    
    // Somehow set the bounds 
    
    private Dictionary<Vector3, bool> SpawnPoints;

    //public float x_min, y_min, z_min = 0;

    //public float x_max, y_max, z_max = 10;

    public BoxCollider box;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Generate *Distr* number of spawn points
        // This will likely need to change for updated spawn method that considers depth and other properties

        SpawnPoints = new Dictionary<Vector3, bool>();

        while(SpawnPoints.Count < TotalDistribution * 2)
        {
            Vector3 point = CreateNewValidPoint();
            
            // before add we need to check if in 
            SpawnPoints.Add(point, false);
        }
        //generate more points than needed

    }

    public Vector3 GetSpawnPoint()
    {
        // Remove check for open points and don't pre-create points just create dynamically and store
        // Used locations
        IEnumerable<Vector3> open = SpawnPoints.Where(p => p.Value == false)
            .Select(p => p.Key);
        
        //since point creation is random just select first available
        try
        {
            Vector3 point = open.First();
            SpawnPoints[point] = true;
            return point;
        }
        catch
        {
            //create a new spawn point that meets requirements (useful for filter or when we use depth)
            // Need to repeat randomization until req are met unless we use some kind of zoning
            // and can just create a new within the right zone
            Vector3 point = CreateNewValidPoint();
            SpawnPoints.Add(point, true);
            return point;
        }
    }

    private Vector3 CreateNewValidPoint()
    {
        bool valid = false;
        Vector3 point = Vector3.zero;
        while (!valid)
        {
            Vector3 min = box.bounds.min;
            Vector3 max = box.bounds.max;
            
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            float z = Random.Range(min.z, max.z);
            point = new Vector3(x, y, z);

            if (!SpawnPoints.Keys.Contains(point)) valid = true;
        }

        return point;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
