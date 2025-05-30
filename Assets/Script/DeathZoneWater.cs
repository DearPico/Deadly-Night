using UnityEngine;

public class DeathZoneWater : MonoBehaviour
{
    public Transform player;
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
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.position = playerSpawn.position;
            cc.enabled = true;
        }
        else
        {
            player.position = playerSpawn.position;
        }
    }
}