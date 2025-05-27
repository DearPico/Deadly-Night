using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class DayNightCycleController : MonoBehaviour
{
    private float currentDayTime;
    private bool hasSkippedNight = false;
    private float initialNightDuration;

    [Header("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -")]
    [SerializeField, Range(0, 120)]
    private float dayDuration;
    [SerializeField, Range(0, 120)]
    private float nightDuration;

    [SerializeField]
    public ChangementDeSkinEtUI planqueDetector;
    [SerializeField]
    private UnityEvent onDayBegins, onNightBegins;
    [SerializeField]
    private UnityEvent<DayNightCycleController> onUpdate;

    void Start()
    {
        currentDayTime = 0;
        initialNightDuration = nightDuration;
        onDayBegins.Invoke();
    }


    void Update()
    {
        bool wasNight = IsNight();
        currentDayTime += Time.deltaTime;

        if (currentDayTime >= dayDuration + nightDuration)
        {
            currentDayTime = 0;
            nightDuration = initialNightDuration;
            hasSkippedNight = false;
        }

        if (wasNight != IsNight())
        {
            if (wasNight)
            {
                onDayBegins.Invoke();
            }
            else
            {
                onNightBegins.Invoke();
                nightDuration = initialNightDuration;
                hasSkippedNight = false;
            }
        }

        onUpdate.Invoke(this);


        if (planqueDetector.IsInPlanque && IsNight() && !hasSkippedNight)
        {
            currentDayTime = dayDuration + nightDuration;
            hasSkippedNight = true;
        }


        if (currentDayTime >= dayDuration + nightDuration)
        {
            // Lancer la scène suivante avant de réinitialiser le cycle
            SceneManager.LoadScene("Death"); // Remplace par le vrai nom de la scène

            currentDayTime = 0;
            nightDuration = initialNightDuration;
            hasSkippedNight = false;
        }

    }

    public bool IsNight() => currentDayTime >= dayDuration;

    public float GetNightProgress()
    {
        if (!IsNight()) return 0;
        return (currentDayTime - dayDuration) / nightDuration;
    }

    public float GetDayProgress()
    {
        if (IsNight()) return 0;
        return currentDayTime / dayDuration;
    }
}
