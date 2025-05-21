using UnityEngine;

public class GrueScript : MonoBehaviour
{
    public GameObject grue;
    public float vitesseRotation;
    private bool boutonCheck = false;
    private float angleCibleY = 0f;

    void Update()
    {
        if (boutonCheck)
        {
            
            Quaternion rotationCible = Quaternion.Euler(0f, angleCibleY, 0f);

            grue.transform.rotation = Quaternion.RotateTowards(
                grue.transform.rotation,
                rotationCible,
                vitesseRotation * Time.deltaTime
                
            );
           

            if (Quaternion.Angle(grue.transform.rotation, rotationCible) < 0.1f)
            {
                grue.transform.rotation = rotationCible;
                boutonCheck = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boutonCheck = true;
            Debug.Log("Début de la rotation");
        }
    }
}