using System.Collections;
using UnityEngine;

public class FishController : MonoBehaviour
{

    public BoxCollider fishingArea;
    public float speed = 3f;
    public float waitTime = 3f;

    private Vector3 targetPosition;

    [SerializeField] private FishingGameManager fishingGameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        while (true)
        {
            targetPosition = GetRandomPointInBounds();

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector3 GetRandomPointInBounds()
    {
        if (fishingArea == null) return transform.position;

        Vector3 center = fishingArea.bounds.center;
        Vector3 size = fishingArea.bounds.extents;

        float randomX = Random.Range(center.x - size.x, center.x + size.x);
        float randomZ = Random.Range(center.z - size.z, center.z + size.z);
        float y = transform.position.y;

        return new Vector3(randomX, y, randomZ);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bobber")
        {
            fishingGameManager.caughtFish();
        }
    }
}
