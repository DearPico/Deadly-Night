using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class DarkestVision : MonoBehaviour
{
    public Transform player;
    public string monsterTag = "Monster";
    public float maxDistance = 20f;
    public Image darkOverlay;

    void Update()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag(monsterTag);
        float closestDistance = Mathf.Infinity;

        foreach (GameObject monster in monsters)
        {
            float dist = Vector3.Distance(player.position, monster.transform.position);
            if (dist < closestDistance)
                closestDistance = dist;
        }

        float t = Mathf.Clamp01(1 - (closestDistance / maxDistance));
        Color color = darkOverlay.color;
        color.a = Mathf.Lerp(0f, 0.7f, t);  
        darkOverlay.color = color;
    }
}
