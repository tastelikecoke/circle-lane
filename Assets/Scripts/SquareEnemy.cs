using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareEnemy : MonoBehaviour
{
    public Vector3 direction;
    public float moveSpeed = 2f; // per second
    public float attackCooldown = 1f;
    public Projectile projectilePrefab;

    private float remainingAttackCooldown = 0f;
    private float life = 100f;
    void Update()
    {
        float computedDistance = moveSpeed * Time.deltaTime;
        this.transform.position += direction * computedDistance;

        if(remainingAttackCooldown <= 0f)
        {
            Projectile newBullet = Instantiate(projectilePrefab);
            newBullet.gameObject.SetActive(true);
            newBullet.flag = FightManager.instance.enemyFlag;
            newBullet.transform.position = this.transform.position;
            newBullet.direction = GetDirectionTo(FightManager.instance.shooterPlayer.transform);
            remainingAttackCooldown += attackCooldown;
        }
        if(remainingAttackCooldown > 0f)
            remainingAttackCooldown -= Time.deltaTime;
    }

    Vector3 GetDirectionTo(Transform target)
    {
        return (target.position - this.transform.position).normalized;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.GetComponent<Projectile>() != null)
        {
            if(collider.gameObject.GetComponent<Projectile>().flag == FightManager.instance.allyFlag)
            {
                life -= 10f;
                collider.gameObject.GetComponent<Projectile>().Explode();
                if(life <= 0f)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
