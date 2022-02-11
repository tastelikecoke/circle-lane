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
    public List<GameObject> projectiles;
    public string enemyFlag = "enemy";
    public string allyFlag = "ally";
    void Awake()
    {
        FightManager.instance = this;
        projectiles = new List<GameObject>();
    }

    public void Restart()
    {
        SceneManager.LoadScene("TestScene1");
    }
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
