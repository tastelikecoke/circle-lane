using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager instance = null;
    public ShooterPlayer shooterPlayer;
    public GameObject explosionPrefab;
    public string enemyFlag = "enemy";
    public string allyFlag = "ally";
    void Awake()
    {
        FightManager.instance = this;
    }
}
