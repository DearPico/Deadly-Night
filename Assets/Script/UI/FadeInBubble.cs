using System.Collections;
using UnityEngine;

public class FadeInBubble : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Camera mainCamera;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Color c = spriteRenderer.color;
        c.a = 0f;
        spriteRenderer.color = c;
    }

    public void StartFadeIn(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(duration));
    }

    private IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;
        Color c = spriteRenderer.color;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, 1f);
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        transform.forward = mainCamera.transform.forward;
    }
}
