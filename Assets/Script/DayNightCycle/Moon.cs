using UnityEngine;

public class Moon : MonoBehaviour
{
    public Camera mainCamera;

    [SerializeField] 
    private SpriteRenderer spriteRenderer;
    
    [SerializeField]
    private Gradient gradient;

    void Update()
    {
        if (mainCamera == null)
            return;

        transform.forward = mainCamera.transform.forward;
    }


    public void OnDayBegins()
    {
        spriteRenderer.enabled = false;
    }

    public void OnNightBegins()
    {
        spriteRenderer.enabled = true;
    }

    public void OnCycleUpdate(DayNightCycleController controller)
    {
        float t = controller.GetNightProgress();
        spriteRenderer.color = gradient.Evaluate(t);
    }
}
