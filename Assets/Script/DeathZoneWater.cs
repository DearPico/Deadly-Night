using UnityEngine;
using UnityEngine.SceneManagement;


public class DeathZoneWater : MonoBehaviour
{
    public GameObject player;
    public GameObject playerSpawn;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.transform.position = playerSpawn.transform.position;
            Debug.Log("Player mort"); 
        }
    }
}
