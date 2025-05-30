using System.Collections;
using UnityEngine;

public class ThinkUI : MonoBehaviour
{
    public GameObject chercheSac;
    public GameObject chercheBouton;
    public GameObject chercheChien;

    private bool sacTrouve = false;
    private bool boutonTrouve = false;
    private bool chienTrouve = false;

    private Coroutine routineBulles;

    void Start()
    {
        routineBulles = StartCoroutine(RoutineAffichageBulles());
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter avec : {other.name} | tag = {other.tag}");

        if (other.CompareTag("Sac"))
        {
            sacTrouve = true;
            ResetBulles();
        }

        if (other.CompareTag("Bouton"))
        {
            boutonTrouve = true;
            ResetBulles();
        }

        if (other.CompareTag("Chien"))
        {
            chienTrouve = true;
            ResetBulles();
        }
    }

    IEnumerator RoutineAffichageBulles()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            AfficherBulle();
            yield return new WaitForSeconds(7f);
            chercheSac.SetActive(false);
            chercheBouton.SetActive(false);
            chercheChien.SetActive(false);
        }
    }

    void AfficherBulle()
    {
        Debug.Log($"AfficherBulle() | sacTrouve = {sacTrouve} | boutonTrouve = {boutonTrouve} | chienTrouve = {chienTrouve}");

        // Masquer toutes les bulles
        chercheSac.SetActive(false);
        chercheBouton.SetActive(false);
        chercheChien.SetActive(false);

        if (!sacTrouve)
        {
            Debug.Log("Affichage de la bulle : chercheSac");
            chercheSac.SetActive(true);
            FadeInBubble fade = chercheSac.GetComponent<FadeInBubble>();
            if (fade != null) fade.StartFadeIn(2f);
        }
        else if (!boutonTrouve)
        {
            Debug.Log("Affichage de la bulle : chercheBouton");
            chercheBouton.SetActive(true);
            FadeInBubble fade = chercheBouton.GetComponent<FadeInBubble>();
            if (fade != null) fade.StartFadeIn(2f);
        }
        else if (!chienTrouve)
        {
            Debug.Log("Affichage de la bulle : chercheChien");
            chercheChien.SetActive(true);
            FadeInBubble fade = chercheChien.GetComponent<FadeInBubble>();
            if (fade != null) fade.StartFadeIn(2f);
        }
        else
        {
            Debug.Log("Tous les objectifs trouvés !");
        }
    }

    void ResetBulles()
    {
        if (routineBulles != null)
        {
            StopCoroutine(routineBulles);
        }

        chercheSac.SetActive(false);
        chercheBouton.SetActive(false);
        chercheChien.SetActive(false);

        routineBulles = StartCoroutine(RoutineAffichageBulles());
    }
}
