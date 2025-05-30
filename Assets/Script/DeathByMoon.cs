using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathByMoon : MonoBehaviour
{

    [SerializeField]
    private DayNightCycleController controller;
    [SerializeField]
    private bool hasTriggeredDeath = false;
    public void OnDayBegins()
    {
        hasTriggeredDeath = false;
    }

    public void OnNightBegins()
    {

    }

    public void OnCycleUpdate(DayNightCycleController controller)
    {

        if (controller.IsNight() && controller.GetNightProgress() >= 1f && !controller.HasSkippedNight && !hasTriggeredDeath)
        {
            hasTriggeredDeath = true;

            Debug.Log("devrait etre MORT");

            SceneManager.LoadScene("Death");

            FindObjectOfType<Chronometre>().StopChronoAndWin();

        }
    }
}