using UnityEngine;

public class Sac : MonoBehaviour
{

    public Sprite[] sprites;
    private bool sacCheck = false;

    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sac")) // Vérifie si l'objet touché est un missile
        {
            ChangeSprite();
            Destroy(other.gameObject); // Détruit le missile après collision
            sacCheck = true;
        }
    }


    void ChangeSprite()
    {
        if (sacCheck) 
        {
       //     spriteRenderer.sprite = newSprite; // Change le sprite
        }
    }
}