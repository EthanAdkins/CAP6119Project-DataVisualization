using System.Drawing;
using Unity.Mathematics;
using UnityEngine;

public class FishingGameManager : MonoBehaviour
{
    [SerializeField] private GameObject _SpeciesPrefab;
    [SerializeField] private Transform _SpawnLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void caughtFish()
    {
        print("CAUGHT FISH");
        GameObject instance = Instantiate(_SpeciesPrefab, _SpawnLocation.position,
                    _SpawnLocation.rotation);
    }
}
