using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Bitgem.VFX.StylisedWater;

public class CreateSpawnPoints : MonoBehaviour
{
    public int TotalDistribution = 100;
    
    private List<Vector3> SpawnPointsInUse;

    private Vector3 buffer = new Vector3(1, 1, 1);

    public BoxCollider box;
    public Renderer rend;
    public GameObject waterObject;
    public GameObject marker;
    public GameObject tankObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnPointsInUse = new List<Vector3>();
    }

    public void SetMaxDepth(float depth)
    {
        if (waterObject != null && tankObject != null)
        {
            
            rend = waterObject.GetComponent<Renderer>();

            // Set new height 
            float oldDepth = marker.transform.localScale.y;
            marker.transform.localScale = new Vector3(
                marker.transform.localScale.x,
                depth,
                marker.transform.localScale.z
            );

            waterObject.GetComponent<WaterVolumeTransforms>().MarkDirty();

            // Lock the top of the water (so if user scrolls before this function is called)
            // Move tank so top stays the same
            tankObject.transform.position = new Vector3(
                tankObject.transform.position.x,
                tankObject.transform.position.y - (depth - oldDepth), // assume depth > olddepth
                tankObject.transform.position.z
            );

            BoxCollider tankObjectBox = tankObject.GetComponent<BoxCollider>();
            tankObjectBox.size = new Vector3(
                tankObjectBox.size.x,
                depth,
                tankObjectBox.size.z
            );
            tankObjectBox.center = new Vector3(
                tankObjectBox.center.x,
                (depth / 2) - (oldDepth / 2),
                tankObjectBox.center.z
            );
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

    public Vector3 GetSpawnPoint(float minDepth, float maxDepth, GameObject model)
    {
        var modelExtents = GetModelExtents(model);
        Vector3 point = CreateNewValidPoint(minDepth, maxDepth, modelExtents);
        SpawnPointsInUse.Add(point);
        return point;
    }

    private Vector3 GetModelExtents(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            var combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 extents = combinedBounds.extents;
            return extents;
            //modelXZRadius = Mathf.Max(extents.x, extents.z);
            //modelYRadius = extents.y;
        }

        return Vector3.zero;
    }

    private Vector3 CreateNewValidPoint(float minDepth, float maxDepth, Vector3 modelExtents)
    {
        var totalBuffer = buffer + modelExtents;
        bool valid = false;
        Vector3 point = Vector3.zero;
        while (!valid)
        {
            // Bounds are from top - minDepth to top - maxDepth
            // I believe this is box.bounds.max.y == top of box
            
            Vector3 min = box.bounds.min;
            Vector3 max = box.bounds.max;

            if (waterObject != null)
            {
                min = rend.bounds.min;
                max = rend.bounds.max;
            }

            // ensure we dont go out of bounds of environ (will want to make sure we set the bounds after loading data
            // to ensure bounds go from globalMinDepth to globalMaxDepth) but keep this as failsafe
            float min_y = Math.Max(max.y - maxDepth, min.y);
            float max_y = max.y - minDepth;

            float x = Random.Range(min.x + totalBuffer.x, max.x - totalBuffer.x);
            float y = Random.Range(min_y + totalBuffer.y, max_y - totalBuffer.y);
            float z = Random.Range(min.z + totalBuffer.z, max.z - totalBuffer.z);
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
