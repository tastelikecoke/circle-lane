using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossyEnemy : MonoBehaviour
{
    public Vector3 direction;
    public float moveSpeed = 2f; // per second
    public float attackCooldown = 1f;
    public Projectile projectilePrefab;
    public Projectile threeBulletPrefab;
    public Transform marker1;
    public Transform marker2;

    private float remainingAttackCooldown = 0f;
    private float remainingPhaseTime = 0f;
    private float life = 300f;

    private List<Phase> phases;
    private Phase prepareSpreadAttack;
    private Phase spreadAttack;
    private Phase prepareFastBullets;
    private Phase fastBullets;
    private Phase prepareCloseFight;
    private Phase closeFight;
    private Phase moveToMarker1;
    private Phase moveToMarker2;

    private Vector3 storedAim;
    private Vector3 storedOrigin;
    private Vector3 storedTarget;
    private List<Projectile> preparedProjectiles;

    public class Phase
    {
        public float duration = 0f;
        public float remainingTime = 0f;
        public System.Action onEnter;
        public System.Action onUpdate;
        public System.Action onExit;
        public int repeat = 0;

        public void Reset()
        {
            remainingTime = duration;
        }
    }

    void Awake()
    {
        phases = new List<Phase>();
        preparedProjectiles = new List<Projectile>();

        prepareSpreadAttack = new Phase();
        prepareSpreadAttack.duration = 1.2f;
        prepareSpreadAttack.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.blue;
        };
        prepareSpreadAttack.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            spreadAttack.repeat = 4;
            storedAim = GetDirectionTo(FightManager.instance.shooterPlayer.transform);
            AddPhase(spreadAttack);
        };

        spreadAttack = new Phase();
        spreadAttack.duration = 0.14f;
        spreadAttack.onEnter = () => {
            Projectile makeProjectile()
            {
                Projectile newBullet = Instantiate(projectilePrefab);
                newBullet.gameObject.SetActive(true);
                newBullet.flag = FightManager.instance.enemyFlag;
                newBullet.transform.position = this.transform.position;
                newBullet.direction = storedAim;
                return newBullet;
            };
            Projectile bullet1 = makeProjectile();
            Projectile bullet2 = makeProjectile();
            bullet2.direction =  Quaternion.Euler(0,0,-15) * bullet2.direction;
            Projectile bullet3 = makeProjectile();
            bullet3.direction =  Quaternion.Euler(0,0,15) * bullet3.direction;

        };
        spreadAttack.onExit = () => {
            AddPhase(prepareFastBullets);
        };
        
        prepareFastBullets = new Phase();
        prepareFastBullets.duration = 1.6f;
        prepareFastBullets.onEnter = () => {
            Projectile makeProjectile()
            {
                Projectile newBullet = Instantiate(threeBulletPrefab);
                newBullet.gameObject.SetActive(true);
                newBullet.flag = FightManager.instance.enemyFlag;
                newBullet.transform.position = this.transform.position;
                newBullet.direction = - GetDirectionTo(FightManager.instance.shooterPlayer.transform);
                newBullet.direction =  Quaternion.Euler(0,0,Random.Range(-10,10)) * newBullet.direction;
                newBullet.moveSpeed = 1.5f;
                return newBullet;
            }
            preparedProjectiles.Clear();
            preparedProjectiles.Add(makeProjectile());
            preparedProjectiles.Add(makeProjectile());
            preparedProjectiles.Add(makeProjectile());

            GetComponent<SpriteRenderer>().color = Color.red;
        };
        prepareFastBullets.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            fastBullets.repeat = 2;
            AddPhase(fastBullets);
        };

        fastBullets = new Phase();
        fastBullets.duration = 0.10f;
        fastBullets.repeat = 2;
        fastBullets.onEnter = () => {
            if(preparedProjectiles.Count > 0)
            {
                preparedProjectiles[0].direction = (FightManager.instance.shooterPlayer.transform.position - preparedProjectiles[0].transform.position).normalized;
                preparedProjectiles[0].moveSpeed = 50f;
                preparedProjectiles.RemoveAt(0);
            }
        };
        fastBullets.onExit = () => {
            AddPhase(prepareSpreadAttack);
        };

        prepareCloseFight = new Phase();
        prepareCloseFight.duration = 0.6f;
        prepareCloseFight.onEnter = () => {
            GetComponent<SpriteRenderer>().color = new Color(0.6f,0.1f,0.0f);
        };
        prepareCloseFight.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            closeFight.repeat = 0;
            AddPhase(closeFight);
        };

        closeFight = new Phase();
        closeFight.duration = 0.2f;
        closeFight.onEnter = () => {
            Projectile makeProjectile(float angleOffset)
            {
                Projectile newBullet = Instantiate(projectilePrefab);
                newBullet.gameObject.SetActive(true);
                newBullet.flag = FightManager.instance.enemyFlag;
                newBullet.direction = GetDirectionTo(FightManager.instance.shooterPlayer.transform);
                newBullet.direction =  Quaternion.Euler(0,0,angleOffset) * newBullet.direction;
                newBullet.transform.position = this.transform.position - (newBullet.direction * 0.2f);
                return newBullet;
            };
            for(int i = 0; i < 24; i++)
            {
                float currentAngle = i * (360f / 24);
                makeProjectile(currentAngle);
            }

        };
        closeFight.onExit = () => {
            AddPhase(prepareSpreadAttack);
            AddPhase(moveToMarker2);
        };

        moveToMarker1 = new Phase();
        moveToMarker1.duration = 4.0f;
        moveToMarker1.onEnter = () => {
            storedOrigin = transform.position;
            storedTarget = marker1.position + (Vector3)(Random.insideUnitCircle * 0.5f);
        };
        moveToMarker1.onUpdate = () => {
            float remainingT = moveToMarker1.remainingTime / moveToMarker1.duration;
            transform.position = Vector3.Lerp(storedTarget, storedOrigin, remainingT);
        };
        moveToMarker1.onExit = () => {
            AddPhase(moveToMarker2);
        };

        moveToMarker2 = new Phase();
        moveToMarker2.duration = 4.0f;
        moveToMarker2.onEnter = () => {
            storedOrigin = transform.position;
            storedTarget = marker2.position + (Vector3)(Random.insideUnitCircle * 0.5f);
        };
        moveToMarker2.onUpdate = () => {
            float remainingT = moveToMarker2.remainingTime / moveToMarker2.duration;
            transform.position = Vector3.Lerp(storedTarget, storedOrigin, remainingT);
        };
        moveToMarker2.onExit = () => {
            AddPhase(moveToMarker1);
        };
        

        AddPhase(prepareSpreadAttack);
        AddPhase(moveToMarker1);
    }
    void AddPhase(Phase phase)
    {
        phases.Add(phase);
        phase.Reset();
        phase.onEnter();
    }
    void Update()
    {
        for(int i = 0; i < phases.Count; i++)
        {
            Phase currentPhase = phases[i];
            if(currentPhase.remainingTime > 0)
            {
                if(currentPhase.onUpdate != null)
                {
                    currentPhase.onUpdate();
                }
                currentPhase.remainingTime -= Time.deltaTime;
            }
            else if(currentPhase.repeat > 0)
            {
                currentPhase.repeat -= 1;
                currentPhase.onEnter();
                currentPhase.Reset();
            }
            else
            {
                if(currentPhase.onExit != null)
                    currentPhase.onExit();
                phases.RemoveAt(i);
                i--;
            }
        }
        ShooterPlayer player = FightManager.instance.shooterPlayer;
        if((player.transform.position - transform.position).magnitude < 4.5f)
        {
            if(!(phases.Contains(prepareCloseFight) || phases.Contains(closeFight)))
            {
                phases.Clear();
                AddPhase(prepareCloseFight);
            }
        }
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
