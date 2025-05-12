using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField]
    private Light sunLight;

    [SerializeField]
    private bool isNight;

    public void OnDayBegins()
    {
        sunLight.enabled = true;
        isNight = false;
    }

    public void OnNightBegins()
    {
        sunLight.enabled = false;
        isNight = true;
    }

    public void OnCycleUpdate(DayNightCycleController controller)
    {
        float t = controller.GetDayProgress();
        float angle = Mathf.Lerp(-20f, 200f, t);

        transform.eulerAngles = new Vector3()
        {
            x = angle,
            y = 0,
            z = 0
        };

       
     
    }
}

