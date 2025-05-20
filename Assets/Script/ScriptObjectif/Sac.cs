using System;
using UnityEngine;
using UnityEngine.UI;


public class Sac : MonoBehaviour
{

    [SerializeField]
    private Gradient gradient;

    public GameObject modelBase;
    public GameObject modelSac;
    public GameObject modelChien;


    public Image sacados;
    public Image chien;

    private bool sacCheck = false;
    private bool chienCheck = false;


    void Start()
    {
    }

    void Update()
    {
        if (sacCheck)
        {
            sacados.color = gradient.Evaluate(1);
            ModelSwapSac();

        }

        if (chienCheck)
        {
            chien.color = gradient.Evaluate(1);
            ModelSwapChien();
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
}

