using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("jeu_8");
        }
    }
}
