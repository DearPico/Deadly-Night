using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ChangementDeSkinEtUI : MonoBehaviour
{

    [SerializeField]
    public Gradient gradientUIFalse;

    [SerializeField]
    public Gradient gradientUITrue;

    public GameObject modelBase;
    public GameObject modelSac;
    public GameObject modelChien;


    public Image sacados;
    public Image chien;

    private bool sacCheck = false;
    private bool chienCheck = false;

    private bool hasFadedOut = false;




    public bool IsInPlanque { get; private set; }


    private IEnumerator FadeOutImage(Image img, Gradient gradient, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            img.color = gradient.Evaluate(t); 
            yield return null;
        }

      
        Color endColor = gradient.Evaluate(1f);
        endColor.a = 0f;
        img.color = endColor;

        Destroy(img.gameObject);
    }

    private IEnumerator FadeOutDelay(Image img, Gradient gradientUITrue, float delay, float fadeDuration)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(FadeOutImage(img, gradientUITrue, fadeDuration));
    }



    void Update()
    {
        if (sacCheck && !hasFadedOut)
        {
            sacados.color = gradientUIFalse.Evaluate(1f);
            sacados.GetComponent<Animator>().enabled = false;


            StartCoroutine(FadeOutDelay(sacados, gradientUITrue, 5f, 2f));
            hasFadedOut = true;
        }

        if (chienCheck)
        {
            chien.color = gradientUIFalse.Evaluate(1);
            ModelSwapChien();
            chien.GetComponent<Animator>().enabled = false;

            StartCoroutine(FadeOutDelay(chien, gradientUITrue, 5f, 2f));
            hasFadedOut = true;


        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sac"))
        {
            Destroy(other.gameObject);
            sacCheck = true;
        }

        if (other.CompareTag("Chien"))
        {
            Destroy(other.gameObject);
            chienCheck = true;
        }
        if (other.CompareTag("Planque"))
        {
            IsInPlanque = true;
            Debug.Log("planquecheck");
        }
    }

    void ModelSwapSac()
    {
        modelBase.gameObject.SetActive(false);
        modelSac.gameObject.SetActive(true);
    }

    void ModelSwapChien()
    {
        modelSac.gameObject.SetActive(false);
        modelChien.gameObject.SetActive(true);
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Planque"))
        {
            IsInPlanque = false;
        }
    }


}

