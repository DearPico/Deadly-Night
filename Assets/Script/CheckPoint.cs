using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Transform playerSpawn;

    void Awake()
    {
        GameObject spawnObj = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (spawnObj != null)
        {
            playerSpawn = spawnObj.transform;
        }
        else
        {
            Debug.LogWarning("PlayerSpawn not found in scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerSpawn != null)
        {
            playerSpawn.position = transform.position;
        }
    }
}

