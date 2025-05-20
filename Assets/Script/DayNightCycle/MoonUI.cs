using System;
using UnityEngine;
using UnityEngine.UI;


public class MoonUI : MonoBehaviour
{
    [SerializeField, Range(0,1)]
    private Image moonFillImage;

    public void OnDayBegins()
    {
        
        moonFillImage.enabled = false;

    }

    public void OnNightBegins()
    {
        moonFillImage.enabled = true;
    }

    public void OnCycleUpdate(DayNightCycleController controller)
    {
        float t = controller.GetNightProgress();
        moonFillImage.fillAmount = t;
    }

}