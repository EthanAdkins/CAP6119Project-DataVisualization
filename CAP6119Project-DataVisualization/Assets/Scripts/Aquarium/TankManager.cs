using System.Collections;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    public GameObject aquarium;
    public GameObject originalTank;
    public Material sandMaterial;
    public float newWaterZ = 50f;
    public float newWaterX = 50f;
    private GameObject swimTank;
    private GameObject swimWater;
    private GameObject swimSand;

    void Start()
    {
        StartCoroutine(WaitThenDuplicate());
    }

    IEnumerator WaitThenDuplicate()
    {
        // Wait until models are loaded and tank is resized
        yield return new WaitForSeconds(40f);

        swimTank = Instantiate(originalTank);
        swimTank.SetActive(false);

        swimWater = swimTank.transform.Find("Water")?.gameObject;
        swimTank.name = originalTank.name + " (Swim)";
        swimWater.name = "Water (Swim)";

        float depth = swimWater.transform.localScale.y;
        swimWater.transform.localScale = new Vector3(newWaterX, depth, newWaterZ);
        swimWater.GetComponent<BoxCollider>().isTrigger = true;
        swimWater.GetComponent<MeshRenderer>().enabled = false;

        swimSand = swimWater.transform.Find("Sand")?.gameObject;
        swimSand.GetComponent<Renderer>().material = sandMaterial;

        Debug.Log("Duplicated tank");
    }

    public void EnterTank()
    {
    }

    public void ExitTank()
    {
    }
}
