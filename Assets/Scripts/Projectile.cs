using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 direction;
    public float moveSpeed;
    public float maxLifetime = 10f;
    public string flag = "";
    
    private float lifetime = 0f;
    void Update()
    {
        lifetime += Time.deltaTime;
        this.transform.position += direction * moveSpeed * Time.deltaTime;
        if(lifetime > maxLifetime)
            Destroy(this.gameObject);
    }
    public void Explode()
    {
        Destroy(this.gameObject);
    }
}
