using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{
    public Text lifeText;
    public Image healthMax;
    public Image health;

    void Update()
    {
        if(FightManager.instance.shooterPlayer != null)
        {
            float lifeValue = FightManager.instance.shooterPlayer.GetLife();
            Vector2 healthSize = healthMax.GetComponent<RectTransform>().sizeDelta;
            healthSize.x *= lifeValue / 100f;
            health.GetComponent<RectTransform>().sizeDelta = healthSize;
        }
    }
}
