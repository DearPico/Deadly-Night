using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) 
        {
            SceneManager.LoadScene("jeu_7");
        }
    }
}
