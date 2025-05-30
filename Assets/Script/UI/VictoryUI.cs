using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        float lastTime = PlayerPrefs.GetFloat("LastRunTime", 0f);

        float minutes = Mathf.FloorToInt(lastTime / 60);
        float seconds = Mathf.FloorToInt(lastTime % 60);

        scoreText.text = "Temps final : " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}