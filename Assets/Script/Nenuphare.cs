using UnityEngine;

public class Nenuphare : MonoBehaviour
{
    private bool shouldDrown = false;
    public float sinkSpeed;

    void Update()
    {
        if (shouldDrown)
        {
            transform.position += Vector3.down * sinkSpeed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldDrown = true;
        }
    }

    void OnBecameInvisible()
    {
        if (shouldDrown)
        {
            Destroy(gameObject);
        }
    }
}