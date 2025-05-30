using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Chronometre : MonoBehaviour
{
    public float elapsedTime = 0f;
    public bool chronometreIsRunning = false;
    public Text timeText;


    private void Start()
    {

        chronometreIsRunning = true;
    }

    void Update()
    {
        if (chronometreIsRunning)
        {
            elapsedTime += Time.deltaTime;
            DisplayTime(elapsedTime);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void StopChronoAndWin()
    {
        chronometreIsRunning = false;

        // Sauvegarde du temps dans PlayerPrefs
        PlayerPrefs.SetFloat("LastRunTime", elapsedTime);
        PlayerPrefs.Save();

        // Charger la scène de victoire
        SceneManager.LoadScene("Death"); // Remplace par le nom exact de ta scène de victoire
    }

}