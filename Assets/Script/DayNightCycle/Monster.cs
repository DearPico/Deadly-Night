using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Monster : MonoBehaviour
{
    [SerializeField]
    private bool dead = false;

    public void OnDayBegins()
    {
        gameObject.SetActive(false);
    }

    public void OnNightBegins()
    {
        gameObject.SetActive(true);
    }

    public void OnCycleUpdate(DayNightCycleController controller)
    {
        if (dead)
        {
            SceneManager.LoadScene("Death");
        }
    }

    void OnTriggerEnter(Collider other)
    {

        Debug.Log("Monstre touché");

        if (other.CompareTag("Player"))
        {
            dead = true;
        }
    }
}
