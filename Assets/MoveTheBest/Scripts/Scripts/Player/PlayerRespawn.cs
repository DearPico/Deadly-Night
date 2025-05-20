using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 lastCheckpoint;

    private void Start()
    {
        lastCheckpoint = transform.position; // Position de départ
    }

    public void SetCheckpoint(Vector3 pos)
    {
        lastCheckpoint = pos;
    }

    public void Die()
    {
        // Tu peux mettre un effet de mort ici
        transform.position = lastCheckpoint;
        // Tu peux aussi reset la vitesse, la vie, etc.
    }
}