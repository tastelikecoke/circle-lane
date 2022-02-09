using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{
    public Text lifeText;
    public Image healthMax;
    public Image health;
    public Image bossHealth;
    public Image bossHealthMax;

    void Update()
    {
        if(FightManager.instance.shooterPlayer != null)
        {
            float lifeValue = FightManager.instance.shooterPlayer.GetLife();
            Vector2 healthSize = healthMax.GetComponent<RectTransform>().sizeDelta;
            healthSize.x *= lifeValue / 100f;
            health.GetComponent<RectTransform>().sizeDelta = healthSize;
        }
        if(FightManager.instance.currentBoss != null)
        {
            float lifeValue = FightManager.instance.currentBoss.GetLife();
            Vector2 healthSize = bossHealthMax.GetComponent<RectTransform>().sizeDelta;
            healthSize.x *= lifeValue / FightManager.instance.currentBoss.lifeMax;
            bossHealth.GetComponent<RectTransform>().sizeDelta = healthSize;
        }
    }
}
