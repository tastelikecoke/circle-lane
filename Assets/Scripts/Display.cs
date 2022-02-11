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
    public GameObject youDiedObject;
    public GameObject youWinObject;

    void Update()
    {
        if(FightManager.instance.shooterPlayer != null)
        {
            float lifeValue = FightManager.instance.shooterPlayer.GetLife();
            float healthRectWidth = healthMax.GetComponent<RectTransform>().rect.width * lifeValue / 100f;
            health.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthRectWidth);
        }
        if(FightManager.instance.currentBoss != null)
        {
            float lifeValue = FightManager.instance.currentBoss.GetLife();
            float healthRectWidth = bossHealthMax.GetComponent<RectTransform>().rect.width * lifeValue / FightManager.instance.currentBoss.lifeMax;
            bossHealth.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthRectWidth);
        }
        if(FightManager.instance.shooterPlayer != null)
        {
            youDiedObject.gameObject.SetActive(FightManager.instance.shooterPlayer.GetDeath());
        }
        if(FightManager.instance.currentBoss != null)
        {
            youWinObject.gameObject.SetActive(FightManager.instance.currentBoss.GetDeath());
        }
    }
}
