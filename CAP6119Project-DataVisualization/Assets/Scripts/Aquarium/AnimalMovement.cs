using System.Collections;
using UnityEngine;
public class AnimalMovement : MonoBehaviour
{
    public float moveSpeed = 0.75f;
    public float rotationSpeed = 2.0f; 
    public float buffer = 0.1f; // Buffer between allowable target positions and the bounds of the water object
    public float verticalDrift = 1.0f; // Distance animal can travel from initial depth level
    public GameObject waterObject;

    private Bounds waterBounds;
    private Vector3 targetPosition; 
    private Vector3 startPosition;
    private Quaternion targetRotation;
    private Quaternion offset;
    private float rotationTime; 
    private float moveDuration;
    private float modelXZRadius;
    private float modelYRadius;
    private float initialY;

    private void Awake()
    {
        initialY = transform.position.y;
        offset = Quaternion.Euler(0, -90, 0); // Correct rotation so that animal faces direction it is moving in
        Bounds combinedBounds;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 extents = combinedBounds.extents;
            modelXZRadius = Mathf.Max(extents.x, extents.z);
            modelYRadius = extents.y;
        }
    }
    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "AquariumScene")
        {
            this.enabled = false;
            return;
        }

        // Find waterObject, if not found, disable script
        if (waterObject == null)
        {
            waterObject = GameObject.FindWithTag("AquariumWater");
        }
        if (waterObject != null)
        {
            waterBounds = GetObjectBounds(waterObject);
        }
        else
        {
            this.enabled = false;
            return;
        }

        PickNewDestination();
        StartCoroutine(MovementRoutine());       
    }

    Bounds GetObjectBounds(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }

        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds;
        }

        return new Bounds(obj.transform.position, Vector3.zero);
    }

    void PickNewDestination()
    {
        startPosition = transform.position;
        float totalBuffer = buffer + (modelXZRadius);
        float minY = Mathf.Max(waterBounds.min.y + modelYRadius, initialY - verticalDrift);
        float maxY = Mathf.Min(waterBounds.max.y - modelYRadius, initialY + verticalDrift);

        // X and Z targets can be anywhere in bounds, Y target must be within certain range to preserve depth level
        targetPosition = new(
                Random.Range(waterBounds.min.x + totalBuffer, waterBounds.max.x - totalBuffer),
                Random.Range(minY, maxY),
                Random.Range(waterBounds.min.z + totalBuffer, waterBounds.max.z - totalBuffer)
        );

        Vector3 directionToTarget = (targetPosition - transform.position);
        directionToTarget.y = 0; // Only rotate along y axis
        directionToTarget = directionToTarget.normalized;
        targetRotation = Quaternion.LookRotation(directionToTarget);

        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

        rotationTime = angleDifference / rotationSpeed;

        float distance = Vector3.Distance(transform.position, targetPosition);

        moveDuration = distance / moveSpeed;
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            PickNewDestination();

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                float t = elapsed / moveDuration;
                float eased = Mathf.SmoothStep(0f, 1f, t);

                // Move smoothly towards the target 
                transform.position = Vector3.Lerp(startPosition, targetPosition, eased);

                // Smoothly rotate toward target over time; snap to final rotation if exceeded
                if (elapsed < rotationTime * 1.2f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation * offset, elapsed / rotationTime);
                }
                else
                {
                    transform.rotation = targetRotation * offset;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            transform.rotation = targetRotation * offset;

            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }
}
