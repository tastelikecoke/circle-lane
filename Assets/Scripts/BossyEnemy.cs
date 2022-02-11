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
    public Projectile sluggishPrefab;
    public Transform marker1;
    public Transform marker2;
    public Transform markerCenter;
    public LaserSegment laserSegment;
    public LineVisuals lineVisuals;

    private float remainingAttackCooldown = 0f;
    private float remainingPhaseTime = 0f;
    private float life = 300f;
    private int bigRepeat = 0;
    public float lifeMax = 300f;
    public int stage = 1;
    private bool dead = false;

    private List<Phase> phases;
    private Phase prepare;
    private Phase prepareSpreadAttack;
    private Phase spreadAttack;
    private Phase prepareFastBullets;
    private Phase fastBullets;
    private Phase prepareCloseFight;
    private Phase closeFight;
    private Phase moveToMarker1;
    private Phase moveToMarker2;
    private Phase moveToMarkerCenter;
    private Phase prepareCloseBlast;
    private Phase closeBlast;
    private Phase prepareSpreadAttack2;
    private Phase spreadAttack2;
    private Phase prepareSpreadAttack3;
    private Phase spreadAttack3;
    private Phase prepareLaser;
    private Phase laser;
    private Phase prepareCurtain;
    private Phase curtain;
    private Phase prepareLaser2;
    private Phase laser2;
    private Phase prepareFastAttack;
    private Phase fastAttack;

    /* stored aim should be in phases themselves? unless bolt */
    private Vector3 storedAim;
    private Vector3 storedAimPosition;

    /* movement parameters, don't use for aiming */
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

        public Phase MakeClone()
        {
            Phase newClone = new Phase();
            newClone.duration = duration;
            newClone.remainingTime = remainingTime;
            newClone.onEnter = onEnter;
            newClone.onUpdate = onUpdate;
            newClone.onExit = onExit;
            newClone.repeat = repeat;
            return newClone;
        }
    }

    void Awake()
    {
        phases = new List<Phase>();
        preparedProjectiles = new List<Projectile>();

        /* Movement */
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

        /* Stage 1 Phases */
        prepare = new Phase();
        prepare.duration = 3.0f;
        prepare.onEnter = () => {
            if(Random.Range(0,2) == 1)
                AddPhase(moveToMarker1);
            else
                AddPhase(moveToMarker2);
        };
        prepare.onExit = () => {
            AddPhase(prepareSpreadAttack);
        };
        /* Spread Attack */
        prepareSpreadAttack = new Phase();
        prepareSpreadAttack.duration = 1.2f;
        prepareSpreadAttack.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.yellow;
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
        
        /* Fast Bullets */
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

        /* Close Fight triggered */
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
            if(Random.Range(0,2) == 1)
                AddPhase(moveToMarker1);
            else
                AddPhase(moveToMarker2);
        };
        
        /* Stage 2 Phases */
        moveToMarkerCenter = new Phase();
        moveToMarkerCenter.duration = float.PositiveInfinity; // we wait infinitely
        moveToMarkerCenter.onEnter = () => {
            storedOrigin = transform.position;
            storedTarget = markerCenter.position + (Vector3)(Random.insideUnitCircle * 0.5f);
        };
        moveToMarkerCenter.onUpdate = () => {
            Vector3 inputDirection = (storedTarget - this.transform.position).normalized;
            this.transform.position += inputDirection * 9f * Time.deltaTime;
            if((storedTarget - this.transform.position).magnitude < 0.5f)
            {
                moveToMarkerCenter.remainingTime = 0f;
            }
        };
        moveToMarkerCenter.onExit = () => {
            AddPhase(prepareCloseBlast);
        };
        prepareCloseBlast = new Phase();
        prepareCloseBlast.duration = 0.6f;
        prepareCloseBlast.onEnter = () => {
            GetComponent<SpriteRenderer>().color = new Color(0.6f,0.1f,0.0f);
        };
        prepareCloseBlast.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            closeBlast.repeat = 0;
            AddPhase(closeBlast);
        };
        closeBlast = new Phase();
        closeBlast.duration = 0.6f;
        closeBlast.onEnter = () => {
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
            if(Random.Range(0,2) == 1)
                AddPhase(moveToMarker1);
            else
                AddPhase(moveToMarker2);
        };
        closeBlast.onExit = () => {
            AddPhase(prepareSpreadAttack2);
        };
        /* Spread Attack */
        prepareSpreadAttack2 = new Phase();
        prepareSpreadAttack2.duration = 1.6f;
        prepareSpreadAttack2.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        };
        prepareSpreadAttack2.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            spreadAttack2.repeat = 4;
            storedAim = GetDirectionTo(FightManager.instance.shooterPlayer.transform);
            AddPhase(spreadAttack2);
        };

        spreadAttack2 = new Phase();
        spreadAttack2.duration = 0.14f;
        spreadAttack2.onEnter = () => {
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
        spreadAttack2.onExit = () => {
            if(phases.Contains(moveToMarker1))
            {
                phases.Remove(moveToMarker1);
            }
            if(phases.Contains(moveToMarker2))
            {
                phases.Remove(moveToMarker2);
            }
            AddPhase(prepareLaser);
        };
        /* Laser Attack */
        prepareLaser = new Phase();
        prepareLaser.duration = 0.7f;
        prepareLaser.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.cyan;
            Vector3 playerDirection = GetDirectionTo(FightManager.instance.shooterPlayer.transform);
            storedAim = FightManager.instance.shooterPlayer.transform.position + (playerDirection * 15f);
            LineVisuals newLine = Instantiate(lineVisuals);
            newLine.Populate(this.transform.position, storedAim, 1.2f);
        };
        prepareLaser.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            AddPhase(laser);
            if(Random.Range(0,2) == 1)
                AddPhase(moveToMarker1);
            else
                AddPhase(moveToMarker2);
        };

        laser = new Phase();
        laser.duration = 1.5f;
        laser.onEnter = () => {
            LaserSegment newSegment = Instantiate(laserSegment);
            newSegment.gameObject.SetActive(true);
            newSegment.transform.position = this.transform.position;
            newSegment.Populate(storedAim, 2.1f, 0.017f, FightManager.instance.enemyFlag);
        };
        laser.onExit = () => {
            AddPhase(prepareSpreadAttack3);
        };

        spreadAttack3 = spreadAttack2.MakeClone();
        prepareSpreadAttack3 = prepareSpreadAttack2.MakeClone();
        prepareSpreadAttack3.duration = 1.6f;
        prepareSpreadAttack3.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            spreadAttack3.repeat = 4;
            storedAim = GetDirectionTo(FightManager.instance.shooterPlayer.transform);
            AddPhase(spreadAttack3);
        };
        spreadAttack3.onExit = () => {
            if(phases.Contains(moveToMarker1))
            {
                phases.Remove(moveToMarker1);
            }
            if(phases.Contains(moveToMarker2))
            {
                phases.Remove(moveToMarker2);
            }
            AddPhase(moveToMarkerCenter);
        };

        /* Stage 3 Phases */

        prepareCurtain = new Phase();
        prepareCurtain.duration = 0.5f;
        prepareCurtain.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        };
        prepareCurtain.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            curtain.repeat = 8;
            AddPhase(curtain);
        };
        curtain = new Phase();
        curtain.duration = 0.26f;
        curtain.onEnter = () => {
            Projectile makeProjectile(float angleOffset)
            {
                Projectile newBullet = Instantiate(sluggishPrefab);
                newBullet.gameObject.SetActive(true);
                newBullet.flag = FightManager.instance.enemyFlag;
                newBullet.direction = GetDirectionTo(FightManager.instance.shooterPlayer.transform);
                newBullet.direction =  Quaternion.Euler(0,0,angleOffset) * newBullet.direction;
                newBullet.transform.position = this.transform.position - (newBullet.direction * 0.2f);
                return newBullet;
            };
            float angleShift = 360f / 12f / 2f * curtain.repeat;
            for(int i = 0; i < 12; i++)
            {
                float currentAngle = i * (360f / 12);
                makeProjectile(currentAngle + angleShift);
            }
        };
        curtain.onExit = () => {
            if(lifeMax/2f > life)
            {
                bigRepeat = 2;
                AddPhase(prepareFastAttack);
            }
            else
            {
                if(phases.Contains(moveToMarker1))
                {
                    phases.Remove(moveToMarker1);
                }
                if(phases.Contains(moveToMarker2))
                {
                    phases.Remove(moveToMarker2);
                }
                AddPhase(prepareLaser2);
            }
        };

        
        prepareLaser2 = new Phase();
        prepareLaser2.duration = 0.6f;
        prepareLaser2.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.cyan;
            storedAim = FightManager.instance.shooterPlayer.transform.position;// + (playerDirection * 15f);
            for(float i = -1f; i < 1.5f; i+=1f)
            {
                Vector3 bossDisplacement = (this.transform.position - storedAim);
                bossDisplacement = Quaternion.Euler(0,0,10 * i) * bossDisplacement;
                Vector3 overshootAim = storedAim + (bossDisplacement.normalized * -15f);
                LineVisuals newLine = Instantiate(lineVisuals);
                newLine.Populate(storedAim + bossDisplacement, overshootAim, 1.2f);
            }
        };
        prepareLaser2.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            AddPhase(laser2);
            AddPhase(moveToMarker1);
        };

        laser2 = new Phase();
        laser2.duration = 1.5f;
        laser2.onEnter = () => {
            for(float i = -1f; i < 1.5f; i+=1f)
            {
                Vector3 bossDisplacement = (this.transform.position - storedAim);
                bossDisplacement = Quaternion.Euler(0,0,10 * i) * bossDisplacement;
                Vector3 overshootAim = storedAim + (bossDisplacement.normalized * -15f);
                LaserSegment newSegment = Instantiate(laserSegment);
                newSegment.gameObject.SetActive(true);
                newSegment.transform.position = storedAim + bossDisplacement;
                newSegment.Populate(overshootAim, 2.1f, 0.017f, FightManager.instance.enemyFlag);
            }
        };
        laser2.onExit = () => {
            if(lifeMax/2f > life)
            {
                bigRepeat = 2;
                AddPhase(prepareFastAttack);
            }
            else
            {
                AddPhase(prepareCurtain);
            }
        };

        
        /* Fast Bullets */
        prepareFastAttack = new Phase();
        prepareFastAttack.duration = 1.6f;
        prepareFastAttack.onEnter = () => {
            Projectile makeProjectile()
            {
                Projectile newBullet = Instantiate(threeBulletPrefab);
                newBullet.gameObject.SetActive(true);
                newBullet.flag = FightManager.instance.enemyFlag;
                newBullet.transform.position = this.transform.position;
                newBullet.direction = - GetDirectionTo(FightManager.instance.shooterPlayer.transform);
                newBullet.direction =  Quaternion.Euler(0,0,Random.Range(-5,5)) * newBullet.direction;
                newBullet.moveSpeed = 1.5f;
                return newBullet;
            }
            preparedProjectiles.Clear();
            preparedProjectiles.Add(makeProjectile());
            preparedProjectiles.Add(makeProjectile());
            preparedProjectiles.Add(makeProjectile());

            GetComponent<SpriteRenderer>().color = Color.red;
        };
        prepareFastAttack.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            fastAttack.repeat = 2;
            AddPhase(fastAttack);
        };

        fastAttack = new Phase();
        fastAttack.duration = 0.10f;
        fastAttack.repeat = 2;
        fastAttack.onEnter = () => {
            if(preparedProjectiles.Count > 0)
            {
                if(preparedProjectiles[0] != null)
                {
                    preparedProjectiles[0].direction = (FightManager.instance.shooterPlayer.transform.position - preparedProjectiles[0].transform.position).normalized;
                    preparedProjectiles[0].moveSpeed = 50f;
                }
                preparedProjectiles.RemoveAt(0);
                    
            }
        };
        fastAttack.onExit = () => {
            if(bigRepeat > 0)
            {
                bigRepeat -= 1;
                AddPhase(prepareFastAttack);
            }
            else
            {
                AddPhase(prepareCurtain);
            }
        };

    }

    void Start()
    {
        InitStage();
    }

    void InitStage()
    {
        phases.Clear();
        if(stage == 1)
        {
            /* Initial phases.  Also this is a really shitty data undriven code. */
            AddPhase(prepare);
            AddPhase(moveToMarker1);
        }
        else if(stage == 2)
        {
            AddPhase(prepareSpreadAttack2);
            AddPhase(moveToMarker1);
        }
        else if(stage == 3)
        {
            AddPhase(prepareCurtain);
            AddPhase(moveToMarker1);
        }
    }
    void AddPhase(Phase phase)
    {
        phases.Add(phase);
        phase.Reset();
        phase.onEnter();
    }
    void Update()
    {
        if(this.dead)
            return;
        for(int i = 0; i < phases.Count; i++)
        {
            Phase currentPhase = phases[i];
            
            if(currentPhase.remainingTime > 0)
            {
                if(currentPhase.onUpdate != null)
                {
                    currentPhase.onUpdate();
                }
                /* don't tick if infinite duration*/
                if(currentPhase.duration != float.PositiveInfinity)
                {
                    currentPhase.remainingTime -= Time.deltaTime;
                }
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
        if(stage == 1)
        {
            if((player.transform.position - transform.position).magnitude < 4.5f)
            {
                if(!(phases.Contains(prepareCloseFight) || phases.Contains(closeFight)))
                {
                    phases.Clear();
                    AddPhase(prepareCloseFight);
                }
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
                    ClearBossBullets();
                    if(stage > 2)
                    {
                        this.dead = true;
                        this.moveSpeed = 0;
                        this.transform.position = new Vector3(-1000, 0, 0);
                        // You win
                    }
                    else
                    {
                        stage += 1;
                        life = lifeMax;
                        InitStage();
                    }
                }
            }
        }
    }

    public void ClearBossBullets()
    {
        for(int i=0; i<FightManager.instance.projectiles.Count; i++)
        {
            GameObject projectile = FightManager.instance.projectiles[i];
            if(projectile == null) continue;
            if(projectile.GetComponent<Projectile>() != null)
            {
                if(projectile.GetComponent<Projectile>().flag == FightManager.instance.enemyFlag)
                {
                    projectile.GetComponent<Projectile>().Explode();
                    i--;
                }
            }
            if(projectile.GetComponent<LaserSegment>() != null)
            {
                if(projectile.GetComponent<LaserSegment>().flag == FightManager.instance.enemyFlag)
                {
                    projectile.GetComponent<LaserSegment>().Explode();
                    i--;
                }
            }
        }
    }

    public float GetLife()
    {
        return life;
    }
    public bool GetDeath()
    {
        return dead;
    }
}
