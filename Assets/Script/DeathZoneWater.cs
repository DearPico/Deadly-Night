using UnityEngine;

public class DeathZoneWater : MonoBehaviour
{
    private Transform player;
    public Transform playerSpawn;

    void Start()
    {

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && playerSpawn != null)
        {
            player.position = playerSpawn.position;

        }
    }
}
