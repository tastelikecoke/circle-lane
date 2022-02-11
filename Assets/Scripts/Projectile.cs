using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 direction = Vector3.zero;
    public float moveSpeed;
    public float maxLifetime = 10f;
    public string flag = "";
    
    private float lifetime = 0f;
    void Awake()
    {
        if(FightManager.instance != null)
            FightManager.instance.projectiles.Add(this.gameObject);
    }
    void Update()
    {
        lifetime += Time.deltaTime;
        this.transform.position += direction * moveSpeed * Time.deltaTime;
        if(lifetime > maxLifetime)
            Explode();
    }
    public void Explode()
    {
        if(FightManager.instance != null)
            FightManager.instance.projectiles.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
