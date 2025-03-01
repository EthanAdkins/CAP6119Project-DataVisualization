using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collision)
    {
        print(collision.gameObject.tag);
        if (collision.gameObject.tag == "player")
        {
            print("LOADING SCENE");
            SceneManager.LoadScene("MainScene");
        }
    }
}
