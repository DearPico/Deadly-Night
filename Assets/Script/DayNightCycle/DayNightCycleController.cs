using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DayNightCycleController : MonoBehaviour
{
    private float currentDayTime;
    private bool hasSkippedNight = false;
    private float initialNightDuration;

    [Header("- - - Cycle Settings - - -")]
    [SerializeField, Range(0, 120)]
    private float dayDuration = 30f;

    [SerializeField, Range(0, 120)]
    private float nightDuration = 30f;

    [Header("- - - Dependencies - - -")]
    [SerializeField]
    public ChangementDeSkinEtUI planqueDetector;

    [Header("- - - Events - - -")]
    [SerializeField]
    private UnityEvent onDayBegins, onNightBegins;

    [SerializeField]
    private UnityEvent<DayNightCycleController> onUpdate;

    void Start()
    {
        currentDayTime = 0f;
        initialNightDuration = nightDuration;
        onDayBegins.Invoke();
        Debug.Log("[DayNightCycle] Début du jour");
    }

    void Update()
    {
        currentDayTime += Time.deltaTime;
        bool wasNight = IsNight();
        bool isNowNight = IsNight();

        if (wasNight != isNowNight)
        {
            if (isNowNight)
            {
                onNightBegins.Invoke();
                hasSkippedNight = false;
                nightDuration = initialNightDuration;
               
            }
            else
            {
                onDayBegins.Invoke();
               
            }
        }

        onUpdate.Invoke(this);

        if (planqueDetector != null && planqueDetector.IsInPlanque && isNowNight && !hasSkippedNight)
        {
            currentDayTime = dayDuration + nightDuration;
            hasSkippedNight = true;
        }

        if (currentDayTime >= dayDuration + nightDuration)
        {
            if (!hasSkippedNight)
            {
                SceneManager.LoadScene("Death");
                return;
            }
            
            currentDayTime = 0f;
            nightDuration = initialNightDuration;
            hasSkippedNight = false;
        }
    }

    public bool IsNight() => currentDayTime >= dayDuration;

    public float GetNightProgress()
    {
        if (!IsNight()) return 0f;
        return (currentDayTime - dayDuration) / nightDuration;
    }

    public float GetDayProgress()
    {
        if (IsNight()) return 0f;
        return currentDayTime / dayDuration;
    }

    public bool HasSkippedNight => hasSkippedNight;
}
