using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightManager : MonoBehaviour
{
    public static FightManager instance = null;
    public ShooterPlayer shooterPlayer;
    public BossyEnemy currentBoss;
    public GameObject explosionPrefab;
    public string enemyFlag = "enemy";
    public string allyFlag = "ally";
    void Awake()
    {
        FightManager.instance = this;
    }

    public void Restart()
    {
        SceneManager.LoadScene("TestScene1");
    }
}
