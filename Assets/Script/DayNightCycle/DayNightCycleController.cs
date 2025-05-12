using UnityEngine;
using UnityEngine.Events;

public class DayNightCycleController : MonoBehaviour
{
    //public GameObject monstre;
    private float currentDayTime;

    [Header("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -")]
    [SerializeField, Range(0, 120)]
    private float dayDuration;
    [SerializeField, Range(0, 120)]
    private float nightDuration;
    [SerializeField]
    private UnityEvent onDayBegins, onNightBegins;
    [SerializeField]
    private UnityEvent<DayNightCycleController> onUpdate;


    void Start()
    {
        currentDayTime = 0;
        onDayBegins.Invoke();
    }

    
    void Update()
    {
        bool wasNight = IsNight();
        currentDayTime += Time.deltaTime;

        if (currentDayTime >= dayDuration + nightDuration)
            currentDayTime = 0;

        if (wasNight != IsNight())
        {
            if (wasNight)
                onDayBegins.Invoke();
            else
                onNightBegins.Invoke();
        }
        Debug.Log(onDayBegins);
        onUpdate.Invoke(this);
    }

    public bool IsNight() => currentDayTime >= dayDuration;
    public float GetNightProgress()
    {
        if (!IsNight())
        {
            return 0;
        }

        return (currentDayTime - dayDuration) / nightDuration;
    }
    public float GetDayProgress()
    {
        if (IsNight())
        {
            return 0;
        }

        return currentDayTime / dayDuration;
    }

   

}
