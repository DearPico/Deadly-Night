using System;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    public void OnDayBegins()
    {
        gameObject.SetActive(false);
    }

    public void OnNightBegins()
    {
        gameObject.SetActive(true);
        GetComponent<AudioSource>().Play();

    }

    public void OnCycleUpdate(DayNightCycleController controller)
    {
    }
}
