using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class CreateSpawnPoints : MonoBehaviour
{
    public int TotalDistribution = 100;
    
    private List<Vector3> SpawnPointsInUse;

    public BoxCollider box;
    public GameObject waterObject;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnPointsInUse = new List<Vector3>();
    }

    public void SetMaxDepth(float depth)
    {
        if (waterObject != null)
        {
            waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, depth, waterObject.transform.localScale.z);
            waterObject.transform.position = new Vector3(waterObject.transform.position.x, depth / 2, waterObject.transform.position.z);
        }
        else if (box != null)
        {
            Vector3 oldSize = box.size;
            Vector3 newSize = new Vector3(oldSize.x, depth, oldSize.z);

            Vector3 oldCenter = box.center;
            Vector3 newCenter = new Vector3(oldCenter.x, depth / 2, oldCenter.z);

            box.size = newSize;
            box.center = newCenter;
        }
    }

    public Vector3 GetSpawnPoint(float minDepth, float maxDepth)
    {
        Vector3 point = CreateNewValidPoint(minDepth, maxDepth);
        SpawnPointsInUse.Add(point);
        return point;
    }

    private Vector3 CreateNewValidPoint(float minDepth, float maxDepth)
    {
        bool valid = false;
        Vector3 point = Vector3.zero;
        while (!valid)
        {
            // Bounds are from top - minDepth to top - maxDepth
            // I believe this is box.bounds.max.y == top of box
            
            Vector3 min = box.bounds.min;
            Vector3 max = box.bounds.max;

            // ensure we dont go out of bounds of environ (will want to make sure we set the bounds after loading data
            // to ensure bounds go from globalMinDepth to globalMaxDepth) but keep this as failsafe
            float min_y = Math.Max(max.y - maxDepth, min.y);
            float max_y = max.y - minDepth;
            
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min_y, max_y);
            float z = Random.Range(min.z, max.z);
            point = new Vector3(x, y, z);

            if (!SpawnPointsInUse.Contains(point)) valid = true;
        }

        return point;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
